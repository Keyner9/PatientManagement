import { Routes } from '@angular/router';
import { PatientListComponent } from './pages/patient-list/patient-list.component';
import { PatientFormComponent } from './pages/patient-form/patient-form.component';
import { PatientDetailComponent } from './pages/patient-detail/patient-detail.component';

export default [
  { path: '', component: PatientListComponent },
  { path: 'new', component: PatientFormComponent },
  { path: ':id', component: PatientDetailComponent },
  { path: ':id/edit', component: PatientFormComponent },
] as Routes;
