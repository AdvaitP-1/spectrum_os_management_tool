import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.page.html',
})
export class LoginPage {
  private router = inject(Router);
  private apiService = inject(ApiService);
  private userService = inject(UserService);

  userId: string = '';
  selectedEnvironment: string = 'QA';
  selectedDemoUserId: string = '';
  loading: boolean = false;
  error: string = '';

  demoUsers = [
    { id: 'P1234567', name: 'Patrick Dugan' },
    { id: 'P2345678', name: 'Kent Herbst' },
    { id: 'P3456789', name: 'Sion Pixley' },
    { id: 'P4567890', name: 'Boyuan Bruce Sun' },
    { id: 'P5678901', name: 'Maria Alvarez Anticona' },
    { id: 'P6789012', name: 'Joel Black' },
    { id: 'P7890123', name: 'Swetha Priya Yarlagadda' },
    { id: 'P8901234', name: 'Sheldon Skaggs' },
    { id: 'P9012345', name: 'Jason Routh' },
    { id: 'P0123456', name: 'Advait Pandey' }
  ];

  environments = ['QA', 'UAT', 'PROD', 'Staging'];

  onDemoUserChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const selectedUserId = selectElement.value;
    
    if (selectedUserId) {
      this.selectedDemoUserId = selectedUserId;
      this.userId = selectedUserId;
    } else {
      this.selectedDemoUserId = '';
      this.userId = '';
    }
  }

  async onLogin(): Promise<void> {
    if (!this.selectedDemoUserId) {
      this.error = 'Please select a demo user to continue.';
      return;
    }

    this.loading = true;
    this.error = '';

    try {
      // Use the selected demo user ID for login
      const user = await this.apiService.login(this.selectedDemoUserId).toPromise();
      
      if (user) {
        user.currentEnvironment = this.selectedEnvironment;
        this.userService.updateCurrentUser(user);
        this.router.navigate(['/dashboard']);
      } else {
        this.error = 'User not found. Please try selecting a different demo user.';
      }
    } catch (error) {
      console.error('Login error:', error);
      this.error = 'Login failed. Please try again.';
    } finally {
      this.loading = false;
    }
  }
} 