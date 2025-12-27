import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error) {

        // Handle 400 Bad Request
        if (error.status === 400) {
            // Check if it's a validation error (array of errors)
            if (error.error.errors) {
                // (Optional) You can loop through them or just throw them back to the component
                throw error.error;
            } else {
                toastr.error(error.error.message || error.statusText, error.status.toString());
            }
        }

        // Handle 401 Unauthorized
        if (error.status === 401) {
          toastr.error(error.error.message || 'Unauthorized', error.status.toString());
        }

        // Handle 404 Not Found
        if (error.status === 404) {
          router.navigateByUrl('/not-found');
        }

        // Handle 500 Server Error
        if (error.status === 500) {
          // Pass the error details to the component so we can see what happened
          const navigationExtras = { state: { error: error.error } };
          router.navigateByUrl('/server-error', navigationExtras);
        }
      }
      return throwError(() => error);
    })
  );
};
