import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Location, NgIf } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { MessageModule } from 'primeng/message';
import { HttpErrorResponse } from '@angular/common/http';
import { PatientService } from '../../../../services/patient.service';
import { ApiResponse, Patient, CreatePatientDto, UpdatePatientDto } from '../../../../models/patient';

const DOCUMENT_TYPES: string[] = ['DNI', 'CE', 'PASSPORT'];

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [
    ReactiveFormsModule, NgIf,
    CardModule, InputTextModule, ButtonModule, CalendarModule, DropdownModule, MessageModule,
  ],
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientFormComponent {
  readonly #fb = inject(FormBuilder);
  readonly #patientService = inject(PatientService);
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);
  readonly #location = inject(Location);
  readonly #destroyRef = inject(DestroyRef);

  readonly documentTypes = DOCUMENT_TYPES;
  readonly isEdit = signal(false);
  readonly loading = signal(false);
  readonly pageTitle = signal('Nuevo Paciente');
  readonly patientId = signal<number | null>(null);

  readonly form = this.#fb.nonNullable.group({
    documentType: ['DNI', Validators.required],
    documentNumber: ['', [Validators.required, Validators.maxLength(20)]],
    firstName: ['', [Validators.required, Validators.maxLength(80)]],
    lastName: ['', [Validators.required, Validators.maxLength(80)]],
    birthDate: [null as Date | null, Validators.required],
    phoneNumber: ['', Validators.maxLength(20)],
    email: ['', [Validators.email, Validators.maxLength(120)]],
  });

  constructor() {
    const id = this.#route.snapshot.params['id'];
    if (id) {
      this.isEdit.set(true);
      this.pageTitle.set('Editar Paciente');
      this.patientId.set(Number(id));
      this.#loadPatient(Number(id));
    }
  }

  #loadPatient(id: number): void {
    this.loading.set(true);
    this.#patientService.getById(id).pipe(takeUntilDestroyed(this.#destroyRef)).subscribe({
      next: (res: ApiResponse<Patient>) => {
        const p = res.data;
        const birthDateParts = p.birthDate.split('-');
        const birthDate = new Date(Number(birthDateParts[0]), Number(birthDateParts[1]) - 1, Number(birthDateParts[2]));
        this.form.patchValue({
          documentType: p.documentType,
          documentNumber: p.documentNumber,
          firstName: p.firstName,
          lastName: p.lastName,
          birthDate: birthDate,
          phoneNumber: p.phoneNumber ?? '',
          email: p.email ?? '',
        });
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    const raw = this.form.getRawValue();
    const bd = raw.birthDate as Date;
    const birthDateStr = `${bd.getFullYear()}-${String(bd.getMonth() + 1).padStart(2, '0')}-${String(bd.getDate()).padStart(2, '0')}`;

    if (this.isEdit()) {
      const dto: UpdatePatientDto = {
        documentType: raw.documentType,
        documentNumber: raw.documentNumber,
        firstName: raw.firstName,
        lastName: raw.lastName,
        birthDate: birthDateStr,
        phoneNumber: raw.phoneNumber || undefined,
        email: raw.email || undefined,
      };
      this.#patientService.update(this.patientId()!, dto).pipe(takeUntilDestroyed(this.#destroyRef)).subscribe({
        next: () => this.#router.navigate(['/patients']),
        error: (err: HttpErrorResponse) => this.#handleError(err),
      });
    } else {
      const dto: CreatePatientDto = {
        documentType: raw.documentType,
        documentNumber: raw.documentNumber,
        firstName: raw.firstName,
        lastName: raw.lastName,
        birthDate: birthDateStr,
        phoneNumber: raw.phoneNumber || undefined,
        email: raw.email || undefined,
      };
      this.#patientService.create(dto).pipe(takeUntilDestroyed(this.#destroyRef)).subscribe({
        next: () => this.#router.navigate(['/patients']),
        error: (err: HttpErrorResponse) => this.#handleError(err),
      });
    }
  }

  #handleError(err: HttpErrorResponse): void {
    this.loading.set(false);
    if (err.status === 409) {
      this.form.controls.documentNumber.setErrors({ duplicate: true });
    }
  }

  goBack(): void {
    this.#location.back();
  }
}
