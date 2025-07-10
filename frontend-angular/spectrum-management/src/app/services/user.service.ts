import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';
import { User } from '../models/user.model';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private apiService: ApiService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    // Try to restore user from localStorage (only in browser)
    if (isPlatformBrowser(this.platformId)) {
      const savedUser = localStorage.getItem('currentUser');
      if (savedUser) {
        this.currentUserSubject.next(JSON.parse(savedUser));
      }
    }
  }

  get currentUser(): User | null {
    return this.currentUserSubject.value;
  }

  login(userId: string): Observable<User> {
    console.log('UserService.login called with userId:', userId);
    return this.apiService.login(userId).pipe(
      tap(user => {
        console.log('UserService.login - API response:', user);
        this.currentUserSubject.next(user);
        if (isPlatformBrowser(this.platformId)) {
          localStorage.setItem('currentUser', JSON.stringify(user));
          console.log('UserService.login - User saved to localStorage');
        }
      })
    );
  }

  logout(): void {
    this.currentUserSubject.next(null);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('currentUser');
    }
  }

  updateCurrentUser(user: User): void {
    this.currentUserSubject.next(user);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('currentUser', JSON.stringify(user));
    }
  }

  updateUser(user: User): Observable<void> {
    return this.apiService.updateUser(user.id, user).pipe(
      tap(() => {
        // If updating the current user, update the local state too
        if (this.currentUser && this.currentUser.id === user.id) {
          this.updateCurrentUser(user);
        }
      })
    );
  }

  switchEnvironment(environment: string): Observable<User> {
    const currentUser = this.currentUser;
    if (!currentUser) {
      throw new Error('No user logged in');
    }

    // Update user environment and return the updated user
    return this.apiService.updateUser(currentUser.id, {
      ...currentUser,
      currentEnvironment: environment
    }).pipe(
      switchMap(() => this.apiService.getUser(currentUser.id)),
      tap(updatedUser => {
        this.updateCurrentUser(updatedUser);
      })
    );
  }

  isLoggedIn(): boolean {
    return this.currentUser !== null;
  }

  getCurrentEnvironment(): string {
    return this.currentUser?.currentEnvironment || 'QA';
  }
} 