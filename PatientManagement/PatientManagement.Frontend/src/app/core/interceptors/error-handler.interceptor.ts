import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export const errorHandlerInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let summary = 'Error';
      let detail = 'An unexpected error occurred.';

      if (error.status === 0) {
        detail = 'Could not connect to the server. Please check your connection.';
      } else if (error.status === 400) {
        detail = error.error?.title || 'Invalid request.';
      } else if (error.status === 404) {
        detail = 'Resource not found.';
      } else if (error.status === 409) {
        detail = error.error || 'Resource conflict.';
      } else if (error.status >= 500) {
        detail = 'Server error. Please try again later.';
      }

      messageService.add({ severity: 'error', summary, detail, life: 5000 });

      return throwError(() => error);
    })
  );
};
