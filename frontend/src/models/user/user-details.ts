export interface IrzPlusCredentials {
  login: string;
  password: string;
}

export default interface UserDetails {
  login: string;
  name: string;
  irzplusCredentials?: IrzPlusCredentials[];
  avatarPath?: string;
}

export interface UpdateMyData {
  name: string;
  password?: string;
}
