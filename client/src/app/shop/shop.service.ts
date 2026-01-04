import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs'; // Import map
import { ShopParams } from '../shared/models/shop-params';
import {  Product } from '../shared/models/pagination';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getProducts(shopParams: ShopParams) {
    let params = new HttpParams();

    if (shopParams.brandId > 0) params = params.append('brandId', shopParams.brandId);
    if (shopParams.typeId > 0) params = params.append('typeId', shopParams.typeId);

    params = params.append('sortingOptions', shopParams.sort);
    params = params.append('pageIndex', shopParams.pageIndex);
    params = params.append('pageSize', shopParams.pageSize);
    if (shopParams.search) params = params.append('search', shopParams.search);

    return this.http.get<any>(this.baseUrl + 'products', { params }).pipe(
      map(response => {
        const root = response.Value || response;

        if (root.Data) {
            root.Data = root.Data.map((item: any) => this.normalizeProduct(item));
        }
        return root;
      })
    );
  }

  getProductById(id: number) {
    return this.http.get<any>(this.baseUrl + 'products/' + id).pipe(
      map(response => {
        const data = response.Value || response;

        return this.normalizeProduct(data);
      })
    );
  }

  getBrands() {
    return this.http.get<any>(this.baseUrl + 'products/brands').pipe(
      map(response => {
        const list = response.Value || response;
        return list.map((item: any) => ({ id: item.Id || item.id, name: item.Name || item.name }));
      })
    );
  }

  getTypes() {
    return this.http.get<any>(this.baseUrl + 'products/types').pipe(
      map(response => {
        const list = response.Value || response;
        return list.map((item: any) => ({ id: item.Id || item.id, name: item.Name || item.name }));
      })
    );
  }


  private normalizeProduct(item: any): Product {
    return {
        id: item.Id || item.id,
        name: item.Name || item.name,
        description: item.Description || item.description,
        price: item.Price || item.price,
        pictureUrl: item.PictureUrl || item.pictureUrl,
        typeName: item.ProductType || item.productType,
        brandName: item.ProductBrand || item.productBrand
    };
  }
}
