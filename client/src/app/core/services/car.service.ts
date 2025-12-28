import { computed, Injectable, signal } from "@angular/core";
import { ProductSearchResponse } from "../../shared/models/product-search-response";


@Injectable({ providedIn: 'root' })
export class CartService {
  private cartItems = signal<ProductSearchResponse[]>([]);

  count = computed(() => this.cartItems().length);

  addToCart(product: ProductSearchResponse) {
    this.cartItems.update((items) => [...items, product]);
    console.log(`Cart item updateed with product ${product}`)
  }

  totalPrice = computed(() => this.cartItems().reduce((sum, item) => sum + item.price, 0));

}
