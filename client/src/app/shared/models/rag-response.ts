import { ProductSearchResponse } from "./product-search-response";


export interface RagResponse {
  aiAnswer: string;
  products: ProductSearchResponse[];
}
