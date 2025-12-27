import { Component, Input, ViewChild, inject, Output, EventEmitter, OnInit, input, output } from '@angular/core'; // 1. Added Output, EventEmitter
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { StripeService, StripeCardComponent } from 'ngx-stripe';
import { StripeCardElementOptions, StripeElementsOptions } from '@stripe/stripe-js';

import { BasketService } from '../../../basket/basket.service';
import { CheckoutService } from '../checkout.service';
import { AccountService } from '../../../account/account.service';



@Component({
  selector: 'app-checkout-payment',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, StripeCardComponent],
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})
export class CheckoutPaymentComponent implements OnInit {
  checkoutForm = input.required<FormGroup>();
  back = output();
  @ViewChild(StripeCardComponent) card?: StripeCardComponent;



  basketService = inject(BasketService);
  private checkoutService = inject(CheckoutService);
  private accountService = inject(AccountService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private stripeService = inject(StripeService);


  ngOnInit() {
    this.getAddressFormValues();
    this.getPaymentIntent();
  }

  getPaymentIntent() {
    this.checkoutService.createPaymentIntent()?.subscribe({
      next: (basket: any) => {
        console.log('Stripe Secret Received!', basket.clientSecret);
      },
      error: error => console.log('Backend Error:', error)
    })
  }

  getAddressFormValues() {
    this.accountService.getUserAddress().subscribe({
      next: address => {
        if (address)
          this.checkoutForm()?.get('addressForm')?.patchValue(address);
      },
      error: err => console.log('No saved address found', err)
    })
  }


  cardOptions: StripeCardElementOptions = {
    style: {
      base: {
        iconColor: '#666EE8',
        color: '#31325F',
        lineHeight: '40px',
        fontWeight: 300,
        fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
        fontSize: '18px',
        '::placeholder': {
          color: '#CFD7E0'
        }
      }
    }
  };

  elementsOptions: StripeElementsOptions = {
    locale: 'en'
  };

  loading = false;

  submitOrder() {
    // 1. Get the actual form from the Signal
  const form = this.checkoutForm();

  // 2. Check the status
  console.log('Form Status:', form.status);

  // 3. Loop to find the invalid field
  Object.keys(form.controls).forEach(key => {
    const control = form.get(key);
    if (control?.invalid) {
      console.log(`❌ Invalid Control: ${key}`);

      // If it's a group (like 'addressForm'), look inside
      if (control instanceof FormGroup) {
         Object.keys(control.controls).forEach(innerKey => {
            const innerControl = control.get(innerKey);
            if (innerControl?.invalid) {
               console.log(`   👉 Inside ${key}: ❌ ${innerKey} is Missing or Invalid`);
               console.log('      Current Value:', innerControl.value);
            }
         });
      } else {
          // If it's a simple field (like nameOnCard)
          console.log('   Errors:', control.errors);
          console.log('   Current Value:', control.value);
      }
    }
  });

    // 1. Check Form Validity
    if (!this.checkoutForm()?.valid) {
      console.log('Form Invalid', this.checkoutForm()?.errors);
      return;
    }

    // 2. Check Basket & Secret
    const basket = this.basketService.getCurrentBasketValue();
    console.log('2. Current Basket:', basket);

    if (!basket || !basket.clientSecret) {
      console.log('ERROR: Missing Basket or ClientSecret');
      this.toastr.error('Cannot verify payment intent');
      return;
    }

    this.loading = true;
    console.log('3. Starting Stripe Confirm with Secret:', basket.clientSecret);

    const cardElement = this.card!.element;

    // 3. Talk to Stripe
    this.stripeService.confirmCardPayment(basket.clientSecret, {
      payment_method: {
        card: cardElement,
        billing_details: {
          name: this.checkoutForm().get('paymentForm')?.get('nameOnCard')?.value
        }
      }
    }).subscribe({
      next: (result) => {
        console.log('4. Stripe Result:', result); // <--- CRITICAL LOG

        if (result.paymentIntent) {
          console.log('5. Payment Success! Creating Order...');
          this.createOrder(basket);
        } else if (result.error) {
          console.log('Stripe Error:', result.error.message);
          this.toastr.error(result.error.message);
          this.loading = false;
        }
      },
      error: err => {
        console.log('Stripe HTTP Error:', err);
        this.toastr.error(err.message);
        this.loading = false;
      }
    })
  }

  private createOrder(basket: any) {
    const orderToCreate = {
      basketId: basket.id,
      deliveryMethodId: +this.checkoutForm()?.get('deliveryForm')?.get('deliveryMethod')?.value,
      shipToAddress: this.checkoutForm()?.get('addressForm')?.value
    };

    this.checkoutService.createOrder(orderToCreate).subscribe({
      next: (order: any) => {
        this.toastr.success('Order created successfully');
        this.basketService.deleteBasket(basket);
        const navigationExtras = { state: { orderId: order.id } };
        this.router.navigate(['checkout/success'], navigationExtras);
        this.loading = false;
      },
      error: err => {
        this.toastr.error(err.message);
        this.loading = false;
      }
    });
  }
}
