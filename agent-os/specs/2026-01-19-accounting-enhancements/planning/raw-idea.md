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
