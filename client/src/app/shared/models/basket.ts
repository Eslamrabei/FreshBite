import { v4 as uuidv4 } from "uuid";

export interface BasketItem {
  id: number;
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand?: string;
  type?: string;
}

export interface Basket {
  id: string;
  items: BasketItem[];
  clientSecret?: string;
  paymentIndentId?: string;
  deliveryMethodId?: number;
  shippingPrice?: number;
}

export class Basket implements Basket {
  id = uuidv4().toString();
  items: BasketItem[] = [];
}

export interface BasketTotals {
  shipping: number;
  subtotal: number;
  total: number;
}


