import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { Patient, PatientFilter, PagedResult, CreatePatientDto, UpdatePatientDto, ApiResponse } from '../models/patient';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly apiUrl = `${environment.apiUrl}/patients`;

  constructor(private http: HttpClient) {}

  getAll(filter: PatientFilter): Observable<ApiResponse<PagedResult<Patient>>> {
    const params = new URLSearchParams();
    if (filter.name) params.set('Name', filter.name);
    if (filter.documentNumber) params.set('DocumentNumber', filter.documentNumber);
    if (filter.page) params.set('Page', filter.page.toString());
    if (filter.pageSize) params.set('PageSize', filter.pageSize.toString());
    return this.http.get<ApiResponse<PagedResult<Patient>>>(`${this.apiUrl}?${params}`);
  }

  getById(id: number): Observable<ApiResponse<Patient>> {
    return this.http.get<ApiResponse<Patient>>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreatePatientDto): Observable<ApiResponse<Patient>> {
    return this.http.post<ApiResponse<Patient>>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdatePatientDto): Observable<ApiResponse<Patient>> {
    return this.http.put<ApiResponse<Patient>>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
