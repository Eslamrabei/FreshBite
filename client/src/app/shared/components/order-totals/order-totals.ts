import { Component, inject } from '@angular/core';
import { BasketService } from '../../../basket/basket.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-order-totals',
  imports: [CurrencyPipe],
  templateUrl: './order-totals.html',
  styleUrl: './order-totals.scss',
})
export class OrderTotals {

  private basketService = inject(BasketService);

  totals = toSignal(this.basketService.basketTotal$);
}
