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
      let detail = 'Ocurrio un error inesperado.';

      if (error.status === 0) {
        detail = 'No se pudo conectar al servidor. Por favor, verifique su conexion.';
      } else if (error.status === 400) {
        detail = error.error?.title || 'Solicitud invalida.';
      } else if (error.status === 404) {
        detail = 'Recurso no encontrado.';
      } else if (error.status === 409) {
        detail = error.error || 'Conflicto de recurso.';
      } else if (error.status >= 500) {
        detail = 'Error del servidor. Por favor, intente de nuevo mas tarde.';
      }

      messageService.add({ severity: 'error', summary, detail, life: 5000 });

      return throwError(() => error);
    })
  );
};
