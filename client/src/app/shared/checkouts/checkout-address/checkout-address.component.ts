import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-checkout-address',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss']
})
export class CheckoutAddressComponent {
  @Input() checkoutForm?: FormGroup; // <--- Received from Parent
  @Output() next = new EventEmitter<void>();

  // This getter lets your HTML stay exactly the same!
  get addressForm() {
    return this.checkoutForm?.get('addressForm') as FormGroup;
  }

  saveAddress() {
    if (this.addressForm?.valid) {
      // You can save to API here if you want persistent address updates
      this.next.emit();
    } else {
      this.addressForm?.markAllAsTouched();
    }
  }
}
