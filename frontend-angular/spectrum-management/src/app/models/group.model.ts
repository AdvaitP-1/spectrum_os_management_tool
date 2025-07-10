import { Permission } from './permission.model';
import { SimpleUser } from './user.model';

export interface Group {
  id: number;
  name: string;
  description: string;
  environment: string;
  createdAt: Date;
  createdBy: string;
  permissions?: Permission[];
  users?: SimpleUser[];
  userCount: number;
}

export interface SimpleGroup {
  id: number;
  name: string;
  description: string;
  environment: string;
  createdAt: Date;
  userCount: number;
}

export interface CreateGroup {
  name: string;
  description: string;
  environment: string;
  permissionIds: number[];
}

export interface UpdateGroup {
  name: string;
  description: string;
  permissionIds: number[];
}

export interface GroupMembership {
  userId: string;
  groupId: number;
} 