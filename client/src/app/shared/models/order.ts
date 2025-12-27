import { Address } from "./user";

export interface Order {
    id: string;
    userEmail: string;
    shippingAddress: Address;
    orderItems: OrderItem[];
    paymentStatus: string;
    deliveryMethod: string;
    deliveryMethodId: number;
    subTotal: number;
    total: number;
    orderDate: string;
    paymentIntentId: string;
}

export interface OrderItem {
    productId: number;
    productName: string;
    pictureUrl: string;
    price: number;
    quantity: number;
}
