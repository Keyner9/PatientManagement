import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { environment } from '../../../../environments/environment';
import { ApiResponse, Appointment, Patient } from '../../../../models/patient';
import { PatientDetailComponent } from './patient-detail.component';

describe('PatientDetailComponent', () => {
  let fixture: ComponentFixture<PatientDetailComponent>;
  let component: PatientDetailComponent;
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

  const appointment: Appointment = {
    appointmentId: 1,
    appointmentDate: '2026-07-10T14:00:00Z',
    diagnosis: 'General checkup',
    treatment: 'None',
    fee: 100,
    doctorName: 'Dr. Smith',
    doctorSpecialty: 'General',
    createdAt: '2026-07-10T14:00:00Z',
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientDetailComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideNoopAnimations(),
        MessageService,
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { params: { id: '1' } },
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PatientDetailComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', () => {
    fixture.detectChanges();
    flushAll();
    expect(component).toBeTruthy();
  });

  it('should load patient data', () => {
    fixture.detectChanges();
    flushAll();

    expect(component.loading()).toBeFalse();
    expect(component.patient()).toEqual(patient);
  });

  it('should render patient details', () => {
    fixture.detectChanges();
    flushAll();

    expect(component.patient()?.firstName).toBe('Ana');
    expect(component.patient()?.lastName).toBe('Lopez');
    expect(component.patient()?.documentNumber).toBe('123456');
    expect(component.patient()?.email).toBe('ana@example.com');
  });

  it('should load appointments', () => {
    fixture.detectChanges();
    flushAll([appointment]);

    expect(component.loadingAppointments()).toBeFalse();
    expect(component.appointments().length).toBe(1);
  });

  it('should show empty message when no appointments', () => {
    fixture.detectChanges();
    flushAll([]);

    expect(component.loadingAppointments()).toBeFalse();
    expect(component.appointments().length).toBe(0);
  });

  function flushAll(appts: Appointment[] = [appointment]): void {
    const patientReq = httpMock.expectOne(`${apiUrl}/1`);
    patientReq.flush({ success: true, message: 'ok', data: patient } as ApiResponse<Patient>);

    const apptReq = httpMock.expectOne(`${apiUrl}/1/appointments`);
    apptReq.flush({ success: true, message: 'ok', data: appts } as ApiResponse<Appointment[]>);
  }
});
