import { ApplicationConfig } from '@angular/core';
import { provideServerRendering } from '@angular/ssr';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { routes } from './app.routes';

export const config: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withFetch()),
    provideServerRendering()
  ]
};
