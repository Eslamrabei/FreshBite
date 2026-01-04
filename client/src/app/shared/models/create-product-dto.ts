

export interface CreateProductDto {
  name: string,
  description: string,
  pictureUrl?: string,
  price: number,
  brandId: number,
  typeId: number
}


export interface UpdateProductDto {
  id: number,
  name: string,
  description: string,
  pictureUrl?: string,
  price: number,
  brandId: number,
  typeId: number,
}



