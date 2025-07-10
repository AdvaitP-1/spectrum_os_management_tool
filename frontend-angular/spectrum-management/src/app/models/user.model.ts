import { SimpleGroup } from './group.model';

export interface User {
  id: string;
  name: string;
  email: string;
  currentEnvironment: string;
  createdAt: Date;
  lastLoginAt: Date;
  groups?: SimpleGroup[];
}

export interface SimpleUser {
  id: string;
  name: string;
  email: string;
  currentEnvironment: string;
}

export interface CreateUser {
  name: string;
  email: string;
  currentEnvironment: string;
}

export interface UpdateUser {
  name: string;
  email: string;
  currentEnvironment: string;
} 