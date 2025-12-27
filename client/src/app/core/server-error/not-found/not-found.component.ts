import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="container mt-5">
        <div class="d-flex justify-content-center align-items-center flex-column" style="min-height: 60vh;">
            <i class="fa fa-search fa-5x text-muted mb-4"></i>
            <h1 class="fw-bold">404 - Page Not Found</h1>
            <p class="text-muted mb-4">We couldn't find what you were looking for.</p>
            <button routerLink="/shop" class="btn btn-primary rounded-pill px-4 shadow-sm">
                Go to Shop
            </button>
        </div>
    </div>
  `
})
export class NotFoundComponent {}
