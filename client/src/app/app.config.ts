import { ApplicationConfig, Injectable, provideBrowserGlobalErrorListeners } from '@angular/core';
import { APP_INITIALIZER } from '@angular/core';
import { provideRouter, RouterStateSnapshot, TitleStrategy } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { routes } from './app.routes';
import { provideNgxStripe } from 'ngx-stripe';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { loadingInterceptor } from './core/interceptors/loading.interceptor';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { environment } from '../environments/environment';
import { Title } from '@angular/platform-browser';

@Injectable({ providedIn: 'root' })
export class TemplatePageTitleStrategy extends TitleStrategy {
  constructor(private readonly title: Title) {
    super();
  }

  override updateTitle(routerState: RouterStateSnapshot) {
    const title = this.buildTitle(routerState);
    if (title !== undefined) {
      this.title.setTitle(`FreshBite | ${title}`);
    } else {
      this.title.setTitle('FreshBite');
    }
  }
}

export const appConfig: ApplicationConfig = {
  providers: [
    { provide: TitleStrategy, useClass: TemplatePageTitleStrategy },
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    provideAnimations(),
    provideNgxStripe(environment.apiKey),
    provideHttpClient(
      withInterceptors([loadingInterceptor, jwtInterceptor, errorInterceptor])),
    provideToastr({
      positionClass: 'toast-bottom-right', // Places it nicely in the corner
      preventDuplicates: true,             // Prevents spamming the user
      timeOut: 3000,                       // Disappears after 3 seconds
      progressBar: true,                   // Shows a little timer bar
      closeButton: true,                   // Adds a small 'X'
    })
  ]
};
