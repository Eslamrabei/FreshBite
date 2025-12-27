import { Routes } from '@angular/router';
import { ShopComponent } from './shop/shop/shop.component';
import { ProductDetails } from './shop/product-details/product-details';
import { BasketComponent } from './basket/basket.component';
import { Login } from './account/login/login';
import { RegisterComponent } from './account/register/register';
import { CheckoutComponent } from './shared/checkouts/checkout.component';
import { CheckoutSuccessComponent } from './shared/checkouts/checkout-success/checkout-success.component';
import { OrdersComponent } from './orders/orders/orders.component';
import { OrderDetailedComponent } from './orders/order-detailed/order-detailed.component';
import { NotFoundComponent } from './core/server-error/not-found/not-found.component';
import { ServerErrorComponent } from './core/server-error/server-error.component';
import { authGuard } from './core/guards/auth.guard';
import { HomeComponent } from './shared/components/home/home.component';
import { ContactComponent } from '../contact/contact/contact.component';
import { SearchComponent } from './shop/ai-shop-search/search.component';



export const routes: Routes = [
  { path: '', component: HomeComponent, title: 'Home' },
  { path: 'contact', component: ContactComponent, title: 'Contact' },
  { path: 'shop', component: ShopComponent, title: 'Shop' },
  { path: 'shop/:id', component: ProductDetails, title: 'Product Details' },
  { path: 'basket', component: BasketComponent, canActivate: [authGuard], title: 'Basket' },
  { path: 'login', component: Login, title: 'Login' },
  { path: 'register', component: RegisterComponent, title: 'Register' },
  { path: 'checkout', component: CheckoutComponent, canActivate: [authGuard], title: 'Checkout' },
  { path: 'checkout/success', component: CheckoutSuccessComponent, canActivate: [authGuard], title: 'Checkout Success' },
  { path: 'orders', component: OrdersComponent, canActivate: [authGuard], title: 'Orders' },
  { path: 'orders/:id', component: OrderDetailedComponent, canActivate: [authGuard], title: 'Order' },
  { path: 'not-found', component: NotFoundComponent, title: 'Not Found' },
  { path: 'server-error', component: ServerErrorComponent, title: 'Server Error' },
  { path: '**', redirectTo: '', pathMatch: 'full' }

];
