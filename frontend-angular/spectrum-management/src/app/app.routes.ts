import { Routes } from '@angular/router';
import { LoginPage } from './pages/login.page';
import { DashboardPage } from './pages/dashboard.page';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginPage },
  { path: 'dashboard', component: DashboardPage },
  { path: '**', redirectTo: '/login' }
];
