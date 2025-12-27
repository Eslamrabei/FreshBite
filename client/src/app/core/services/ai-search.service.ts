import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ProductSearchResponse } from '../../shared/models/product-search-response';
import { RagResponse } from '../../shared/models/rag-response';

@Injectable({
  providedIn: 'root',
})
export class AiSearchService {
  baseUrl = environment.apiUrl + 'search';
  private http = inject(HttpClient);

  search(querry: string, price?: number) {
    let params = new HttpParams().set('q', querry);
    if (price)
      params = params.set('price', price.toString());

    return this.http.get<RagResponse>(this.baseUrl + '/query', { params });

  }

}
