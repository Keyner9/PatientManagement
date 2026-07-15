import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe, Location, NgFor, NgIf } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { PatientService } from '../../../../services/patient.service';
import { ApiResponse, Appointment, Patient } from '../../../../models/patient';

@Component({
  selector: 'app-patient-detail',
  standalone: true,
  imports: [RouterLink, DatePipe, CurrencyPipe, NgIf, NgFor, CardModule, ButtonModule, SkeletonModule, TableModule],
  templateUrl: './patient-detail.component.html',
  styleUrls: ['./patient-detail.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientDetailComponent {
  readonly #patientService = inject(PatientService);
  readonly #route = inject(ActivatedRoute);
  readonly #location = inject(Location);
  readonly #destroyRef = inject(DestroyRef);

  readonly loading = signal(true);
  readonly loadingAppointments = signal(true);
  readonly patient = signal<Patient | null>(null);
  readonly appointments = signal<Appointment[]>([]);

  constructor() {
    const id = Number(this.#route.snapshot.params['id']);
    if (id) {
      this.#loadPatient(id);
      this.#loadAppointments(id);
    }
  }

  #loadPatient(id: number): void {
    this.loading.set(true);
    this.#patientService.getById(id).pipe(takeUntilDestroyed(this.#destroyRef)).subscribe({
      next: (res: ApiResponse<Patient>) => {
        this.patient.set(res.data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  #loadAppointments(id: number): void {
    this.loadingAppointments.set(true);
    this.#patientService.getAppointments(id).pipe(takeUntilDestroyed(this.#destroyRef)).subscribe({
      next: (res: ApiResponse<Appointment[]>) => {
        this.appointments.set(res.data ?? []);
        this.loadingAppointments.set(false);
      },
      error: () => this.loadingAppointments.set(false),
    });
  }

  goBack(): void {
    this.#location.back();
  }
}
