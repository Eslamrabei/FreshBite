import { Component, computed, ElementRef, HostListener, inject, signal } from '@angular/core';
import { BasketService } from '../../basket/basket.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink, RouterModule } from "@angular/router";
import { AccountService } from '../../account/account.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-nav-bar',
  imports: [RouterLink, CommonModule, RouterModule],
  templateUrl: './nav-bar.html',
  styleUrl: './nav-bar.scss',
})


export class NavBar {

  private basketService = inject(BasketService);
  private accountService = inject(AccountService);
  private eRef = inject(ElementRef);

  basket = toSignal(this.basketService.basket$);
  currentUser = toSignal(this.accountService.currentUser$);


  showMenu = signal(false);

  count = computed(() => {
    const currentBasket = this.basket();
    return currentBasket?.items.reduce((sum, item) => sum + item.quantity, 0) ?? 0;
  });


  toggleMenu() {
    this.showMenu.update(value => !value);
  }

  logout() {
    this.basketService.clearLocalBasket();
    this.accountService.logout();
    this.showMenu.set(false);
  }

  @HostListener('document:click', ['$event'])
  clickout(event: Event) {
    // If the click is NOT inside this component, close the menu
    if (!this.eRef.nativeElement.contains(event.target)) {
      this.showMenu.set(false);
    }
  }
}
