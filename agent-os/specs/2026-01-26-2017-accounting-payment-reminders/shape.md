# Accounting Payment Reminders — Shaping Notes

## Zakres

System przypomnień o terminach płatności faktur w module księgowości. Funkcja wzorowana na istniejących alertach w module pasz (dostawy pasz).

### Co budujemy

1. **Alerty wizualne w DataGrid** — podświetlanie wierszy faktur wg zbliżającego się terminu płatności
2. **Badge w sidebarze** — licznik faktur z nadchodzącymi terminami przy pozycji "Księgowość"
3. **Powiadomienia na dashboardzie** — wpisy o fakturach księgowych w liście nadchodzących terminów

### Poza zakresem

- Powiadomienia email/push (tylko wizualne)
- Automatyczne przypomnienia cykliczne

## Decyzje

### Reguły kolorów/priorytetów

| Dni do terminu | Kolor | Priorytet |
|----------------|-------|-----------|
| ≥14 dni | brak alertu | — |
| 13–8 dni | żółty | Low |
| 7–4 dni | pomarańczowy | Medium |
| 3–1 dni | czerwony | High |
| po terminie | czerwony | High |

### Widoczność alertów

- Faktury z przypisanym pracownikiem (`AssignedUser`) → alerty widoczne **tylko dla tego pracownika**
- Faktury bez przypisania → alerty widoczne **dla wszystkich użytkowników**

### Filtrowanie

- Alerty dotyczą tylko faktur **nieopłaconych** (bez daty płatności)
- Uwzględniamy faktury z `DueDate` w zakresie 14 dni od dziś

## Kontekst

- **Wizualne materiały:** Brak — bazujemy na kodzie
- **Referencje:** 
  - Dashboard notifications (`GetDashboardNotificationsQuery`)
  - Sidebar badge (`GetNotificationsQuery`)
  - Feed deliveries row highlighting
- **Zgodność z produktem:** Niezależne ulepszenie UX (nie powiązane z konkretnym celem roadmapy)

## Standardy zastosowane

- backend/api — konwencje API
- backend/queries — wzorce zapytań
- frontend/components — wzorce komponentów UI
- global/coding-style — konwencje kodowania
- global/error-handling — obsługa błędów
