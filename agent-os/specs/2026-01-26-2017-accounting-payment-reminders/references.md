# References for Accounting Payment Reminders

## Similar Implementations

### 1. Dashboard Notifications (Backend)

- **Location:** `backend/src/FarmsManager.Application/Queries/Dashboard/GetDashboardNotificationsQuery.Handler.cs`
- **Relevance:** Główny wzorzec dla agregacji powiadomień z różnych źródeł i wyświetlania na dashboardzie
- **Key patterns:**
  - `NotificationSource` record do ujednolicenia źródeł
  - Priorytetyzacja wg dni do terminu
  - Top 5 powiadomień posortowanych wg pilności
  - `GenerateDescription` dla czytelnych opisów

### 2. Sidebar Badge Notifications (Backend)

- **Location:** `backend/src/FarmsManager.Application/Queries/User/GetNotificationsQuery.cs`
- **Relevance:** Wzorzec dla liczników i priorytetów w sidebarze
- **Key patterns:**
  - `NotificationData` z sekcjami per moduł (SalesInvoices, FeedDeliveries, Employees)
  - `ProcessNotifications<T>` - generyczna metoda agregacji
  - `GetPriority` - logika ustalania priorytetu
  - Specyfikacje do pobierania faktur z nadchodzącymi terminami

### 3. Dashboard Notifications (Frontend)

- **Location:** `frontend/src/components/dashboard/dashboard-notifications.tsx`
- **Relevance:** Wzorzec UI dla wyświetlania powiadomień
- **Key patterns:**
  - `notificationIcons` - mapowanie typu na ikonę
  - `priorityConfig` - mapowanie priorytetu na kolor i label
  - `getLinkPath` - nawigacja do źródła powiadomienia

### 4. Sidebar Menu Item Badge (Frontend)

- **Location:** `frontend/src/layouts/dashboard/sidebar-menu-item.tsx`
- **Relevance:** Wzorzec badge z licznikiem i kolorem priorytetu
- **Key patterns:**
  - `getBadgeColor()` - mapowanie priorytetu na kolor
  - `renderBadge()` - komponent badge z licznikiem
  - Props: `notificationCount`, `notificationPriority`

### 5. Feed Deliveries Row Highlighting (Frontend)

- **Location:** `frontend/src/pages/feeds/deliveries/index.tsx`
- **Relevance:** Główny wzorzec dla podświetlania wierszy w DataGrid
- **Key patterns:**
  - `getRowClassName` z `getDueDateClassName` helper
  - Klasy CSS w `sx` DataGrid:
    - `.payment-overdue` (czerwony)
    - `.payment-due-soon` (żółty)
    - `.payment-due-warning` (niebieski/info)
  - Style dla hover state

### 6. Due Date Helper (Frontend)

- **Location:** `frontend/src/utils/due-date-helper.ts`
- **Relevance:** Helper do obliczania klasy CSS na podstawie terminu płatności
- **Key patterns:**
  - Funkcja `getDueDateClassName(dueDate, paymentDate, isCorrection)`
  - Logika progów czasowych
  - Zwraca nazwę klasy CSS

## Notification Data Model

```typescript
// frontend/src/models/common/notifications.ts
interface NotificationData {
  salesInvoices: NotificationInfo;
  feedDeliveries: NotificationInfo;
  employees: NotificationInfo;
  // + accountingInvoices: NotificationInfo; // do dodania
}

interface NotificationInfo {
  count: number;
  priority: NotificationPriority;
}

enum NotificationPriority {
  Low = "Low",
  Medium = "Medium", 
  High = "High"
}
```

## Dashboard Notification Item

```typescript
// frontend/src/models/dashboard/dashboard.ts
type NotificationType = 
  | "SaleInvoice"
  | "FeedInvoice"
  | "EmployeeContract"
  | "EmployeeReminder";
  // + "AccountingInvoice" // do dodania

interface DashboardNotificationItem {
  description: string;
  dueDate: string;
  priority: NotificationPriority;
  type: NotificationType;
  sourceId: string;
}
```
