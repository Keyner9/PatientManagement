import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { MessageService } from 'primeng/api';
import { PatientFormComponent } from './patient-form.component';

describe('PatientFormComponent', () => {
  let fixture: ComponentFixture<PatientFormComponent>;
  let component: PatientFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientFormComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        provideNoopAnimations(),
        MessageService,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PatientFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be invalid initially', () => {
    expect(component.form.invalid).toBeTrue();
  });

  it('should be valid when required fields and valid email are provided', () => {
    component.form.setValue({
      documentType: 'DNI',
      documentNumber: '123456',
      firstName: 'Ana',
      lastName: 'Lopez',
      birthDate: new Date(1990, 0, 1),
      phoneNumber: '555-0100',
      email: 'ana@example.com',
    });

    expect(component.form.valid).toBeTrue();
  });

  it('should validate required fields', () => {
    component.form.controls.documentNumber.markAsTouched();
    component.form.controls.firstName.markAsTouched();
    component.form.controls.lastName.markAsTouched();
    component.form.controls.birthDate.markAsTouched();

    expect(component.form.controls.documentNumber.hasError('required')).toBeTrue();
    expect(component.form.controls.firstName.hasError('required')).toBeTrue();
    expect(component.form.controls.lastName.hasError('required')).toBeTrue();
    expect(component.form.controls.birthDate.hasError('required')).toBeTrue();
  });

  it('should validate email format', () => {
    component.form.controls.email.setValue('invalid-email');
    component.form.controls.email.markAsTouched();

    expect(component.form.controls.email.hasError('email')).toBeTrue();

    component.form.controls.email.setValue('valid@example.com');

    expect(component.form.controls.email.hasError('email')).toBeFalse();
  });
});
