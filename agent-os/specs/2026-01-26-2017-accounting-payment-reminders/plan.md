# Accounting Payment Reminders — Plan

## Przegląd

System przypomnień o terminach płatności faktur w zakładce księgowość. Alerty wizualne (kolory wierszy w DataGrid, badge w sidebarze, powiadomienia na dashboardzie) informujące użytkowników o zbliżających się terminach płatności.

## Reguły priorytetów

| Dni do terminu | Kolor | Priorytet |
|----------------|-------|-----------|
| ≥14 dni | brak | — |
| 13–8 dni | żółty | Low |
| 7–4 dni | pomarańczowy | Medium |
| 3–1 dni | czerwony | High |
| po terminie | czerwony | High |

## Widoczność

- Jeśli faktura ma przypisanego pracownika (`AssignedUser`) → alerty widoczne tylko dla niego
- Jeśli brak przypisania → alerty widoczne dla wszystkich

---

## Task 1: Zapisanie dokumentacji specyfikacji ✅

Utworzono folder `agent-os/specs/2026-01-26-2017-accounting-payment-reminders/` z:
- plan.md
- shape.md
- standards.md
- references.md

---

## Task 2: Backend — logika przypomnień księgowości

### 2.1 Rozszerzenie NotificationType
- Dodać `AccountingInvoice` do enum `NotificationType`

### 2.2 Rozszerzenie GetNotificationsQuery
- Dodać nową sekcję `AccountingInvoices` w `NotificationData`
- Pobrać faktury księgowe z due date w zakresie 14 dni
- Filtrować wg przypisanego pracownika (lub wszystkie jeśli brak)
- Zastosować nowe reguły priorytetów

### 2.3 Rozszerzenie GetDashboardNotificationsQuery
- Dodać faktury księgowe jako źródło powiadomień dashboardowych
- Uwzględnić filtrowanie wg przypisanego pracownika

---

## Task 3: Frontend — dashboard + sidebar

### 3.1 Modele
- Dodać `AccountingInvoice` do typu `NotificationType`
- Rozszerzyć `NotificationData` o `accountingInvoices`

### 3.2 Dashboard
- Dodać ikonę dla typu `AccountingInvoice`
- Dodać linkowanie do `/accounting`

### 3.3 Sidebar
- Dodać badge z licznikiem i priorytetem przy pozycji "Księgowość"

---

## Task 4: Frontend — DataGrid w księgowości

### 4.1 Helper dla klas CSS
- Utworzyć/rozszerzyć helper `getDueDateClassName` dla faktur księgowych
- Uwzględnić nowe progi (14/8/4/1 dni)

### 4.2 DataGrid styling
- Dodać `getRowClassName` w `accounting/index.tsx`
- Dodać klasy CSS w `sx` DataGrid:
  - `.payment-due-warning` (żółty)
  - `.payment-due-soon` (pomarańczowy)  
  - `.payment-overdue` (czerwony)

---

## Status

- [x] Task 1: Dokumentacja specyfikacji
- [ ] Task 2: Backend
- [ ] Task 3: Frontend dashboard + sidebar
- [ ] Task 4: Frontend DataGrid
