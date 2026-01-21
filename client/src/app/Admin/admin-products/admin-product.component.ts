import { Component, inject, OnInit, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from "@angular/router";
import Swal from 'sweetalert2';


import { Product } from '../../shared/models/pagination';
import { AdminService } from '../admin.service';
import { AdminListBase } from '../../core/Base/admin-list.base';
import { ShopService } from '../../shop/shop.service';


@Component({
  selector: 'app-admin-product.component',
  standalone: true,
  imports: [CurrencyPipe, RouterLink],
  templateUrl: './admin-product.component.html',
  styleUrl: './admin-product.component.scss',
})
export class AdminProductComponent extends AdminListBase<Product> implements OnInit {

  private adminService = inject(AdminService);
  private shopService = inject(ShopService);

  ngOnInit() {
    this.getData();
  }

  override getData(): void {
    this.shopService.getProducts(this.shopParams).subscribe({
      next: res => {
        this.data.set(res.data);
        this.shopParams.pageIndex = res.pageIndex;
        this.shopParams.pageSize = res.pageSize;
        this.totalCount.set(res.totalCount);
      }
    })
  }



  deleteProduct(productId: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
      if (result.isConfirmed) {
        this.adminService.deleteProduct(productId).subscribe({
          next: () => {
            this.data.update(current => current.filter(p => p.id !== productId));
            Swal.fire('Deleted!', 'Product has been deleted.', 'success');
            if (this.data().length === 0 && this.shopParams.pageIndex > 1)
              this.changePage(this.shopParams.pageIndex - 1);
          },
          error: (err) => {
            console.error(err);
            Swal.fire('Error!', 'Failed to delete product.', 'error');
          }
        });
      }
    });
  }


  changePage(newPage: number) {
    if (newPage < 1 || newPage > Math.ceil(this.totalCount() / this.shopParams.pageSize)) return;

    this.shopParams.pageIndex = newPage;
    this.getData();

  }

  get totalPages() {
    return Math.ceil(this.totalCount() / this.shopParams.pageSize);
  }

}
