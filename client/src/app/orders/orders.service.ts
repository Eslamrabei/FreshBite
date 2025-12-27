import { inject, Injectable } from '@angular/core';
import { Order } from '../shared/models/order';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);


  getOrdersForUser() {
    return this.http.get<Order[]>(this.baseUrl + 'orders');
  }

  getOrderDetailed(id: string) {
    return this.http.get<Order>(this.baseUrl + 'orders/' + id);
  }

}
