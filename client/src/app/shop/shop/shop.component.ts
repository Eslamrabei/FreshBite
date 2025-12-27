import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShopService } from '../shop.service';
import { ShopParams } from '../../shared/models/shop-params';
import { Brand, Product, ProductType } from '../../shared/models/pagination';
import { ProductItemComponent } from '../products-item/product-item.component';
import { Pager } from "../../shared/components/pager/pager";


@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [CommonModule, ProductItemComponent, Pager], // Add imports
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService); // Modern inject style

  products: Product[] = [];
  brands = signal<Brand[]>([]);
  types = signal<ProductType[]>([]);
  totalCount = signal(0);

  shopParams = new ShopParams();

  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low to High', value: 'priceAsc' },
    { name: 'Price: High to Low', value: 'priceDesc' }
  ];

  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getTypes();
  }

  getProducts() {
  this.shopService.getProducts(this.shopParams).subscribe({
    next: (response: any) => {
      if (response.Data) {
        this.products = response.Data;
      } else if (response.data) {
        this.products = response.data;
      }

      if (response.TotalCount !== undefined) {
        this.totalCount.set(response.TotalCount);
      } else if (response.totalCount !== undefined) {
        this.totalCount.set(response.totalCount);
      } else if (response.count !== undefined) {
        this.totalCount.set(response.count);
      }

      console.log('Total Count set to:', this.totalCount());
    },
    error: error => console.log(error)
  });
}

  getBrands() {
    this.shopService.getBrands().subscribe({
      next: (response) => {
        this.brands.set([{ id: 0, name: 'All' }, ...response]);
      },
      error: error => console.log(error)
    });
  }

  getTypes() {
    this.shopService.getTypes().subscribe({
      next: (response) => {
        this.types.set([{ id: 0, name: 'All' }, ...response]);
      },
      error: error => console.log(error)
    });
  }

  onBrandSelected(brandId: number) {
    this.shopParams.brandId = brandId;
    this.shopParams.pageIndex = 1;
    this.getProducts();
  }

  onTypeSelected(typeId: number) {
    this.shopParams.typeId = typeId;
    this.shopParams.pageIndex = 1;
    this.getProducts();
  }

  onSortSelected(event: any) {
    this.shopParams.sort = event.target.value;
    this.getProducts();
  }

  onPageChanged(event: any) {
    if (this.shopParams.pageIndex !== event) {
      this.shopParams.pageIndex = event;
      this.getProducts();
    }
  }
}
