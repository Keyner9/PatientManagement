import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe, Location } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { PatientService } from '../../../../services/patient.service';
import { ApiResponse, Patient } from '../../../../models/patient';

@Component({
  selector: 'app-patient-detail',
  imports: [RouterLink, DatePipe, CardModule, ButtonModule, SkeletonModule],
  templateUrl: './patient-detail.component.html',
  styleUrl: './patient-detail.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientDetailComponent {
  readonly #patientService = inject(PatientService);
  readonly #route = inject(ActivatedRoute);
  readonly #location = inject(Location);
  readonly #destroyRef = inject(DestroyRef);

  readonly loading = signal(true);
  readonly patient = signal<Patient | null>(null);

  constructor() {
    const id = Number(this.#route.snapshot.params['id']);
    if (id) {
      this.#loadPatient(id);
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

  goBack(): void {
    this.#location.back();
  }
}
