import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

import { CreateProductDto, UpdateProductDto } from "../shared/models/create-product-dto";
import { environment } from "../../environments/environment";
import { Product } from "../shared/models/pagination";
import { ShopService } from "../shop/shop.service";


@Injectable({ providedIn: 'root' })
export class AdminService {
  private shopService = inject(ShopService);
  private _http = inject(HttpClient);
  baseUrl = environment.apiUrl;


  getProducts(pageIndex: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageIndex', pageIndex);
    params = params.append('pageSize', pageSize);
    return this._http.get<Product[]>(this.baseUrl + 'products', { params });
  }

  createProduct(product: CreateProductDto) {
    return this._http.post<any>(this.baseUrl + 'Admin', product);
  }

  updateProduct(id: number, product: any, file: File | null) {
    const formData = new FormData();

    formData.append('id', id.toString());
    formData.append('name', product.name);
    formData.append('description', product.description);
    formData.append('price', product.price.toString());
    formData.append('prictureUrl', product.pictureUrl);
    formData.append('brandId', product.brandId.toString());
    formData.append('typeId', product.brandId.toString());


    if (file)
      formData.append('imageFile', file);

    return this._http.put<any>(this.baseUrl + 'Admin/' + id , formData)
  }

  deleteProduct(id: number) {
    return this._http.delete<any>(`${this.baseUrl}Admin/${id}`);
  }

  getBrands() {
    return this.shopService.getBrands();
  }
  getTypes() {
    return this.shopService.getTypes();
  }

  getProductById(id: number) {
    return this._http.get<Product>(this.baseUrl + 'products/' + id);
  }


  uploadImage(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this._http.post<{ filePath: string }>(`${this.baseUrl}admin/upload-image`, formData);
  }


}
