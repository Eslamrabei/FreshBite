import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { BasketService } from "../../basket/basket.service";
import { environment } from "../../../environments/environment";
import { map } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  private http = inject(HttpClient);
  private basketService = inject(BasketService);
  baseUrl = environment.apiUrl;

  createPaymentIntent() {
    const currentBasket = this.basketService.getCurrentBasketValue();
    if (!currentBasket) return;

    // Matches your API: /api/Payments/{basketId}
    return this.http.post<any>(this.baseUrl + 'Payments/' + currentBasket.id, {})
      .pipe(
        map(basket => {
          this.basketService.setBasket(basket);
          return basket;
        })
      );
  }

  createOrder(order: any) {
    return this.http.post(this.baseUrl + 'Orders', order);
  }

}
