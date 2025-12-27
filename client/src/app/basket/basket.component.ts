import { Component, inject } from '@angular/core';
import { BasketService } from './basket.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { BasketItem } from '../shared/models/basket';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderTotals } from "../shared/components/order-totals/order-totals";

@Component({
  selector: 'app-basket.component',
  imports: [CurrencyPipe, RouterLink, OrderTotals],
  templateUrl: './basket.component.html',
  styleUrl: './basket.component.scss',
})
export class BasketComponent {

  private basketService = inject(BasketService);


  basket = toSignal(this.basketService.basket$);
  totals = toSignal(this.basketService.basketTotal$);

  incrementItem(item: BasketItem) {
    this.basketService.incrementItemQuantity(item);
  }

  decrementItem(item: BasketItem) {
    this.basketService.decrementItemQuantity(item);
  }

  removeItem(item: BasketItem) {
    this.basketService.removeItemFromBasket(item);
  }
}
