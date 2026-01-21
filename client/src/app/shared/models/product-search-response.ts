export interface ProductSearchResponse {
  id: number;
  name: string;
  description: string;
  price: number;
  score: number;
  pictureUrl: string;
}


export interface ChatMessage {
  type: 'user' | 'bot';
  text?: string;
  products?: ProductSearchResponse[];
}
