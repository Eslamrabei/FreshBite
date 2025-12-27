export class ShopParams {
  brandId = 0;
  typeId = 0;
  sort: 'NameAsc' | 'NameDesc' | 'PriceAsc' | 'PriceDesc' = 'NameAsc';
  pageIndex = 1;
  pageSize = 6;
  search = '';
}
