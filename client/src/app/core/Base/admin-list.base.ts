import { Directive, signal } from "@angular/core";
import { ShopParams } from "../../shared/models/shop-params";


@Directive()

export abstract class AdminListBase<T> {

  data = signal<T[]>([]);
  totalCount = signal(0);
  shopParams = new ShopParams();

  onPageChanged(event: any) {
    if (this.shopParams.pageIndex! == event) {
      this.shopParams.pageIndex = event;
      this.getData();
    }
  }

  onSearch(searchTerm: string) {
    this.shopParams.search = searchTerm;
    this.shopParams.pageIndex = 1;
    this.getData();
  }

  onReset() {
    this.shopParams = new ShopParams();
    this.getData();
  }


  abstract getData(): void;

}
