import ApiUrl from "../common/ApiUrl";
import type {
  ExpenseAdvanceEntity,
  ExpenseAdvancePermission,
  AssignExpenseAdvancePermissionRequest,
  UpdateExpenseAdvancePermissionRequest,
  RemoveExpenseAdvancePermissionRequest,
  UserExpenseAdvancePermissions,
  ExpenseAdvanceColumnSettingsResponse,
  CurrentUserColumnSettingsResponse,
  UpdateExpenseAdvanceColumnSettingsRequest,
} from "../models/expenses/advances/expense-advance-permissions";
import AxiosWrapper from "../utils/axios/wrapper";

export class ExpenseAdvancePermissionsService {
  /**
   * Pobiera wszystkie dostępne ewidencje zaliczek (dla wszystkich pracowników)
   */
  public static async getAllExpenseAdvances() {
    return await AxiosWrapper.get<ExpenseAdvanceEntity[]>(
      ApiUrl.ExpenseAdvancesList
    );
  }

  /**
   * Pobiera uprawnienia użytkownika do ewidencji zaliczek
   */
  public static async getUserExpenseAdvancePermissions(userId: string) {
    return await AxiosWrapper.get<UserExpenseAdvancePermissions>(
      ApiUrl.UserExpenseAdvancePermissions(userId)
    );
  }

  /**
   * Przypisuje uprawnienia do ewidencji zaliczek dla użytkownika
   */
  public static async assignExpenseAdvancePermission(
    request: AssignExpenseAdvancePermissionRequest
  ) {
    return await AxiosWrapper.post<ExpenseAdvancePermission[]>(
      ApiUrl.AssignExpenseAdvancePermission,
      request
    );
  }

  /**
   * Aktualizuje uprawnienia do ewidencji zaliczek
   */
  public static async updateExpenseAdvancePermission(
    request: UpdateExpenseAdvancePermissionRequest
  ) {
    return await AxiosWrapper.put<ExpenseAdvancePermission[]>(
      ApiUrl.UpdateExpenseAdvancePermission,
      request
    );
  }

  /**
   * Usuwa uprawnienia do ewidencji zaliczek
   */
  public static async removeExpenseAdvancePermission(
    request: RemoveExpenseAdvancePermissionRequest
  ) {
    return await AxiosWrapper.delete<void>(
      ApiUrl.RemoveExpenseAdvancePermission(request.permissionId)
    );
  }

  /**
   * Pobiera ustawienia widoczności kolumn dla użytkownika (panel administracyjny)
   */
  public static async getUserColumnSettings(userId: string) {
    return await AxiosWrapper.get<ExpenseAdvanceColumnSettingsResponse>(
      ApiUrl.UserExpenseAdvanceColumnSettings(userId)
    );
  }

  /**
   * Aktualizuje ustawienia widoczności kolumn dla użytkownika
   */
  public static async updateUserColumnSettings(
    request: UpdateExpenseAdvanceColumnSettingsRequest
  ) {
    return await AxiosWrapper.put<{ userId: string; visibleColumns: string[] }>(
      ApiUrl.UpdateExpenseAdvanceColumnSettings,
      request
    );
  }

  /**
   * Pobiera ustawienia widoczności kolumn dla aktualnie zalogowanego użytkownika
   */
  public static async getCurrentUserColumnSettings() {
    return await AxiosWrapper.get<CurrentUserColumnSettingsResponse>(
      ApiUrl.CurrentUserExpenseAdvanceColumnSettings
    );
  }
}
