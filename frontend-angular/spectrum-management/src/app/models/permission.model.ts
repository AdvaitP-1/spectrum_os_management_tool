export interface Permission {
  id: number;
  name: string;
  description: string;
  category: string;
  createdAt: Date;
}

export interface CreatePermission {
  name: string;
  description: string;
  category: string;
} 