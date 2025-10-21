import type { FilterConfig } from "../components/filters/filter-types";

/**
 * Generuje konfigurację checkboxa "Pokaż usunięte" dla filtrów (widoczny tylko dla admina)
 * @param isAdmin - czy użytkownik jest adminem
 * @returns konfiguracja filtra lub null jeśli użytkownik nie jest adminem
 */
export const getShowDeletedFilterConfig = (
  isAdmin: boolean
): FilterConfig | null => {
  if (!isAdmin) {
    return null;
  }

  return {
    type: "checkbox",
    key: "showDeleted",
    label: "Pokaż usunięte",
  };
};
