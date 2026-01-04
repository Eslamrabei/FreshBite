import { Component, inject, output, signal } from "@angular/core";
import { FormsModule, NgModel } from "@angular/forms";


import { ShopService } from "../shop.service";
import { ShopParams } from "../../shared/models/shop-params";
import { Product } from "../../shared/models/pagination";



@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})

export class SearchBarComponent {

  search = signal('');
  searchedClicked = output<string>();
  resetClicked = output<void>();

  onSearch() {
    this.searchedClicked.emit(this.search());
  }

  onReset() {
    this.search.set('');
    this.resetClicked.emit();
  }

}
