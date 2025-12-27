export interface Product {
  id: number;
  name: string;
  description: string;
  pictureUrl: string;
  price: number;
  brandName: string;
  typeName: string;
}

export interface Pagination<T> {
  pageSize: number;
  pageIndex: number;
  totalCount: number;
  data: T[];
}

export interface Brand {
  id: number;   // Capital I
  name: string; // Capital N
}

export interface ProductType {
  id: number;   // Capital I
  name: string; // Capital N
}
