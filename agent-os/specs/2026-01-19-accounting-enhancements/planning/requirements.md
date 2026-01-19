# Spec Requirements: Accounting Enhancements

## Initial Description
Chciałbym podać własny feature, a raczej listę zadań do zrealizowania:
1. Trzeba dodać w @[frontend/src/components/modals/accounting/upload-invoice-modal.tsx]  wybór modułu, po wyborze: Gaz, Koszty produkcyjne, Paszy i Sprzedaże - przy tych jest oddzielne metody po backendzie które zczytują odpowiednie dane dla modułów,  dla reszty po prostu jest wybór modułu z @[backend/src/FarmsManager.Domain/Aggregates/AccountingAggregate/Enums/ModuleType.cs]  i zaczytywane są te dane potrzebne do utworzenia faktury @[backend/src/FarmsManager.Domain/Aggregates/AccountingAggregate/Entities/KSeFInvoiceEntity.cs]   czyli podstawowe, odczytaj jakie. Po odczytaniu przez AI (Azure DI) danych, mają się uzupełniać jak dla konkretnego modułu, sprawdź scenariusz zczytywania faktur za pasze.
2. W widoku @[frontend/src/components/modals/accounting/save-accounting-invoice-modal.tsx]    przy scrollowaniu, scrolluje się tez widok po lewej stronie (podgląd faktury) a chciałbym żeby te dwie rzeczy były niezależne między sobą przy scrollowaniu. Tzn, scrolluje to po prawej to po lewej się nie scrolluje, chyba rozumiesz o co chodzi.
3. Jesli faktura poza KSeF to: 
- Zamiast wizualizacji przy podglądzie (@[frontend/src/components/modals/accounting/invoice-details-modal.tsx]   )to ma pokazywać się faktyczny wgrany plik faktury. Czyli de facto podgląd z @[frontend/src/components/modals/accounting/save-accounting-invoice-modal.tsx]   
- Jeżeli jeden podmiot ma jedną fermę to niech od razu dopasowuje lokalizację 
- Przy dodawaniu w @[frontend/src/components/modals/accounting/save-accounting-invoice-modal.tsx]   trzeba umożliwić dodawanie dodatkowych załączników do tej faktury (opcjonalnych oczywiście)
- Dodanie walidacji duplikatów w poszczególnych modułach. Czyli przy ręcznym dodawaniu i jak się dodaje od razu wpis do konkretnego modułu to ma sprawdzać duplikaty. (Podmiot, numer faktury)
- Przy pobieraniu faktury w formacie pdf w @[frontend/src/pages/accounting/index.tsx] zamiast wygenerowanej wizualizacji to niech pobiera ten wgrany plik

4. Dodatkowy filtr w @[frontend/src/pages/sales/index.tsx]   po module @[backend/src/FarmsManager.Domain/Aggregates/AccountingAggregate/Enums/ModuleType.cs]   

## Requirements Discussion

### First Round Questions

**Q1:** Czy dobrze rozumiem, że dla modułów "Gaz", "Koszty produkcyjne", "Pasze" i "Sprzedaże" ekstrakcja AI powinna priorytetyzować pola specyficzne dla tych modułów (np. ilości, daty dostaw, dane kontrahentów), a dla pozostałych powinna skupić się na standardowych polach `KSeFInvoiceEntity`?
**Answer:** Dokładnie

**Q2:** Planuję zastosować rozwiązanie oparte na CSS (np. `overflow-y-auto` na dwóch oddzielnych kontenerach) w `save-accounting-invoice-modal.tsx`. Czy pozycje przewijania powinny być zachowane, jeśli użytkownik przełącza się między zakładkami/modułami w ramach tej samej sesji modalu?
**Answer:** Po prostu niech będą one odrębne, w sensie żeby to co jest po lewej nie scrollowało się gdy scrollujemy prawą część modalu

**Q3:** W przypadku plików wgrywanych ręcznie zakładam, że powinniśmy obsługiwać popularne formaty, takie jak PDF i obrazy (JPG/PNG). Czy podgląd "prawdziwego pliku" ma całkowicie zastąpić wizualizację, czy nadal powinniśmy oferować opcję "Przełącz widok", aby zobaczyć dane wyekstrahowane przez system obok pliku?
**Answer:** Całkowicie zastąpić wizualizacje i skorzystać z @[FilePreview] ale tylko dla faktura spoza KSeF

**Q4:** Jeśli podmiot gospodarczy ma tylko jedną fermę, automatycznie ją wybiorę. Czy użytkownik powinien mieć możliwość "odznaczenia" lub zmiany tej fermy, czy ma ona być "zablokowana", jeśli jest to jedyna opcja?
**Answer:** Możliwość edycji zawsze, ale niech przypisuje wpierw samemu

**Q5:** Zakładam, że sprawdzanie duplikatów (Podmiot + Numer faktury) powinno odbywać się "na bieżąco" podczas wpisywania numeru, pokazując ostrzeżenie lub blokując przycisk "Zapisz". Które rozwiązanie preferujesz?
**Answer:** Po kliknięciu "Zapisz" ma wtedy zwalidować

**Q6:** Czy dodatkowe załączniki powinny być przechowywane w tym samym miejscu co główny plik faktury i czy mają być również dostępne do pobrania z widoku `accounting/index.tsx`?
**Answer:** Może jakiś podfolder? I mają być również możliwość pobrania/podglądu ale w szczegółach danej faktury, bo to per faktura maja być te dodatkowe załączniki

**Q7:** Czy filtr modułu w `sales/index.tsx` powinien być filtrem wielokrotnego wyboru, czy prostą listą rozwijaną?
**Answer:** Źle zrozumiałeś po prostu w @[frontend/src/pages/accounting/index.tsx] trzeba dodać filtr po module (Pasze, gaz itd. - @[backend/src/FarmsManager.Domain/Aggregates/AccountingAggregate/Enums/ModuleType.cs] )

**Q8:** Czy istnieją jakieś konkretne limity rozmiaru plików lub typy plików, które powinniśmy wykluczyć w przypadku dodatkowych załączników?
**Answer:** Nie istnieją

### Existing Code to Reference

**Similar Features Identified:**
- Użytkownik potwierdził, że w kodzie są podobne wzorce i należy z nich skorzystać.

## Visual Assets

### Files Provided:
No visual assets provided.

## Requirements Summary

### Functional Requirements
- Dodanie wyboru modułu w `upload-invoice-modal.tsx`.
- Ekstrakcja danych AI (Azure DI) dostosowana do wybranego modułu (Gaz, Koszty produkcyjne, Pasze, Sprzedaże) lub standardowa dla pozostałych.
- Niezależne przewijanie lewej (podgląd) i prawej (formularz) części w `save-accounting-invoice-modal.tsx`.
- Zastąpienie wizualizacji PDF/XML podglądem faktycznego pliku (`FilePreview`) dla faktur spoza KSeF.
- Automatyczne przypisanie lokalizacji (fermy), jeśli podmiot ma tylko jedną, z możliwością edycji.
- Możliwość dodawania opcjonalnych załączników do faktury w `save-accounting-invoice-modal.tsx`.
- Pobieranie i podgląd dodatkowych załączników w szczegółach faktury.
- Walidacja duplikatów (Podmiot + Numer faktury) przy kliknięciu "Zapisz".
- Pobieranie oryginalnego pliku faktury zamiast wizualizacji w `accounting/index.tsx`.
- Dodanie filtra po module (`ModuleType`) w `accounting/index.tsx` oraz `sales/index.tsx`.

### Reusability Opportunities
- Wykorzystanie komponentu `FilePreview` do podglądu faktur spoza KSeF.
- Wykorzystanie istniejących wzorców w kodzie dla wgrywania plików i walidacji.

### Scope Boundaries
**In Scope:**
- Modyfikacja modali wgrywania i zapisywania faktur.
- Rozszerzenie backendu o obsługę dodatkowych załączników i walidację duplikatów.
- Modyfikacja list faktur (filtry i pobieranie plików).

**Out of Scope:**
- Zmiana wizualizacji dla faktur KSeF (zostaje po staremu).

### Technical Considerations
- Wykorzystanie Azure Document Intelligence do ekstrakcji danych.
- Obsługa plików PDF/Obrazów dla faktur spoza KSeF.
- Implementacja walidacji duplikatów po stronie serwera/klienta przy zapisie.
