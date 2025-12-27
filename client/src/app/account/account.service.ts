import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { catchError, map, of, ReplaySubject } from 'rxjs';
import { Address, User } from '../shared/models/user';
import { BasketService } from '../basket/basket.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private basketService = inject(BasketService);
  baseUrl = environment.apiUrl;

  private currentUserSource = new ReplaySubject<User | null>(1);
  currentUser$ = this.currentUserSource.asObservable();

  loadCurrentUser(token: string | null) {
    if (token === null) {
      this.currentUserSource.next(null);
      return of(null);
    }

    return this.http.get<User>(this.baseUrl + 'Authentication').pipe(
      map(user => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
          return user;
        }
        return null;
      }),
      catchError(err => {
        // THE FIX: If the token is invalid/expired, the API returns an error.
        // We must clear the local storage so we don't get stuck in this loop.
        console.error('Token is invalid or expired', err);
        localStorage.removeItem('token');
        this.currentUserSource.next(null);
        return of(null);
      })
    );
  }

  getUserAddress() {
    return this.http.get<Address>(this.baseUrl + 'Authentication/address');
  }

  login(values: any) {
    return this.http.post<User>(this.baseUrl + 'Authentication/Login', values).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
        return user;
      })
    );
  }

  register(values: any) {
    return this.http.post<User>(this.baseUrl + 'Authentication/Register', values).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
        return user;
      })
    );
  }



  //Logout
  logout() {
    localStorage.removeItem('token');
    this.basketService.clearLocalBasket();
    this.currentUserSource.next(null);
    this.router.navigateByUrl('/');
  }


}
