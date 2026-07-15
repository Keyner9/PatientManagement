import { ChangeDetectionStrategy, Component, computed, DestroyRef, inject, signal, ViewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AsyncPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, shareReplay, switchMap, tap } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TableModule, Table } from 'primeng/table';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { CalendarModule } from 'primeng/calendar';
import { ConfirmationService, MessageService } from 'primeng/api';
import { SkeletonModule } from 'primeng/skeleton';
import { ToolbarModule } from 'primeng/toolbar';
import { FormsModule } from '@angular/forms';
import { TooltipModule } from 'primeng/tooltip';
import { PatientService } from '../../../../services/patient.service';
import { ApiResponse, Patient, PatientFilter, PagedResult } from '../../../../models/patient';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [
    RouterLink, AsyncPipe, DatePipe, NgIf, NgFor, FormsModule,
    TableModule, PaginatorModule, InputTextModule, ButtonModule,
    ConfirmDialogModule, DialogModule, CalendarModule,
    SkeletonModule, ToolbarModule, TooltipModule,
  ],
  templateUrl: './patient-list.component.html',
  styleUrls: ['./patient-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [ConfirmationService],
})
export class PatientListComponent {
  readonly #destroyRef = inject(DestroyRef);
  readonly #patientService = inject(PatientService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #messageService = inject(MessageService);

  @ViewChild('table') table!: Table;

  readonly pageSize = signal(10);

  readonly #filterSubject = new BehaviorSubject<PatientFilter>({ page: 1, pageSize: this.pageSize() });

  readonly #nameSubject = new Subject<string>();
  readonly #documentSubject = new Subject<string>();

  readonly nameFilterValue = signal('');
  readonly documentFilterValue = signal('');
  readonly loading = signal(true);

  readonly hasActiveFilters = computed(() => !!this.nameFilterValue() || !!this.documentFilterValue());

  readonly reportDialogVisible = signal(false);
  readonly reportDate = signal<Date | null>(null);

  readonly #response$: Observable<ApiResponse<PagedResult<Patient>>> = this.#filterSubject.pipe(
    tap(() => this.loading.set(true)),
    switchMap((filter) => this.#patientService.getAll(filter).pipe(finalize(() => this.loading.set(false)))),
    shareReplay(1),
  );

  readonly patients$ = this.#response$.pipe(map((r) => r.data?.items ?? []));
  readonly totalCount$ = this.#response$.pipe(map((r) => r.data?.totalCount ?? 0));

  constructor() {
    this.#nameSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.#destroyRef))
      .subscribe((name) => {
        this.nameFilterValue.set(name);
        this.#applyFilter();
      });

    this.#documentSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.#destroyRef))
      .subscribe((doc) => {
        this.documentFilterValue.set(doc);
        this.#applyFilter();
      });
  }

  onNameChange(value: string): void {
    this.#nameSubject.next(value);
  }

  onDocumentNumberChange(value: string): void {
    this.#documentSubject.next(value);
  }

  onClearFilters(): void {
    this.nameFilterValue.set('');
    this.documentFilterValue.set('');
    this.#nameSubject.next('');
    this.#documentSubject.next('');
    this.#filterSubject.next({ page: 1, pageSize: this.pageSize() });
  }

  refresh(): void {
    const current = this.#filterSubject.value;
    this.#filterSubject.next({ ...current });
  }

  onPageChange(event: PaginatorState): void {
    const page = (event.page ?? 0) + 1;
    this.pageSize.set(event.rows ?? 10);
    this.#filterSubject.next({ ...this.#filterSubject.value, page, pageSize: this.pageSize() });
  }

  onDelete(patient: Patient): void {
    this.#confirmationService.confirm({
      message: `Esta seguro de que desea eliminar a ${patient.firstName} ${patient.lastName}?`,
      header: 'Confirmar eliminacion',
      icon: 'pi pi-exclamation-triangle',
      accept: () => this.#deletePatient(patient),
    });
  }

  #deletePatient(patient: Patient): void {
    this.#patientService
      .delete(patient.patientId)
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: () => {
          this.#messageService.add({
            severity: 'success',
            summary: 'Eliminado',
            detail: `${patient.firstName} ${patient.lastName} ha sido eliminado.`,
            life: 3000,
          });
          this.refresh();
        },
      });
  }

  exportCSV(): void {
    this.table.exportCSV();
  }

  openReportDialog(): void {
    this.reportDate.set(null);
    this.reportDialogVisible.set(true);
  }

  downloadReport(): void {
    const url = this.#patientService.getReportUrl(this.reportDate() ?? undefined);
    window.open(url, '_blank');
    this.reportDialogVisible.set(false);
  }

  #applyFilter(): void {
    this.#filterSubject.next({
      name: this.nameFilterValue() || undefined,
      documentNumber: this.documentFilterValue() || undefined,
      page: 1,
      pageSize: this.pageSize(),
    });
  }

  rowTrackBy(_index: number, item: Patient): number {
    return item.patientId;
  }
}
