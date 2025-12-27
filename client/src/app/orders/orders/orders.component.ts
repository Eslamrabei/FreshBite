import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersService } from '../orders.service';
import { Order } from '../../shared/models/order';
import { RouterLink } from '@angular/router';


@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  private ordersService = inject(OrdersService);
  orders = signal<Order[]>([]);
  loading = signal<boolean>(false);

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders() {
    this.loading.set(true);

    this.ordersService.getOrdersForUser().subscribe({
      next: orders => {
        this.orders.update(() => [...orders]);
        this.loading.set(false);
      },
      error: err => {
        console.log(err);
        this.loading.set(false);
      }
    });
  }
}
