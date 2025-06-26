export interface IrzPlusCredentials {
  login: string;
  password: string;
}

export default interface UserDetails {
  login: string;
  name: string;
  irzplusCredentials?: IrzPlusCredentials;
}
