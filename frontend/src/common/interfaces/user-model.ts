export default interface UserModel {
  id: string;
  login: string;
  name: string;
  isAdmin: boolean;
  mustChangePassword: boolean;
  avatarPath?: string;
  permissions: string[];
  accessibleFarmIds: string[];
}
