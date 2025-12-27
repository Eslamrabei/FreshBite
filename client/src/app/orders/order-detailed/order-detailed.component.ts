import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrdersService } from '../orders.service';
import { CommonModule } from '@angular/common';
import { Order } from '../../shared/models/order';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-order-detailed',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order-detailed.component.html',
  styleUrls: ['./order-detailed.component.scss']
})
export class OrderDetailedComponent implements OnInit {
  baseUrl = environment.imgUrl;
  private ordersService = inject(OrdersService);
  private route = inject(ActivatedRoute);

  // Signal definition is correct
  order = signal<Order | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.ordersService.getOrderDetailed(id).subscribe({
        next: (order) => {
          // Use .set() when replacing the data completely
          this.order.set(order);
        },
        error: err => console.log(err)
      });
    }
  }

  getOrderStatusClass(status: string): string {
  if (!status) return '';
  const normalizedStatus = status.trim().toLowerCase();
  if (normalizedStatus === 'pending') {
    return 'bg-warning';
  }
  else if (normalizedStatus === 'paymentreceived' || normalizedStatus === 'paymentrecieved') {
    return 'bg-success';
  }
  else if (normalizedStatus === 'paymentfailed') {
    return 'bg-danger';
  }
  return 'bg-secondary';
  }
}
