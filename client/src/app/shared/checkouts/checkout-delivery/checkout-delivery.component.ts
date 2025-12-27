import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BasketService } from '../../../basket/basket.service';

@Component({
  selector: 'app-checkout-delivery',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkout-delivery.component.html',
  styleUrls: ['./checkout-delivery.component.scss']
})
export class CheckoutDeliveryComponent implements OnInit {
  @Input() checkoutForm?: FormGroup; // <--- Received
  @Output() next = new EventEmitter<void>();
  @Output() back = new EventEmitter<void>();

  public basketService = inject(BasketService);

  // Mock Data
  deliveryMethods = [
    { id: 1, shortName: 'UPS1', deliveryTime: '1-2 Days', description: 'Fastest delivery time', price: 10 },
    { id: 2, shortName: 'UPS2', deliveryTime: '2-5 Days', description: 'Get it within 5 days', price: 5 },
    { id: 3, shortName: 'UPS3', deliveryTime: '5-10 Days', description: 'Slower but cheap', price: 2 },
    { id: 4, shortName: 'FREE', deliveryTime: '1-2 Weeks', description: 'Free! You get what you pay for', price: 0 },
  ];

  ngOnInit(): void {
    const basket = this.basketService.getCurrentBasketValue();
    if (basket?.deliveryMethodId && this.checkoutForm) {
      this.checkoutForm.get('deliveryForm')?.get('deliveryMethod')
        ?.patchValue(basket.deliveryMethodId.toString());
    }
  }

  // Getter for HTML
  get deliveryForm() {
    return this.checkoutForm?.get('deliveryForm') as FormGroup;
  }

  submitDelivery() {
    const form = this.deliveryForm;
    if (form && form.valid) {
      const selectedId = form.get('deliveryMethod')?.value;
      const method = this.deliveryMethods.find(x => x.id === +selectedId);

      if (method) {
        this.basketService.setShippingPrice(method);
      }
      this.next.emit();
    }
  }
}
