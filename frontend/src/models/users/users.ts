export interface UserListModel {
  id: string;
  dateCreatedUtc: string;

  login: string;
  name: string;
}

export interface AddUserData {
  login: string;
  name: string;
  temporaryPassword: string;
}

export interface UpdateUserData {
  name: string;
  password?: string;
  isAdmin: boolean;
}

export interface UserDetailsModel {
  id: string;
  dateCreatedUtc: string;

  name: string;
  login: string;
  isAdmin: boolean;
  permissions: string[];
  accessibleFarmIds: string[];
}

export interface PermissionModel {
  name: string;
  description: string;
  group: string;
}

export interface GetPermissionsResponse {
  items: PermissionModel[];
}
