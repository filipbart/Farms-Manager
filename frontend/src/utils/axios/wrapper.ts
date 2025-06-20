import type BaseResponse from "./base-response";
import type DomainException from "./domain-exception";
import axios from "axios";
import qs from "qs";

const handleErrorResponse: <T>(error: any) => BaseResponse<T> = (
  error: any
) => {
  let domainException: DomainException | undefined = undefined;
  if (error?.response?.data?.errorName) {
    domainException = error.response.data;
  }

  return {
    success: false,
    domainException: domainException,
    statusCode: error?.response?.status || 500,
    responseData: {} as any,
    errors: error?.response?.data?.errors || [],
  };
};

export default class AxiosWrapper {
  public static getAuthTokenFromHeader(): string {
    return (
      (axios.defaults.headers.common["Authorization"] as string) || ""
    ).replace("Bearer ", "");
  }

  public static setAuthToken(token: string): void {
    axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  }

  public static get<T>(
    url: string,
    params: any = undefined,
    withCredentials = false
  ): Promise<BaseResponse<T>> {
    return new Promise((resolve) => {
      axios
        .get<BaseResponse<T>>(url, {
          params,
          paramsSerializer: (params: any) => {
            return qs.stringify(params, { arrayFormat: "repeat" });
          },
          withCredentials,
        })

        .then((response) => {
          resolve({
            ...response?.data,
            statusCode: response.status,
            success: response.data.success,
          });
        })
        .catch((error) => {
          resolve(handleErrorResponse(error));
        });
    });
  }

  public static put<T>(
    url: string,
    params?: any,
    queryParams?: any
  ): Promise<BaseResponse<T>> {
    return new Promise((resolve) => {
      axios
        .put<BaseResponse<T>>(url, params, {
          params: queryParams,
        })
        .then((response) => {
          resolve({
            ...response.data,
            statusCode: response.status,
            success: response.data.success,
          });
        })
        .catch((error) => {
          resolve(handleErrorResponse(error));
        });
    });
  }

  public static patch<T>(url: string, params?: any): Promise<BaseResponse<T>> {
    return new Promise((resolve) => {
      axios
        .patch<BaseResponse<T>>(url, params)
        .then((response) => {
          resolve({
            ...response.data,
            statusCode: response.status,
            success: response.data.success,
            errors: response.data.errors,
          });
        })
        .catch((error) => {
          resolve(handleErrorResponse(error));
        });
    });
  }

  public static post<T>(
    url: string,
    params?: any,
    queryParams?: any,
    withCredentials = false,
    isFormData = false
  ): Promise<BaseResponse<T>> {
    return new Promise((resolve) => {
      axios
        .post<BaseResponse<T>>(url, params, {
          params: queryParams,
          withCredentials,
          paramsSerializer: (params: any) => {
            return qs.stringify(params, { arrayFormat: "repeat" });
          },
          headers: isFormData ? { "Content-Type": "multipart/form-data" } : {},
        })
        .then((response) => {
          resolve({
            ...response.data,
            statusCode: response.status,
            success: response.data.success,
            domainException: response?.data?.domainException,
          });
        })
        .catch((error) => {
          resolve(handleErrorResponse(error));
        });
    });
  }

  public static delete<T>(url: string, params?: any): Promise<BaseResponse<T>> {
    return new Promise((resolve) => {
      axios
        .delete<BaseResponse<T>>(url, { data: params })
        .then((response) => {
          resolve({
            ...response.data,
            statusCode: response.status,
            success: response.data.success,
          });
        })
        .catch((error) => {
          resolve(handleErrorResponse(error));
        });
    });
  }
}
