import { Component, inject, OnInit, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from "@angular/router";
import Swal from 'sweetalert2';


import { Product } from '../../shared/models/pagination';
import { AdminService } from '../admin.service';


@Component({
  selector: 'app-admin-product.component',
  standalone: true,
  imports: [CurrencyPipe, RouterLink],
  templateUrl: './admin-product.component.html',
  styleUrl: './admin-product.component.scss',
})
export class AdminProductComponent implements OnInit {
  private adminService = inject(AdminService);

  products = signal<Product[]>([]);
  totalCount = signal<number>(0);
  pageSize = signal<number>(10);
  pageIndex = signal<number>(1);


  ngOnInit() {
    this.getAllProducts();
  }

  getAllProducts() {
    return this.adminService.getProducts(this.pageIndex(), this.pageSize()).subscribe({
      next: (data: any) => {
        this.products.set(data.data);
        this.pageIndex.set(data.pageIndex);
        this.pageSize.set(data.pageSize);
        this.totalCount.set(data.totalCount);
        console.log(data.data);

      },
      error: err => console.log(err)
    });
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
            this.products.update(current => current.filter(p => p.id !== productId));
            Swal.fire('Deleted!', 'Product has been deleted.', 'success');
            if (this.products().length === 0 && this.pageIndex() > 1)
              this.changePage(this.pageIndex() - 1);
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
    if (newPage < 1 || newPage > Math.ceil(this.totalCount() / this.pageSize())) return;

    this.pageIndex.set(newPage);
    this.getAllProducts();

  }

  get totalPages() {
    return Math.ceil(this.totalCount() / this.pageSize());
  }

}
