export default interface Action<T> {
  type: T;
  payload?: any;
}
