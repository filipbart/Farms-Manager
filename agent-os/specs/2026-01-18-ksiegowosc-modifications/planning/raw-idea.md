# Lista zadań do modyfikacji w module „Księgowość" (aplikacja farms-manager)

Kontekst: Wszystkie zmiany dotyczą zakładki „Księgowość" oraz modalu szczegółów faktury.

## 🐛 Błędy i poprawki UI (Bugfix & UI)

1. **Naprawa widoczności pola „Ubojnia" (Moduł Sprzedaże)**
   - Problem: Po wejściu w szczegóły faktury powiązanej z modułem „Sprzedaże", pole wyboru ubojni jest puste, mimo że zostało wcześniej wybrane i zapisane w bazie.
   - Oczekiwane działanie: System musi poprawnie zaciągać i wyświetlać przypisaną ubojnię przy ładowaniu modalu.

2. **Usunięcie szarego tła w formularzach**
   - Zadanie: W formularzach uzupełniania danych dla konkretnych modułów należy usunąć szare tło (stylizacja CSS), aby zachować spójność z resztą aplikacji.

3. **Walidacja przypisania pracownika**
   - Zadanie: Zablokuj możliwość kliknięcia przycisku „Zaakceptuj", jeśli do faktury nie przypisano pracownika.
   - Oczekiwane działanie: Przy próbie akceptacji system ma sprawdzić, czy pole pracownika jest wypełnione. Jeśli nie – wyświetlić komunikat i nie procesować akceptacji.

5. **Naprawa widoczności pola „Typ wydatku" (Moduł Koszty Produkcyjne)**
   - Problem: Analogicznie do pkt 1 – typ wydatku nie wyświetla się po ponownym otwarciu, ponieważ dane te prawdopodobnie są pobierane z KsefInvoiceEntity, zamiast z encji dedykowanej dla modułu (np. ExpenseProductionEntity).
   - Rozwiązanie: Należy pobierać „Typ wydatku" z encji powiązanej z danym modułem, gdzie ta informacja jest faktycznie zapisana.

6. **Odświeżanie po akcji „Przekaż do biura"**
   - Problem: Kliknięcie „Przekaż do biura" wykonuje akcję, ale widok się nie aktualizuje.
   - Oczekiwane działanie: Po sukcesie akcji (response 200 OK) należy automatycznie odświeżyć listę/widok, aby status faktury był aktualny dla użytkownika.

## 🚀 Nowe funkcjonalności i Zmiany w logice (Features & Logic)

4. **Automatyczne tworzenie kontrahentów i obsługa typów wydatków przy synchronizacji w KsefSynchronizationJob**
   - Kontekst: Moduł Gaz oraz Koszty Produkcyjne.
   - Logika:
     1. Jeśli system nie znajdzie kontrahenta po numerze NIP, ma go automatycznie utworzyć.
     2. Jeśli jest to nowy kontrahent (w scenariuszu Koszty Produkcyjne) i nie ma przypisanych typów wydatków:
        - Wyświetl listę wszystkich dostępnych typów wydatku w modalu SZCZEGÓŁÓW faktury.
        - Po wybraniu typu przez użytkownika, przypisz ten typ do nowo utworzonego kontrahenta na stałe (analogicznie do wprowadzania automatycznej faktury).

7. **Tryb „Przeglądaj faktury" (Iteracja po liście)**
   - Zadanie: Dodać przycisk „Przeglądaj faktury".
   - Działanie:
     1. Funkcja bierze pod uwagę aktualnie wyfiltrowaną listę faktur.
     2. Otwiera szczegóły pierwszej faktury z tej listy.
     3. Po kliknięciu „Zaakceptuj" (lub wykonaniu głównej akcji) system automatycznie ładuje szczegóły kolejnej faktury z tej samej przefiltrowanej listy.
     4. Proces trwa do wyczerpania listy.

8. **Możliwość zmiany statusu płatności zaakceptowanej faktury**
   - Zadanie: Dodać przycisk, który pozwala nadpisać status płatności faktury, która została już zaakceptowana (umożliwienie korekty pomyłek lub aktualizację). Przycisk się wyświetla tylko w tym scenariuszu: kiedy faktura zaakceptowana i kiedy został zmieniony status płatności.

9. **Synchronizacja statusów płatności (Paszowóz <-> Księgowość)**
   - Ważne: Encje są we wspólnej bazie i są powiązane (podmiot, moduł, numer faktury), więc nie wymaga to integracji między różnymi mikroserwisami.
   - Logika biznesowa: Statusy płatności faktur za pasze muszą być spójne z podsystemem płatności. Tak samo w przypadku Sprzedaże -> „Faktury sprzedażowe"
   - Wymagany przepływ (Workflow):
     1. Użytkownik generuje przelew w zakładce „Dostawy pasz".
     2. Przechodzi do zakładki „Przelewy".
     3. Wskazuje datę wykonania przelewu.
     4. System automatycznie aktualizuje status odpowiedniej faktury w zakładce „Księgowość" na „Opłacona przelewem".

10. **Upload plików do S3 (Załączniki)**
    - Zadanie: Dodać możliwość wgrania fizycznego dodatkowych/pomocniczych plików do KsefInvoiceEntity.
    - Technikalia: Plik ma być wysyłany i zapisywany w S3 (analogicznie jak w innych modułach aplikacji).

11. **Obsługa dat płatności**
    - Zadanie: Dodać pole „Data płatności".
    - Logika domyślnych wartości:
      - Jeśli metoda to Gotówka → domyślnie wstaw „Data wystawienia faktury".
      - Jeśli metoda to Przelew → domyślnie wstaw „Dzisiaj" (data zatwierdzenia/kliknięcia „Opłacona przelewem").
    - Użytkownik musi mieć możliwość ręcznej edycji tej daty.

12. **Logi audytowe (Historia zmian)**
    - Zadanie: Stworzyć rejestr akcji dla admina, śledzący kto i kiedy zmienił status faktury.
    - Zdarzenia do logowania:
      - „Zaakceptuj"
      - „Wstrzymaj"
      - „Odrzuć"
      - „Przekaż do biura"

W przypadku niejasności lub braku danych w którymkolwiek punkcie, proszę o zadanie pytań doprecyzujących przed rozpoczęciem implementacji.
