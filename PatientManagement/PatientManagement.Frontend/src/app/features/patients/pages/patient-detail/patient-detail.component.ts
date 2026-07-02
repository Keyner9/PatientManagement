import { AsyncPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { catchError, map, Observable, of, shareReplay, switchMap } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { SkeletonModule } from 'primeng/skeleton';
import { Patient } from '../../../../models/patient';
import { PatientService } from '../../../../services/patient.service';

@Component({
  selector: 'app-patient-detail',
  imports: [AsyncPipe, DatePipe, RouterLink, ButtonModule, CardModule, SkeletonModule],
  templateUrl: './patient-detail.component.html',
  styleUrl: './patient-detail.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientDetailComponent {
  readonly #route = inject(ActivatedRoute);
  readonly #router = inject(Router);
  readonly #patientService = inject(PatientService);

  readonly patient$: Observable<Patient | null> = this.#route.paramMap.pipe(
    map((params) => Number(params.get('id'))),
    switchMap((id) => {
      if (!Number.isInteger(id) || id <= 0) {
        void this.#router.navigate(['/patients']);
        return of(null);
      }

      return this.#patientService.getById(id).pipe(
        map((response) => response.data),
        catchError(() => of(null)),
      );
    }),
    shareReplay({ bufferSize: 1, refCount: true }),
  );
}
