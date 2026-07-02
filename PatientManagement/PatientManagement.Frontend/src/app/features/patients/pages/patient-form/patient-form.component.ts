import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { SkeletonModule } from 'primeng/skeleton';
import { CreatePatientDto, UpdatePatientDto } from '../../../../models/patient';
import { PatientService } from '../../../../services/patient.service';

interface DocumentTypeOption {
  label: string;
  value: string;
}

type PatientFormValue = {
  documentType: string;
  documentNumber: string;
  firstName: string;
  lastName: string;
  birthDate: Date | string;
  phoneNumber: string;
  email: string;
};

@Component({
  selector: 'app-patient-form',
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    CardModule,
    DatePickerModule,
    InputTextModule,
    SelectModule,
    SkeletonModule,
  ],
  templateUrl: './patient-form.component.html',
  styleUrl: './patient-form.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientFormComponent {
  readonly #fb = inject(FormBuilder);
  readonly #route = inject(ActivatedRoute);
  readonly #router = inject(Router);
  readonly #destroyRef = inject(DestroyRef);
  readonly #patientService = inject(PatientService);
  readonly #messageService = inject(MessageService);

  readonly documentTypes: DocumentTypeOption[] = [
    { label: 'DNI', value: 'DNI' },
    { label: 'CE', value: 'CE' },
    { label: 'Passport', value: 'Passport' },
  ];

  readonly isEditMode = signal(false);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  readonly form = this.#fb.nonNullable.group({
    documentType: ['', Validators.required],
    documentNumber: ['', Validators.required],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    birthDate: this.#fb.nonNullable.control<Date | string>('', Validators.required),
    phoneNumber: [''],
    email: ['', Validators.email],
  });

  readonly title = signal('New Patient');
  #patientId: number | null = null;

  constructor() {
    const id = Number(this.#route.snapshot.paramMap.get('id'));

    if (Number.isInteger(id) && id > 0) {
      this.#patientId = id;
      this.isEditMode.set(true);
      this.title.set('Edit Patient');
      this.#loadPatient(id);
    }
  }

  onSubmit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const dto = this.#toDto(this.form.getRawValue());
    const request$ = this.isEditMode()
      ? this.#patientService.update(this.#patientId!, dto as UpdatePatientDto)
      : this.#patientService.create(dto);

    request$.pipe(takeUntilDestroyed(this.#destroyRef)).subscribe({
      next: () => {
        this.#messageService.add({
          severity: 'success',
          summary: 'Saved',
          detail: `Patient ${this.isEditMode() ? 'updated' : 'created'} successfully.`,
          life: 3000,
        });
        void this.#router.navigate(['/patients']);
      },
      error: (error: HttpErrorResponse) => {
        this.submitting.set(false);
        if (error.status === 409) {
          this.form.controls.documentNumber.setErrors({ duplicate: true });
        }
      },
    });
  }

  cancel(): void {
    void this.#router.navigate(['/patients']);
  }

  hasError(controlName: keyof PatientFormValue, error: string): boolean {
    const control = this.form.controls[controlName];
    return control.hasError(error) && (control.dirty || control.touched);
  }

  #loadPatient(id: number): void {
    this.loading.set(true);
    this.#patientService
      .getById(id)
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: ({ data }) => {
          this.form.patchValue({
            documentType: data.documentType,
            documentNumber: data.documentNumber,
            firstName: data.firstName,
            lastName: data.lastName,
            birthDate: this.#toDate(data.birthDate),
            phoneNumber: data.phoneNumber ?? '',
            email: data.email ?? '',
          });
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  #toDto(value: PatientFormValue): CreatePatientDto {
    return {
      documentType: value.documentType.trim(),
      documentNumber: value.documentNumber.trim(),
      firstName: value.firstName.trim(),
      lastName: value.lastName.trim(),
      birthDate: this.#formatDate(value.birthDate),
      phoneNumber: value.phoneNumber.trim() || undefined,
      email: value.email.trim() || undefined,
    };
  }

  #formatDate(value: Date | string): string {
    if (value instanceof Date) {
      return value.toISOString().slice(0, 10);
    }

    return value;
  }

  #toDate(value: string): Date | string {
    const parsed = new Date(value);
    return Number.isNaN(parsed.getTime()) ? value : parsed;
  }
}
