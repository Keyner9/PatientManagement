import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../environments/environment';
import { ApiResponse, CreatePatientDto, PagedResult, Patient, UpdatePatientDto } from '../models/patient';
import { PatientService } from './patient.service';

describe('PatientService', () => {
  let service: PatientService;
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

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(PatientService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get patients with filters', () => {
    const response: ApiResponse<PagedResult<Patient>> = {
      success: true,
      message: 'ok',
      data: {
        items: [patient],
        totalCount: 1,
        page: 1,
        pageSize: 10,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      },
    };

    service.getAll({ name: 'Ana', documentNumber: '123', page: 1, pageSize: 10 }).subscribe((result) => {
      expect(result).toEqual(response);
    });

    const req = httpMock.expectOne(`${apiUrl}?Name=Ana&DocumentNumber=123&Page=1&PageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush(response);
  });

  it('should get patient by id', () => {
    const response: ApiResponse<Patient> = { success: true, message: 'ok', data: patient };

    service.getById(1).subscribe((result) => {
      expect(result.data.patientId).toBe(1);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(response);
  });

  it('should create patient', () => {
    const dto: CreatePatientDto = {
      documentType: patient.documentType,
      documentNumber: patient.documentNumber,
      firstName: patient.firstName,
      lastName: patient.lastName,
      birthDate: patient.birthDate,
      phoneNumber: patient.phoneNumber,
      email: patient.email,
    };
    const response: ApiResponse<Patient> = { success: true, message: 'created', data: patient };

    service.create(dto).subscribe((result) => {
      expect(result.data).toEqual(patient);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(dto);
    req.flush(response);
  });

  it('should update patient', () => {
    const dto: UpdatePatientDto = {
      documentType: patient.documentType,
      documentNumber: patient.documentNumber,
      firstName: 'Ana Maria',
      lastName: patient.lastName,
      birthDate: patient.birthDate,
      phoneNumber: patient.phoneNumber,
      email: patient.email,
    };
    const response: ApiResponse<Patient> = {
      success: true,
      message: 'updated',
      data: { ...patient, firstName: dto.firstName },
    };

    service.update(1, dto).subscribe((result) => {
      expect(result.data.firstName).toBe('Ana Maria');
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush(response);
  });

  it('should delete patient', () => {
    service.delete(1).subscribe((result) => {
      expect(result).toBeNull();
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
