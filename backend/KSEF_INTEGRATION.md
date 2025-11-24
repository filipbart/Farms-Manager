# Integracja z KSeF (Krajowy System e-Faktur)

## Przegląd

Implementacja integracji z Krajowym Systemem e-Faktur umożliwia:
- Pobieranie faktur sprzedaży (wystawionych przez fermy)
- Pobieranie faktur zakupu (wystawionych dla farm)
- Filtrowanie po NIP kontrahenta i zakresie dat
- Pobieranie szczegółów faktur
- Pobieranie XML faktur

## Architektura

### Komponenty

1. **IKSeFService** (`/Application/Interfaces/IKSeFService.cs`)
   - Interfejs serwisu do komunikacji z KSeF
   - Metody: `GetInvoicesAsync`, `GetInvoiceDetailsAsync`, `GetInvoiceXmlAsync`

2. **KSeFService** (`/Application/Services/KSeFService.cs`)
   - Implementacja serwisu KSeF
   - Wykorzystuje bibliotekę `Soneta.KSeF.Client`
   - Obsługuje sesje, wyszukiwanie i pobieranie faktur

3. **Modele** (`/Application/Models/KSeF/`)
   - `KSeFInvoicesRequest` - request do wyszukiwania faktur
   - `KSeFInvoicesResponse` - odpowiedź z listą faktur
   - `KSeFInvoiceDetails` - szczegóły faktury
   - `KSeFInvoiceItem` - podstawowe info o fakturze
   - `KSeFClientOptions` - opcje konfiguracji

4. **Query** (`/Application/Queries/Accounting/GetKSeFInvoicesQuery.cs`)
   - Handler do pobierania faktur przez API
   - Obsługuje filtrowanie, sortowanie i paginację

5. **Controller** (`/Api/Controllers/AccountingController.cs`)
   - Endpoint: `GET /api/accounting`
   - Zwraca faktury z KSeF

## Konfiguracja

### appsettings.json

```json
{
  "KSeF": {
    "BaseUrl": "https://ksef-test.mf.gov.pl/",
    "CustomHeaders": {
      "X-Custom-Header": "value"
    }
  }
}
```

### Środowiska KSeF

- **TEST**: `https://ksef-test.mf.gov.pl/`
- **DEMO**: `https://ksef-demo.mf.gov.pl/`
- **PRODUKCJA**: `https://ksef.mf.gov.pl/`

## Użycie

### Pobieranie faktur zakupu

```http
GET /api/accounting?subjectNip=1234567890&dateFrom=2024-01-01&dateTo=2024-12-31&invoiceType=Purchase&pageNumber=1&pageSize=50
```

### Pobieranie faktur sprzedaży

```http
GET /api/accounting?subjectNip=1234567890&dateFrom=2024-01-01&dateTo=2024-12-31&invoiceType=Sales&pageNumber=1&pageSize=50
```

### Parametry zapytania

- `subjectNip` - NIP kontrahenta (opcjonalny)
- `dateFrom` - Data początkowa (opcjonalny)
- `dateTo` - Data końcowa (opcjonalny)
- `invoiceType` - Typ faktury: `Sales` lub `Purchase` (domyślnie `Purchase`)
- `pageNumber` - Numer strony (domyślnie 1)
- `pageSize` - Rozmiar strony (domyślnie 10)
- `orderBy` - Sortowanie: `InvoiceDate`, `InvoiceNumber`, `GrossAmount`, `SellerName`, `BuyerName`, `ReceivedDate`
- `isDescending` - Kierunek sortowania (domyślnie false)

## TODO - Wymagane implementacje

### 1. Autoryzacja KSeF ⚠️ KRYTYCZNE

Obecnie brak implementacji autoryzacji. Wymagane:

```csharp
// W KSeFService.EnsureSessionAsync()
var sessionRequest = new KsefSessionOpenRequest
{
    // TODO: Konfiguracja autoryzacji
    // Opcje:
    // 1. Token sesyjny
    // 2. Certyfikat kwalifikowany
    // 3. Credentials użytkownika
};

var sessionResponse = await _ksefClient.SessionOpenAsync(
    sessionRequest, 
    cancellationToken);
    
_sessionId = sessionResponse.SessionId;
_sessionToken = sessionResponse.SessionToken;
```

**Kroki do implementacji:**
1. Określić metodę autoryzacji (token/certyfikat)
2. Dodać konfigurację credentials do appsettings
3. Zaimplementować otwieranie sesji
4. Dodać obsługę refresh tokena
5. Implementować zamykanie sesji

### 2. Parsowanie wyników wyszukiwania ⚠️ WYMAGANE

```csharp
// W KSeFService.ParseInvoiceQueryResult()
private List<KSeFInvoiceItem> ParseInvoiceQueryResult(
    KsefInvoiceQueryResultResponse response)
{
    // TODO:
    // 1. Rozpakować ZIP z odpowiedzi
    // 2. Sparsować każdy XML faktury
    // 3. Zmapować na KSeFInvoiceItem
    // 4. Zwrócić listę faktur
}
```

### 3. Parsowanie XML faktury ⚠️ WYMAGANE

```csharp
// W KSeFService.ParseInvoiceXml()
private KSeFInvoiceDetails ParseInvoiceXml(string invoiceXml)
{
    // TODO:
    // 1. Sparsować XML zgodnie ze schematem FA_v2
    // 2. Wyciągnąć wszystkie wymagane pola
    // 3. Zmapować pozycje faktury
    // 4. Zwrócić KSeFInvoiceDetails
    
    // Schemat: https://www.gov.pl/web/kas/struktury-faktur
}
```

### 4. Obsługa błędów KSeF

Dodać obsługę specyficznych błędów KSeF:
- Rate limiting
- Błędy autoryzacji
- Błędy walidacji
- Timeout sesji

### 5. Caching i optymalizacja

- Cache dla tokenów sesji
- Cache dla często pobieranych faktur
- Batch processing dla wielu faktur

### 6. Testy

- Unit testy dla KSeFService
- Integration testy z mock KSeF API
- Testy dla parsowania XML

## Referencje

- [Dokumentacja KSeF API](https://ksef24.com/en/)
- [Biblioteka KSeF.Client](https://github.com/CIRFMF/ksef-client-csharp)
- [Struktury faktur FA_v2](https://www.gov.pl/web/kas/struktury-faktur)
- [Portal KSeF](https://www.gov.pl/web/kas/krajowy-system-e-faktur)

## Status implementacji

✅ Struktura projektu i modele  
✅ Interfejsy i serwisy  
✅ Query handler i controller  
✅ Konfiguracja i DI  
⚠️ Autoryzacja KSeF - **WYMAGANE**  
⚠️ Parsowanie XML faktur - **WYMAGANE**  
⚠️ Parsowanie wyników wyszukiwania - **WYMAGANE**  
❌ Testy  
❌ Dokumentacja API (Swagger)  

## Uwagi

1. Obecnie implementacja używa środowiska testowego KSeF
2. Wymagana jest pełna implementacja autoryzacji przed użyciem w produkcji
3. Parsowanie XML faktur wymaga zgodności ze schematem FA_v2
4. Należy obsłużyć wszystkie możliwe błędy API KSeF
