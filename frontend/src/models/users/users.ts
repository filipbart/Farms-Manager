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
}

export interface UserDetailsModel {
  id: string;
  dateCreatedUtc: string;

  name: string;
  login: string;
}
