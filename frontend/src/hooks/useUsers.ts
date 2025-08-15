import { useCallback, useState } from "react";
import type { UserListModel } from "../models/users/users";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type { UsersFilterPaginationModel } from "../models/users/users-filters";
import { UsersService } from "../services/users-service";

export function useUsers(filters: UsersFilterPaginationModel) {
  const [users, setUsers] = useState<UserListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchUsers = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse(
        () => UsersService.getUsers(filters),
        (data) => {
          setUsers(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Nie udało się pobrać listy użytkowników"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  return {
    users,
    totalRows,
    loading,
    fetchUsers,
  };
}
