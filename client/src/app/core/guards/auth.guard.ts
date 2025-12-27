import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from "@angular/router";
import { AccountService } from "../../account/account.service";
import { map } from "rxjs";




export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  return accountService.currentUser$.pipe(
    map(auth => {
      if (auth)
        return true;
      router.navigate(['authentication/login'], { queryParams: { returnUrl: state.url } });
      return false;
    })
  )
}
