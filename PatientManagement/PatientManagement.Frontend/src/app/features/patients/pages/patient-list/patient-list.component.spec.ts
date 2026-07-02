import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Confirmation, ConfirmationService, MessageService } from 'primeng/api';
import { environment } from '../../../../environments/environment';
import { ApiResponse, PagedResult, Patient } from '../../../../models/patient';
import { PatientListComponent } from './patient-list.component';

describe('PatientListComponent', () => {
  let fixture: ComponentFixture<PatientListComponent>;
  let component: PatientListComponent;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/patients`;

  const patient: Patient = {
    patientId: 1,
    documentType: 'DNI',
    documentNumber: '123456',
    firstName: 'Ana',
    lastName: 'Lopez',
    birthDate: '1990-01-01',
    phoneNumber: '555-0100',
    email: 'ana@example.com',
    createdAt: '2026-07-01T10:00:00Z',
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientListComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        provideNoopAnimations(),
        MessageService,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PatientListComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should load data correctly', () => {
    fixture.detectChanges();
    flushPatients([patient]);
    fixture.detectChanges();

    expect(component.loading()).toBeFalse();
    expect(fixture.nativeElement.textContent).toContain('Ana Lopez');
  });

  it('should render the table', () => {
    fixture.detectChanges();
    flushPatients([patient]);
    fixture.detectChanges();

    const table = fixture.nativeElement.querySelector('p-table');
    expect(table).not.toBeNull();
    expect(fixture.nativeElement.textContent).toContain('Doc. Number');
    expect(fixture.nativeElement.textContent).toContain('123456');
  });

  it('should apply filters', fakeAsync(() => {
    fixture.detectChanges();
    flushPatients([patient]);
    fixture.detectChanges();

    component.onNameChange('Ana');
    tick(300);

    const req = httpMock.expectOne(`${apiUrl}?Name=Ana&Page=1&PageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush(createPagedResponse([patient]));
  }));

  it('should delete correctly', () => {
    fixture.detectChanges();
    flushPatients([patient]);

    const confirmationService = fixture.debugElement.injector.get(ConfirmationService);
    spyOn(confirmationService, 'confirm').and.callFake((confirmation: Confirmation) => {
      confirmation.accept?.();
      return confirmationService;
    });

    component.onDelete(patient);

    const deleteReq = httpMock.expectOne(`${apiUrl}/1`);
    expect(deleteReq.request.method).toBe('DELETE');
    deleteReq.flush(null);

    flushPatients([]);
    fixture.detectChanges();

    expect(confirmationService.confirm).toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('No patients found');
  });

  function flushPatients(items: Patient[]): void {
    const req = httpMock.expectOne(`${apiUrl}?Page=1&PageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush(createPagedResponse(items));
  }

  function createPagedResponse(items: Patient[]): ApiResponse<PagedResult<Patient>> {
    return {
      success: true,
      message: 'ok',
      data: {
        items,
        totalCount: items.length,
        page: 1,
        pageSize: 10,
        totalPages: items.length ? 1 : 0,
        hasPreviousPage: false,
        hasNextPage: false,
      },
    };
  }
});
