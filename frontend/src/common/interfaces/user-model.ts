export default interface UserModel {
  id: string;
  login: string;
  name: string;
  isAdmin?: boolean;
  permissions: string[];
  accessibleFarmIds: string[];
}
