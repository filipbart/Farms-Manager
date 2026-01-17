# Farms-Manager - Tech Stack

## Frontend

| Technologia | Wersja | Zastosowanie |
|-------------|--------|--------------|
| **React** | 19.1 | Framework UI |
| **TypeScript** | 5.8 | Typowanie statyczne |
| **Vite** | 6.3 | Bundler i dev server |
| **React Router** | 7.6 | Routing |
| **TanStack Query** | 5.85 | Zarządzanie stanem serwera, cache |

### UI/Styling

| Technologia | Wersja | Zastosowanie |
|-------------|--------|--------------|
| **MUI (Material-UI)** | 7.1 | Komponenty UI |
| **MUI X Data Grid Premium** | 8.9 | Zaawansowane tabele |
| **MUI X Charts** | 8.10 | Wykresy |
| **MUI X Date Pickers** | 8.5 | Wybór dat |
| **TailwindCSS** | 4.1 | Utility-first CSS |
| **Emotion** | 11.14 | CSS-in-JS |

### Formularze i walidacja

| Technologia | Wersja | Zastosowanie |
|-------------|--------|--------------|
| **React Hook Form** | 7.58 | Zarządzanie formularzami |

### Utilities

| Technologia | Wersja | Zastosowanie |
|-------------|--------|--------------|
| **Axios** | 1.10 | HTTP client |
| **Day.js** | 1.11 | Manipulacja datami |
| **jwt-decode** | 4.0 | Dekodowanie tokenów JWT |
| **react-toastify** | 11.0 | Notyfikacje |
| **@dnd-kit** | 6.3 | Drag and drop |

---

## Backend

| Technologia | Zastosowanie |
|-------------|--------------|
| **.NET** | Framework |
| **Clean Architecture** | Wzorzec architektoniczny |

### Struktura projektu

```
backend/src/
├── FarmsManager.Api/           # Warstwa API (kontrolery, middleware)
├── FarmsManager.Application/   # Logika biznesowa, CQRS, walidacja
├── FarmsManager.Domain/        # Encje, agregaty, reguły domenowe
├── FarmsManager.Infrastructure/# Repozytoria, integracje zewnętrzne
├── FarmsManager.Shared/        # Wspólne utilities
└── FarmsManager.HostBuilder/   # Konfiguracja hosta
```

### Integracje

- **KSeF** - Krajowy System e-Faktur (w trakcie integracji)
- **MinIO** - Object storage dla plików/faktur

---

## Infrastruktura

| Technologia | Zastosowanie |
|-------------|--------------|
| **Docker** | Konteneryzacja |
| **Docker Compose** | Orkiestracja lokalna |
| **MinIO** | Storage plików |

---

## Narzędzia deweloperskie

| Technologia | Zastosowanie |
|-------------|--------------|
| **ESLint** | Linting JavaScript/TypeScript |
| **TypeScript ESLint** | Reguły TS dla ESLint |
| **GitHub Actions** | CI/CD |

---

## Konwencje

### Frontend
- **Komponenty**: PascalCase (`ExpenseProductionPage.tsx`)
- **Hooki**: camelCase z prefixem `use` (`useExpenseProductions.ts`)
- **Serwisy**: PascalCase z sufiksem `Service` (`ExpensesService.ts`)
- **Modele**: PascalCase (`ExpenseProductionListModel`)

### Backend
- **Clean Architecture** z wyraźnym podziałem warstw
- **CQRS** - separacja komend i zapytań w warstwie Application
