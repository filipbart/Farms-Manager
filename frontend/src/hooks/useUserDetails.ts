import { useCallback, useState } from "react";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type UserDetails from "../models/user/user-details";
import { UserService } from "../services/user-service";

export function useUserDetails() {
  const [userDetails, setUserDetails] = useState<UserDetails>();
  const [loadingUser, setLoadingUser] = useState(false);

  const fetchUserDetails = useCallback(async () => {
    setLoadingUser(true);
    try {
      await handleApiResponse<UserDetails>(
        () => UserService.getDetails(),
        (data) => setUserDetails(data.responseData),
        undefined,
        "Nie udało się pobrać szczegółów użytkownika"
      );
    } finally {
      setLoadingUser(false);
    }
  }, []);

  return { userDetails, loadingUser, fetchUserDetails };
}
