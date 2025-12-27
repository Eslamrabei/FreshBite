export interface ProductSearchResponse {
  id: number;
  name: string;
  description: string;
  price: number;
  score: number;
}


export interface ChatMessage {
  type: 'user' | 'bot';
  text?: string;
  products?: ProductSearchResponse[];
}
