# Spec Requirements: ksiegowosc-modifications

## Initial Description

Lista zadań do modyfikacji w module „Księgowość" (aplikacja farms-manager)

Kontekst: Wszystkie zmiany dotyczą zakładki „Księgowość" oraz modalu szczegółów faktury.

## Requirements Discussion

### First Round Questions

**Q1:** Zakładam, że wszystkie błędy (punkty 1, 5, 6) dotyczą problemów z wyświetlaniem danych, które już są zapisane w bazie, ale nie są poprawnie ładowane w modalu szczegółów faktury. Czy te problemy występują we wszystkich modułach (Sprzedaże, Koszty Produkcyjne, Gaz, Pasze) czy tylko w wymienionych?
**Answer:** Sprawdź wszystkie bo nie jestem pewien.

**Q2:** Co do walidacji przypisania pracownika (pkt 3) - czy ta walidacja ma dotyczyć wszystkich typów faktur czy tylko niektórych modułów? I czy pracownik ma być wymagany również dla faktur z KSeF, które są automatycznie synchronizowane?
**Answer:** Wszystkich typów faktur. Tak, ale wtedy i tak system w KSeFSynchronizationJob.cs przypisuje odpowiedniego

**Q3:** W trybie "Przeglądaj faktury" (pkt 7) - czy po wykonaniu głównej akcji (np. "Zaakceptuj") system ma automatycznie przechodzić do kolejnej faktury, czy użytkownik ma mieć możliwość przerwania procesu? I czy ten tryb ma być dostępny tylko dla użytkowników z uprawnieniami do akceptacji faktur?
**Answer:** Automatycznie przechodzić do kolejnej faktury oraz użytkownik ma mieć możliwość przerwania tego. Tak, niech będzie tylko dla uprawnionych

**Q4:** Co do synchronizacji statusów płatności (pkt 9) - czy ta synchronizacja ma być dwukierunkowa (zmiana w Paszowozie aktualizuje Księgowość i odwrotnie) czy tylko jednokierunkowa z Paszowóz/Sprzedaże do Księgowości?
**Answer:** W dwie strony, ale w jedną już chyba aktualnie jest - jak nie no to tak, w dwie strony.

**Q5:** Dla logów audytowych (pkt 12) - czy te logi mają być dostępne tylko dla administratorów systemu czy też dla innych ról? I czy historia ma zawierać tylko zmiany statusów czy także inne modyfikacje faktury (np. zmianę przypisanego pracownika)?
**Answer:** Tylko dla administratorów systemu, ale na razie skup się na zapisywaniu tych logów, nie twórz widoku dla nich. Na razie sam zapis.

**Q6:** Czy wszystkie nowe funkcjonalności (punkty 4, 7, 8, 9, 10, 11, 12) mają być wdrożone jednocześnie, czy preferujesz podział na etapy/priorytety?
**Answer:** Jak wygodniej, może być etapowo, może być jednocześnie.

### Existing Code to Reference

**Similar Features Identified:**
- KSeFSynchronizationJob.cs - Path: `backend/src/FarmsManager.Infrastructure/BackgroundJobs/KSeFSynchronizationJob.cs`
- Components to potentially reuse: Komponenty z modułów pasz, wstawień - większość komponentów korzysta
- Backend logic to reference: Istniejące mechanizmy synchronizacji, logowania, obsługi plików S3

## Visual Assets

### Files Provided:
No visual assets provided.

### Visual Insights:
- Brak dostarczonych assetów wizualnych
- Rozwój oparty na istniejących wzorcach UI w aplikacji

## Requirements Summary

### Functional Requirements
- Naprawa wyświetlania pól "Ubojnia" i "Typ wydatku" w modalu szczegółów faktury
- Usunięcie szarego tła w formularzach modułowych
- Walidacja przypisania pracownika dla wszystkich typów faktur
- Automatyczne odświeżanie widoku po akcji "Przekaż do biura"
- Automatyczne tworzenie kontrahentów i przypisywanie typów wydatków w KSeFSynchronizationJob
- Tryb "Przeglądaj faktury" z iteracją po przefiltrowanej liście
- Możliwość zmiany statusu płatności zaakceptowanych faktur
- Dwukierunkowa synchronizacja statusów płatności między modułami
- Upload plików do S3 jako załączników do faktur
- Obsługa dat płatności z logiką domyślnych wartości
- Logi audytowe dla zmian statusów faktur (tylko zapis)

### Reusability Opportunities
- Komponenty UI z modułów pasz i wstawień
- Istniejące mechanizmy synchronizacji KSeF
- Wzorce obsługi plików S3 w innych modułach
- Systemy walidacji i formularzy z istniejących modułów
- Mechanizmy uprawnień i ról użytkowników

### Scope Boundaries
**In Scope:**
- Wszystkie 12 zadań z listy (bugfixes + nowe funkcjonalności)
- Naprawa wyświetlania danych we wszystkich modułach
- Implementacja logiki dwukierunkowej synchronizacji
- Tylko zapis logów audytowych (bez widoku)

**Out of Scope:**
- Widok dla logów audytowych (tylko backend)
- Integracje zewnętrzne poza KSeF
- Zmiany w innych modułach niż Księgowość

### Technical Considerations
- Integracja z istniejącym KSeFSynchronizationJob.cs
- Użycie istniejących komponentów MUI/React
- Backend: Clean Architecture z CQRS
- Storage: S3 dla załączników
- Baza danych: wspólna dla modułów (ułatwia synchronizację)
- Uprawnienia: rola administratora dla logów audytowych
