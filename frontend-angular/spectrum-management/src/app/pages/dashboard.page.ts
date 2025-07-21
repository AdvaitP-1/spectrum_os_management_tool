import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, forkJoin } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { User } from '../models/user.model';
import { Group, CreateGroup } from '../models/group.model';
import { Permission, CreatePermission } from '../models/permission.model';
import { UserService } from '../services/user.service';
import { ApiService } from '../services/api.service';

interface DashboardState {
  currentUser: User | null;
  userGroups: Group[];
  availableGroups: Group[];
  users: User[];
  allPermissions: Permission[];
  permissionCategories: string[];
  currentEnvironment: string;
  activeTab: string;
}

interface ModalState {
  showModal: boolean;
  showDeleteConfirmation: boolean;
  selectedGroup: Group | null;
  selectedUser: User | null;
  groupToDelete: Group | null;
  selectedGroupMembers: User[];
}

interface LoadingState {
  actionLoading: boolean;
  createLoading: boolean;
  dataLoading: boolean;
}

interface MessageState {
  successMessage: string;
  errorMessage: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.scss'
})
export class DashboardPage implements OnInit, OnDestroy {
    private readonly destroy$ = new Subject<void>();

  public dashboardState: DashboardState = {
    currentUser: null,
    userGroups: [],
    availableGroups: [],
    users: [],
    allPermissions: [],
    permissionCategories: [],
    currentEnvironment: 'QA',
    activeTab: 'my-groups'
  };

  public modalState: ModalState = {
    showModal: false,
    showDeleteConfirmation: false,
    selectedGroup: null,
    selectedUser: null,
    groupToDelete: null,
    selectedGroupMembers: []
  };

  public loadingState: LoadingState = {
    actionLoading: false,
    createLoading: false,
    dataLoading: false
  };

  public messageState: MessageState = {
    successMessage: '',
    errorMessage: ''
  };

  public newGroup: CreateGroup = this.createEmptyGroup();
  public newPermission: CreatePermission = this.createEmptyPermission();
  public customCategory: string = '';

  constructor(
    private readonly router: Router,
    private readonly userService: UserService,
    private readonly apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.initializeComponent();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get currentUser(): User | null {
    return this.dashboardState.currentUser;
  }

  get userGroups(): Group[] {
    return this.dashboardState.userGroups;
  }

  get availableGroups(): Group[] {
    return this.dashboardState.availableGroups;
  }

  get users(): User[] {
    return this.dashboardState.users;
  }

  get allPermissions(): Permission[] {
    return this.dashboardState.allPermissions;
  }

  get permissionCategories(): string[] {
    return this.dashboardState.permissionCategories;
  }

  get currentEnvironment(): string {
    return this.dashboardState.currentEnvironment;
  }

  set currentEnvironment(value: string) {
    this.dashboardState.currentEnvironment = value;
  }

  get activeTab(): string {
    return this.dashboardState.activeTab;
  }

  get selectedGroup(): Group | null {
    return this.modalState.selectedGroup;
  }

  get selectedUser(): User | null {
    return this.modalState.selectedUser;
  }

  get showModal(): boolean {
    return this.modalState.showModal;
  }

  get showDeleteConfirmation(): boolean {
    return this.modalState.showDeleteConfirmation;
  }

  get selectedGroupMembers(): User[] {
    return this.modalState.selectedGroupMembers;
  }

  get groupToDelete(): Group | null {
    return this.modalState.groupToDelete;
  }

  get actionLoading(): boolean {
    return this.loadingState.actionLoading;
  }

  get createLoading(): boolean {
    return this.loadingState.createLoading;
  }

  get dataLoading(): boolean {
    return this.loadingState.dataLoading;
  }

  get successMessage(): string {
    return this.messageState.successMessage;
  }

  get errorMessage(): string {
    return this.messageState.errorMessage;
  }

  private initializeComponent(): void {
    this.subscribeToUserChanges();
    this.loadInitialData();
  }

  private subscribeToUserChanges(): void {
    this.userService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (user) => {
          this.dashboardState.currentUser = user;
          if (user) {
            this.dashboardState.currentEnvironment = user.currentEnvironment || 'QA';
            this.loadDashboardData();
          } else {
            this.handleUserLogout();
          }
        },
        error: (error) => {
          console.error('Error in user subscription:', error);
          this.showErrorMessage('Failed to load user data');
        }
      });
  }

  private loadInitialData(): void {
    if (!this.dashboardState.currentUser) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadDashboardData();
  }

  private loadDashboardData(): void {
    if (!this.dashboardState.currentUser) return;

    const currentEnv = this.dashboardState.currentEnvironment;
    this.loadingState.dataLoading = true;

    const requests = forkJoin([
      this.apiService.getAllGroupsByUser(this.dashboardState.currentUser.id),
      this.apiService.getAllGroups(currentEnv), 
      this.apiService.getAllUsers(),
      this.apiService.getPermissions(),
      this.apiService.getPermissionCategories()
    ]);

    requests.pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (results) => {
          this.handleDataLoadSuccess(results);
        },
        error: (error) => {
          this.handleDataLoadError(error);
        },
        complete: () => {
          this.loadingState.dataLoading = false;
        }
      });
  }

  private handleDataLoadSuccess(
    results: [Group[], Group[], User[], Permission[], string[]]
  ): void {
    const [userGroups, allGroups, users, permissions, categories] = results;
    
    this.dashboardState.userGroups = userGroups;
    this.dashboardState.users = users;
    this.dashboardState.allPermissions = permissions;
    this.dashboardState.permissionCategories = categories;
    
    this.updateAvailableGroups(allGroups, userGroups);
  }

  private handleDataLoadError(error: any): void {
    console.error('Error loading dashboard data:', error);
    this.showErrorMessage('Failed to load dashboard data');
    this.loadingState.dataLoading = false;
  }

  private updateAvailableGroups(allGroups: Group[], userGroups: Group[]): void {
    const userGroupIds = new Set(userGroups.map(group => group.id));
    this.dashboardState.availableGroups = allGroups.filter(group => 
      !userGroupIds.has(group.id)
    );
  }

  public setActiveTab(tab: string): void {
    this.dashboardState.activeTab = tab;
  }

  public onEnvironmentChange(newEnvironment: string): void {
    this.dashboardState.currentEnvironment = newEnvironment;
    this.switchEnvironment();
  }

  public switchEnvironment(): void {
    if (!this.dashboardState.currentUser) return;

    const newEnvironment = this.dashboardState.currentEnvironment;

    const updatedUser: User = {
      ...this.dashboardState.currentUser,
      currentEnvironment: newEnvironment
    };

    this.userService.updateCurrentUser(updatedUser);
    this.newGroup.environment = newEnvironment;
    
    this.loadDashboardData();
  }

  public joinGroup(groupId: number): void {
    if (!this.validateUserAndLoading()) return;

    this.performGroupAction(
      () => this.apiService.joinGroup(this.dashboardState.currentUser!.id, groupId),
      'Successfully joined group',
      'Failed to join group',
      () => this.handleGroupActionSuccess(groupId)
    );
  }

  public leaveGroup(groupId: number): void {
    if (!this.validateUserAndLoading()) return;

    if (!this.confirmAction('Are you sure you want to leave this group?')) return;

    this.performGroupAction(
      () => this.apiService.leaveGroup(this.dashboardState.currentUser!.id, groupId),
      'Successfully left group',
      'Failed to leave group',
      () => this.handleGroupActionSuccess(groupId)
    );
  }

  public deleteGroup(group: Group): void {
    this.modalState.groupToDelete = group;
    this.modalState.showDeleteConfirmation = true;
  }

  public confirmDeleteGroup(): void {
    if (!this.modalState.groupToDelete || this.loadingState.actionLoading) return;

    const groupId = this.modalState.groupToDelete.id;
    
    this.performGroupAction(
      () => this.apiService.deleteGroup(groupId),
      'Group deleted successfully',
      'Failed to delete group',
      () => {
        this.closeDeleteConfirmation();
        if (this.modalState.selectedGroup?.id === groupId) {
          this.closeModal();
        }
      }
    );
  }

  public createGroup(): void {
    if (this.loadingState.createLoading || !this.validateGroupForm()) return;

    this.loadingState.createLoading = true;
    
    this.apiService.createGroup(this.newGroup)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.handleCreateSuccess('Group created successfully');
          this.resetGroupForm();
        },
        error: (error) => {
          this.handleCreateError('Failed to create group', error);
        },
        complete: () => {
          this.loadingState.createLoading = false;
        }
      });
  }

  public createPermission(): void {
    if (this.loadingState.createLoading || !this.validatePermissionForm()) return;

    const permissionToCreate = this.preparePermissionForCreation();
    this.loadingState.createLoading = true;

    this.apiService.createPermission(permissionToCreate)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.handleCreateSuccess('Permission created successfully');
          this.resetPermissionForm();
        },
        error: (error) => {
          this.handleCreateError('Failed to create permission', error);
        },
        complete: () => {
          this.loadingState.createLoading = false;
        }
      });
  }

  // Modal management methods
  public selectGroup(group: Group): void {
    this.modalState.selectedGroup = group;
    this.modalState.selectedUser = null;
    this.loadGroupMembers(group.id);
    this.modalState.showModal = true;
  }

  public selectUser(user: User): void {
    this.modalState.selectedUser = user;
    this.modalState.selectedGroup = null;
    this.modalState.showModal = true;
  }

  public closeModal(): void {
    this.resetModalState();
  }

  public closeDeleteConfirmation(): void {
    this.modalState.showDeleteConfirmation = false;
    this.modalState.groupToDelete = null;
  }

  public isUserInGroup(groupId: number): boolean {
    return this.dashboardState.userGroups.some(group => group.id === groupId);
  }

  public getUserPermissions(user: User): Permission[] {
    if (!user.groups) return [];

    const permissions: Permission[] = [];
    const permissionIds = new Set<number>();

    user.groups.forEach(userGroup => {
      const fullGroup = this.findGroupById(userGroup.id);
      if (fullGroup?.permissions) {
        fullGroup.permissions.forEach(permission => {
          if (!permissionIds.has(permission.id)) {
            permissionIds.add(permission.id);
            permissions.push(permission);
          }
        });
      }
    });

    return permissions;
  }

  public getGroupPermissions(group: Group): Permission[] {
    return group.permissions || [];
  }

  public getPermissionCategories(permissions: Permission[]): string[] {
    const categories = new Set(permissions.map(p => p.category));
    return Array.from(categories);
  }

  public getPermissionsByCategory(permissions: Permission[], category: string): Permission[] {
    return permissions.filter(p => p.category === category);
  }

  public formatUserId(userId: string): string {
    const numericPart = userId.replace(/[^0-9]/g, '');
    return `P${numericPart.padStart(7, '0')}`;
  }

  public togglePermission(permissionId: number, event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    const checked = checkbox.checked;
    
    if (checked) {
      if (!this.newGroup.permissionIds.includes(permissionId)) {
        this.newGroup.permissionIds.push(permissionId);
      }
    } else {
      this.newGroup.permissionIds = this.newGroup.permissionIds.filter(id => id !== permissionId);
    }
  }

  public logout(): void {
    this.userService.logout();
    this.router.navigate(['/login']);
  }

  private validateUserAndLoading(): boolean {
    return !!this.dashboardState.currentUser && !this.loadingState.actionLoading;
  }

  private confirmAction(message: string): boolean {
    return window.confirm(message);
  }

  private performGroupAction(
    action: () => any,
    successMessage: string,
    errorMessage: string,
    onSuccess?: () => void
  ): void {
    this.loadingState.actionLoading = true;

    action()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.showSuccessMessage(successMessage);
          this.loadDashboardData();
          onSuccess?.();
        },
        error: (error: any) => {
          console.error(`Error performing group action:`, error);
          this.showErrorMessage(errorMessage);
        },
        complete: () => {
          this.loadingState.actionLoading = false;
        }
      });
  }

  private handleGroupActionSuccess(groupId: number): void {
    if (this.modalState.selectedGroup?.id === groupId) {
      this.closeModal();
    }
  }

  private handleCreateSuccess(message: string): void {
    this.showSuccessMessage(message);
    this.loadDashboardData();
  }

  private handleCreateError(message: string, error: any): void {
    console.error('Create operation error:', error);
    this.showErrorMessage(message);
  }

  private validateGroupForm(): boolean {
    return !!(this.newGroup.name?.trim() && this.newGroup.environment?.trim());
  }

  private validatePermissionForm(): boolean {
    const hasName = !!(this.newPermission.name?.trim());
    const hasCategory = !!(this.newPermission.category?.trim());
    const hasCustomCategory = this.newPermission.category !== 'custom' || !!(this.customCategory?.trim());
    
    return hasName && hasCategory && hasCustomCategory;
  }

  private preparePermissionForCreation(): CreatePermission {
    const permission = { ...this.newPermission };
    if (permission.category === 'custom') {
      permission.category = this.customCategory;
    }
    return permission;
  }

  private loadGroupMembers(groupId: number): void {
    this.modalState.selectedGroupMembers = this.dashboardState.users.filter(user =>
      user.groups?.some(g => g.id === groupId)
    );
  }

  private findGroupById(groupId: number): Group | undefined {
    return this.dashboardState.userGroups.find(g => g.id === groupId) ||
           this.dashboardState.availableGroups.find(g => g.id === groupId);
  }

  private handleUserLogout(): void {
    this.router.navigate(['/login']);
  }

  private resetModalState(): void {
    this.modalState = {
      showModal: false,
      showDeleteConfirmation: false,
      selectedGroup: null,
      selectedUser: null,
      groupToDelete: null,
      selectedGroupMembers: []
    };
  }

  private createEmptyGroup(): CreateGroup {
    return {
      name: '',
      description: '',
      environment: this.dashboardState.currentEnvironment,
      permissionIds: []
    };
  }

  private createEmptyPermission(): CreatePermission {
    return {
      name: '',
      description: '',
      category: ''
    };
  }

  private resetGroupForm(): void {
    this.newGroup = this.createEmptyGroup();
    this.newGroup.environment = this.dashboardState.currentEnvironment;
  }

  private resetPermissionForm(): void {
    this.newPermission = this.createEmptyPermission();
    this.customCategory = '';
  }

  private showSuccessMessage(message: string): void {
    this.messageState.successMessage = message;
    this.messageState.errorMessage = '';
    this.clearMessageAfterDelay();
  }

  private showErrorMessage(message: string): void {
    this.messageState.errorMessage = message;
    this.messageState.successMessage = '';
    this.clearMessageAfterDelay();
  }

  private clearMessageAfterDelay(): void {
    setTimeout(() => {
      this.messageState.successMessage = '';
      this.messageState.errorMessage = '';
    }, 5000);
  }
} 