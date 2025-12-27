import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checkout-success',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './checkout-success.component.html',
  styles: [`
    .success-icon {
      font-size: 5rem;
      color: #198754; // Bootstrap Success Green
      animation: popIn 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }

    @keyframes popIn {
      0% { opacity: 0; transform: scale(0.5); }
      100% { opacity: 1; transform: scale(1); }
    }
  `]
})
export class CheckoutSuccessComponent {
  private router = inject(Router);
  orderId?: number;

  constructor() {
    // Attempt to get the order ID passed from the previous page
    const navigation = this.router.getCurrentNavigation();
    const state = navigation?.extras?.state as { orderId: number };

    if (state) {
      this.orderId = state.orderId;
    }
  }
}
