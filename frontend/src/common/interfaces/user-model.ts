export default interface UserModel {
  id: string;
  login: string;
  name: string;
  isAdmin: boolean;
  mustChangePassword: boolean;
  permissions: string[];
  accessibleFarmIds: string[];
}
