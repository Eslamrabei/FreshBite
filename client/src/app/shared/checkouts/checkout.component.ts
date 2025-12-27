import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';


// Child Components
import { CheckoutAddressComponent } from './checkout-address/checkout-address.component';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { CheckoutPaymentComponent } from './checkout-payment/checkout-payment.component';
import { AccountService } from '../../account/account.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CheckoutAddressComponent,
    CheckoutDeliveryComponent,
    CheckoutReviewComponent,
    CheckoutPaymentComponent
  ],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);

  currentStep = signal<number>(1);

  // --- THE MASTER FORM ---
  checkoutForm: FormGroup = this.fb.group({
    addressForm: this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      street: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      state: ['', Validators.required],
      zipCode: ['', Validators.required],
    }),
    deliveryForm: this.fb.group({
      deliveryMethod: ['', Validators.required]
    }),
    paymentForm: this.fb.group({
      nameOnCard: ['', Validators.required]
    })
  });

  ngOnInit() {
    this.getAddressFormValues();
  }

  // Auto-fill address from API if user saved it before
  getAddressFormValues() {
    this.accountService.getUserAddress().subscribe({
      next: address => {
        if (address) {
          this.checkoutForm.get('addressForm')?.patchValue(address);
        }
      }
    });
  }

  nextStep() {
    this.currentStep.update(v => Math.min(v + 1, 4));
  }

  prevStep() {
    this.currentStep.update(v => Math.max(v - 1, 1));
  }

  goToStep(step: number) {
    this.currentStep.set(step);
  }
}
