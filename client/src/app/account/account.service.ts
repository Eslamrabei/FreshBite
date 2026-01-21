import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode, JwtPayload, JwtDecodeOptions } from 'jwt-decode';



import { environment } from '../../environments/environment';
import { catchError, map, of, ReplaySubject } from 'rxjs';
import { Address, User } from '../shared/models/user';
import { BasketService } from '../basket/basket.service';
import { stringify } from 'uuid';

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
          localStorage.setItem('token', user.accessToken);

          this.currentUserSource.next(user);
          return user;
        }
        return null;
      }),
      catchError(err => {

        console.error('Token is invalid or expired', err);
        localStorage.removeItem('token');
        this.currentUserSource.next(null);
        return of(null);
      })
    );
  }

  getUserAddress() {
    return this.http.get<Address>(this.baseUrl + 'Authentication/Address');
  }

  login(values: any) {
    return this.http.post<User>(this.baseUrl + 'Authentication/Login', values).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('token', user.accessToken);
          let role = this.getRoleFromToken(user.accessToken);
          // adding role to a user
          role = user.role;
          console.log(role);
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
          localStorage.setItem('token', user.accessToken);
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

  getRoleFromToken(token: string): string | string[] {
    if (!token) return [];
    const decode: any = jwtDecode(token);
    const role = decode['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    return role;
  }


  isAdmin(token: string): boolean {
    const roles = this.getRoleFromToken(token);

    if (Array.isArray(roles)) {
      return roles.includes('Admin') || roles.includes('SuperAdmin');
    }
    return roles === 'Admin' || roles === 'SuperAdmin';
  }


}
