import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-server-error',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mt-5">
        <div class="alert alert-danger shadow-sm">
            <h4 class="alert-heading mb-0"><i class="fa fa-exclamation-triangle me-2"></i> Internal Server Error</h4>
            @if(error) {
                <hr>
                <p class="mb-0 font-monospace small">{{ error.message }}</p>
                <p class="mt-2 text-muted small">Note: Check the browser console for more details.</p>
            }
        </div>
        <button (click)="router.navigateByUrl('/shop')" class="btn btn-outline-secondary mt-3">Back to Home</button>
    </div>
  `
})
export class ServerErrorComponent {
  public router = inject(Router);
  error: any;

  constructor() {
    // We will pass the error details via the router state
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.['error'];
  }
}
