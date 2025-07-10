import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, CreateUser, UpdateUser } from '../models/user.model';
import { Group, CreateGroup, UpdateGroup } from '../models/group.model';
import { Permission, CreatePermission } from '../models/permission.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {
    console.log('ApiService initialized with baseUrl:', this.baseUrl);
  }

  // User endpoints
  getUsers(environment?: string): Observable<User[]> {
    let params = new HttpParams();
    if (environment) {
      params = params.set('environment', environment);
    }
    return this.http.get<User[]>(`${this.baseUrl}/users`, { params });
  }

  getUser(id: string): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/users/${id}`);
  }

  createUser(user: CreateUser): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/users`, user);
  }

  updateUser(id: string, user: UpdateUser): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/users/${id}`, user);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/users/${id}`);
  }

  joinGroup(userId: string, groupId: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/users/${userId}/groups/${groupId}`, {});
  }

  leaveGroup(userId: string, groupId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/users/${userId}/groups/${groupId}`);
  }

  login(userId: string): Observable<User> {
    console.log('ApiService.login - Making request to:', `${this.baseUrl}/users/login`);
    console.log('ApiService.login - Payload:', userId);
    return this.http.post<User>(`${this.baseUrl}/users/login`, JSON.stringify(userId), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  // Group endpoints
  getGroups(environment?: string): Observable<Group[]> {
    let params = new HttpParams();
    if (environment) {
      params = params.set('environment', environment);
    }
    return this.http.get<Group[]>(`${this.baseUrl}/groups`, { params });
  }

  // Alias methods for dashboard compatibility
  getUserGroups(userId: string, environment?: string): Observable<Group[]> {
    return this.getGroupsByUser(userId, environment);
  }

  getAllGroups(environment?: string): Observable<Group[]> {
    return this.getGroups(environment);
  }

  getGroup(id: number): Observable<Group> {
    return this.http.get<Group>(`${this.baseUrl}/groups/${id}`);
  }

  createGroup(group: CreateGroup): Observable<Group> {
    return this.http.post<Group>(`${this.baseUrl}/groups`, group);
  }

  updateGroup(id: number, group: UpdateGroup): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/groups/${id}`, group);
  }

  deleteGroup(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/groups/${id}`);
  }

  getGroupsByUser(userId: string, environment?: string): Observable<Group[]> {
    let params = new HttpParams();
    if (environment) {
      params = params.set('environment', environment);
    }
    return this.http.get<Group[]>(`${this.baseUrl}/groups/by-user/${userId}`, { params });
  }

  getEnvironments(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/groups/environments`);
  }

  getGroupMembers(groupId: number): Observable<User[]> {
    return this.http.get<User[]>(`${this.baseUrl}/groups/${groupId}/members`);
  }

  // Permission endpoints
  getPermissions(category?: string): Observable<Permission[]> {
    let params = new HttpParams();
    if (category) {
      params = params.set('category', category);
    }
    return this.http.get<Permission[]>(`${this.baseUrl}/permissions`, { params });
  }

  getPermission(id: number): Observable<Permission> {
    return this.http.get<Permission>(`${this.baseUrl}/permissions/${id}`);
  }

  createPermission(permission: CreatePermission): Observable<Permission> {
    return this.http.post<Permission>(`${this.baseUrl}/permissions`, permission);
  }

  getPermissionCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/permissions/categories`);
  }
} 