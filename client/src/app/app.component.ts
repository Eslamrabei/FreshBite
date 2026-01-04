import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopComponent } from './shop/shop/shop.component';
import { RouterOutlet } from '@angular/router';
import { NavBar } from './core/nav-bar/nav-bar';
import { Footer } from './core/footer/footer';
import { AccountService } from './account/account.service';
import { BasketService } from './basket/basket.service';
import { SearchComponent } from "./shop/ai-shop-search/search.component";


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavBar, Footer, SearchComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('FreshBite');

  private accountService = inject(AccountService);
  private basketService = inject(BasketService);


  ngOnInit(): void {
    this.loadCurrentUser();
    this.loadCurrentBasket();
  }

  loadCurrentUser() {
    const token = localStorage.getItem('token');
    this.accountService.loadCurrentUser(token).subscribe();
  }

  loadCurrentBasket() {
    const basketId = localStorage.getItem('basket_id');
    if (basketId)
      this.basketService.getBasketById(basketId).subscribe();
  }

}
