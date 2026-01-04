import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShopService } from '../shop.service';
import { ShopParams } from '../../shared/models/shop-params';
import { Brand, Product, ProductType } from '../../shared/models/pagination';
import { ProductItemComponent } from '../products-item/product-item.component';
import { Pager } from "../../shared/components/pager/pager";
import { SearchBarComponent } from "../Search-bar/search.component";


@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [CommonModule, ProductItemComponent, Pager, SearchBarComponent],
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})

export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);

  products = signal<Product[]>([]);
  brands = signal<Brand[]>([]);
  types = signal<ProductType[]>([]);
  totalCount = signal(0);

  shopParams = new ShopParams();

  sortOptions = [
    { name: 'Name', value: 1 },
    { name: 'NameDesc', value: 2 },
    { name: 'PriceAsc', value: 3 },
    { name: 'PriceDesc', value: 4 }
  ];

  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getTypes();
  }

  // Search Bar
  onSearchChange(searchTerm: string) {
    this.shopParams.search = searchTerm;
    this.shopParams.pageIndex = 1;
    this.getSpecificProducts();
  }
  onResetSearch() {
    this.shopParams.search = '';
    this.shopParams.pageIndex = 1;
    this.getProducts();
  }
  getSpecificProducts() {
    this.shopService.getProducts(this.shopParams).subscribe({
      next: res => {
        this.products.set(res.data);
      }
    });
  }

//////

  getProducts() {
    this.shopService.getProducts(this.shopParams).subscribe({
      next: (response) => {
        console.log(response);
        this.products.set( response.data);
        this.totalCount.set(response.totalCount);
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
