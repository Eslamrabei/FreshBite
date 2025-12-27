import { Component, Input, inject } from '@angular/core';
import { CurrencyPipe, CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Product } from '../../shared/models/pagination';
import { BasketService } from '../../basket/basket.service';


@Component({
  selector: 'app-product-item',
  standalone: true,
  imports: [CommonModule, RouterLink], // Ensure imports are here
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss']
})
export class ProductItemComponent {
  @Input() product?: Product;
  basketService = inject(BasketService);

  addItemToBasket() {
    if (this.product) {
      this.basketService.addItemToBasket(this.product);
      console.log('Added to basket via Child Component!'); // Debug log
    }
  }
}
