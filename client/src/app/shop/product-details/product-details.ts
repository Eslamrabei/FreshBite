import { Component, computed, inject, OnInit, signal, Signal } from '@angular/core';
import { Product } from '../../shared/models/pagination';
import { ShopService } from '../shop.service';
import { ActivatedRoute } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { BasketService } from '../../basket/basket.service';

@Component({
  selector: 'app-product-details',
  imports: [CurrencyPipe],
  templateUrl: './product-details.html',
  styleUrl: './product-details.scss',
})
export class ProductDetails implements OnInit {
  product = signal<Product | null>(null);
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute);
  private basketService = inject(BasketService);

  quantity = 1;


  ngOnInit(): void {
    this.loadProductById();
  }

  loadProductById() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if (id) {
      this.shopService.getProductById(+id).subscribe({
        next: (response: Product) => {
          this.product.update((data) => data = response);
          console.log(response);
        },
        error: (error) => console.log(error)
      })
    }
  }

  incrementQuantity() {
    this.quantity++;
  }

  decrementQuantity() {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  addItemToBasket() {
    const item = this.product();
    if (item) {
      // Pass the quantity to your basket service
      this.basketService.addItemToBasket(item, this.quantity);
    }
  }

}
