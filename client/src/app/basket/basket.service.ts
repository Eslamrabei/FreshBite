import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, map } from 'rxjs'; // Removed retryWhen (unused)
import { Basket, BasketItem, BasketTotals } from '../shared/models/basket';
import { HttpClient } from '@angular/common/http';
import { Product } from '../shared/models/pagination';
import { IDeliveryMethod } from '../shared/models/deliveryMethod'; // Import if you have it, or use 'any'

@Injectable({
  providedIn: 'root',
})
export class BasketService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  private basketSource = new BehaviorSubject<Basket | null>(null);
  basket$ = this.basketSource.asObservable();

  private basketTotalSource = new BehaviorSubject<BasketTotals | null>(null);
  basketTotal$ = this.basketTotalSource.asObservable();

  getBasketById(id: string) {
    return this.http.get<Basket>(this.baseUrl + 'basket?key=' + id).pipe(
      map(basket => {
        this.basketSource.next(basket);
        this.calculateTotal();
        return basket;
      })
    );
  }

  // Use this to update the basket on the API (Redis)
  setBasket(basket: Basket) {
    return this.http.post<Basket>(this.baseUrl + 'basket', basket).subscribe({
      next: response => {
        this.basketSource.next(response);
        this.calculateTotal(); // Recalculate totals after saving
      },
      error: error => console.log(error)
    });
  }

  getCurrentBasketValue() {
    return this.basketSource.value;
  }

  addItemToBasket(item: Product, quantity = 1) {
    const itemToAdd: BasketItem = this.mapProductToBasketItem(item, quantity);
    const basket = this.getCurrentBasketValue() ?? this.createBasket();
    basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);
    this.setBasket(basket);
  }

  incrementItemQuantity(item: BasketItem) {
    const basket = this.getCurrentBasketValue();
    if (!basket) return;

    const index = basket.items.findIndex(x => x.id === item.id);
    if (index !== -1) {
      basket.items[index].quantity++;
      this.setBasket(basket);
    }
  }

  decrementItemQuantity(item: BasketItem) {
    const basket = this.getCurrentBasketValue();
    if (!basket) return;

    const index = basket.items.findIndex(x => x.id === item.id);
    if (index !== -1) {
      if (basket.items[index].quantity > 1) {
        basket.items[index].quantity--;
        this.setBasket(basket);
      } else {
        this.removeItemFromBasket(item);
      }
    }
  }

  removeItemFromBasket(item: BasketItem) {
    const basket = this.getCurrentBasketValue();
    console.log("BASKET:<> ", basket)
    if (!basket) return;

    if (basket.items.some(x => x.id === item.id)) {
      basket.items = basket.items.filter(i => i.id !== item.id);
      console.log("BASKETITEMS:<> ", basket.items)
      if (basket.items.length > 0) {
        this.setBasket(basket);
      } else {
        this.deleteBasket(basket);
      }
    }
  }

  deleteBasket(basket: Basket) {
    return this.http.delete(this.baseUrl + 'basket?id=' + basket.id).subscribe({
      next: () => {
        this.basketSource.next(null);
        this.basketTotalSource.next(null);
        localStorage.removeItem('basket_id');
      },
      error: err => console.log(err)
    });
  }

  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    const basket = this.getCurrentBasketValue();

    if (basket) {
      // 1. Update local properties
      basket.shippingPrice = deliveryMethod.price;
      basket.deliveryMethodId = deliveryMethod.id;

      // 2. IMPORTANT: Save to API so Stripe knows the new total!
      this.setBasket(basket);
    }
  }

  private calculateTotal() {
    const basket = this.getCurrentBasketValue();
    if (!basket) return;

    // FIX: Use the actual shipping price, default to 0 if missing
    const shipping = basket.shippingPrice || 0;

    const subtotal = basket.items.reduce((sum, item) => (item.price * item.quantity) + sum, 0);
    const total = subtotal + shipping;

    this.basketTotalSource.next({ shipping, subtotal, total });
  }

  private createBasket(): Basket {
    const basket = new Basket();
    localStorage.setItem('basket_id', basket.id);
    return basket;
  }

  private addOrUpdateItem(items: BasketItem[], itemToAdd: BasketItem, quantity: number): BasketItem[] {
    const index = items.findIndex(i => i.id === itemToAdd.id);

    if (index === -1) {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    } else {
      items[index].quantity += quantity;
    }
    return items;
  }

  private mapProductToBasketItem(item: Product, quantity: number): BasketItem {
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      quantity,
      brand: item.brandName, // Ensure this matches your API DTO
      type: item.typeName // FIX: Was item.brandName
    };
  }

  clearLocalBasket() {
    this.basketSource.next(null);
    this.basketTotalSource.next(null);
    localStorage.removeItem('basket_id');
  }
}
