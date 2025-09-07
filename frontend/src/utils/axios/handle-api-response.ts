import { toast } from "react-toastify";
import type BaseResponse from "./base-response";

export async function handleApiResponse<T>(
  apiCall: () => Promise<BaseResponse<T>>,
  onSuccess?: (data: BaseResponse<T>) => void,
  onError?: (data: BaseResponse<T>) => void,
  fallbackErrorMessage?: string | null
): Promise<void> {
  try {
    const response = await apiCall();

    if (response.success) {
      onSuccess?.(response);
    } else {
      if (response.domainException?.errorDescription) {
        toast.error(response.domainException.errorDescription);
      } else if (response.errors && typeof response.errors === "object") {
        Object.values(response.errors)
          .flat()
          .forEach((err: any) => toast.error(err));
      } else {
        toast.error(
          fallbackErrorMessage ?? "Wystąpił błąd podczas przetwarzania żądania."
        );
      }

      onError?.(response);
    }
  } catch (error: any) {
    if (error.name === "CanceledError") {
      throw error;
    }

    toast.error(
      fallbackErrorMessage ?? "Wystąpił błąd podczas przetwarzania żądania."
    );

    throw error;
  }
}

export async function handleApiResponseWithResult<T>(
  apiCall: () => Promise<BaseResponse<T>>,
  fallbackErrorMessage?: string | null
): Promise<BaseResponse<T> | null> {
  try {
    const response = await apiCall();

    if (response.success) {
      return response;
    }

    if (response.domainException?.errorDescription) {
      toast.error(response.domainException.errorDescription);
    } else if (response.errors && typeof response.errors === "object") {
      Object.values(response.errors)
        .flat()
        .forEach((err: any) => toast.error(err));
    } else {
      toast.error(
        fallbackErrorMessage ?? "Wystąpił błąd podczas przetwarzania żądania."
      );
    }

    return null;
  } catch (error: any) {
    if (error.name === "CanceledError") {
      throw error;
    }

    toast.error(
      fallbackErrorMessage ?? "Wystąpił błąd podczas przetwarzania żądania."
    );

    return null;
  }
}
