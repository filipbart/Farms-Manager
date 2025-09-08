export default interface AuthUserResponse {
  accessToken: string;
  expiryAtUtc: Date;
  mustChangePassword: boolean;
}
