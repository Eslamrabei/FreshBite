import { Component, EventEmitter, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasketService } from '../../../basket/basket.service';



@Component({
  selector: 'app-checkout-review',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss']
})
export class CheckoutReviewComponent {
  @Output() next = new EventEmitter<void>();
  @Output() back = new EventEmitter<void>();

  basketService = inject(BasketService);
  basket = this.basketService.basket$;

}
