import type DomainException from "./domain-exception";

export default interface BaseResponse<T> {
  success: boolean;
  responseData?: T;
  responseTimeUtc?: Date;
  errors?: Record<string, string>;
  statusCode: number;
  domainException?: DomainException;
}
