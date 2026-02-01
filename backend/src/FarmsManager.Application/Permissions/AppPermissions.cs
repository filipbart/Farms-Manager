using System.ComponentModel;
using FarmsManager.Application.Attributes;

namespace FarmsManager.Application.Permissions;

/// <summary>
/// Definiuje klucze uprawnień w systemie.
/// </summary>
public static class AppPermissions
{
    [PermissionGroup("Pulpit Główny")]
    public static class Dashboard
    {
        [Description("Dostęp do pulpitu głównego")]
        public const string View = "dashboard:view";
    }

    [PermissionGroup("Księgowość")]
    public static class Accounting
    {
        [Description("Dostęp do modułu Księgowość")]
        public const string View = "accounting:view";

        [Description("Zarządzanie fakturami z KSeF")]
        public const string Manage = "accounting:manage";
    }

    [PermissionGroup("Wstawienia")]
    public static class Insertions
    {
        [Description("Dostęp do wstawień")] public const string View = "insertions:view";
    }

    [PermissionGroup("Sprzedaż")]
    public static class Sales
    {
        [Description("Dostęp do modułu Sprzedaże")]
        public const string View = "sales:view";

        [Description("Przeglądanie faktur sprzedażowych")]
        public const string InvoicesView = "sales:invoices:view";

        [Description("Zarządzanie ustawieniami sprzedaży")]
        public const string SettingsManage = "sales:settings:manage";
    }

    [PermissionGroup("Pasze")]
    public static class Feeds
    {
        [Description("Dostęp do modułu Pasze")]
        public const string View = "feeds:view";

        [Description("Przeglądanie dostaw pasz")]
        public const string DeliveriesView = "feeds:deliveries:view";

        [Description("Przeglądanie cen pasz")] public const string PricesView = "feeds:prices:view";

        [Description("Przeglądanie przelewów za pasze")]
        public const string PaymentsView = "feeds:payments:view";
    }

    [PermissionGroup("Koszty")]
    public static class Expenses
    {
        [Description("Dostęp do modułu Koszty")]
        public const string View = "expenses:view";

        [Description("Przeglądanie kosztów produkcyjnych")]
        public const string ProductionView = "expenses:production:view";

        [Description("Przeglądanie ewidencji zaliczek")]
        public const string AdvancesView = "expenses:advances:view";

        [Description("Przeglądanie kontrahentów")]
        public const string ContractorsView = "expenses:contractors:view";

        [Description("Zarządzanie kontrahentami")]
        public const string ContractorsManage = "expenses:contractors:manage";

        [Description("Przeglądanie typów wydatków")]
        public const string TypesView = "expenses:types:view";

        [Description("Zarządzanie typami wydatków")]
        public const string TypesManage = "expenses:types:manage";
    }

    [PermissionGroup("Dane Produkcyjne")]
    public static class ProductionData
    {
        [Description("Dostęp do danych produkcyjnych")]
        public const string View = "productiondata:view";

        [Description("Przeglądanie upadków i wybrakowań")]
        public const string FailuresView = "productiondata:failures:view";

        [Description("Przeglądanie pozostałej paszy")]
        public const string RemainingFeedView = "productiondata:remainingfeed:view";

        [Description("Przeglądanie paszy przeniesionej")]
        public const string TransferFeedView = "productiondata:transferfeed:view";

        [Description("Przeglądanie pomiarów cotygodniowych")]
        public const string WeeklyMeasurementsView = "productiondata:weeklymeasurements:view";

        [Description("Przeglądanie ważeń (masy ciała)")]
        public const string WeighingsView = "productiondata:weighings:view";

        [Description("Przeglądanie pomiarów upadków")]
        public const string FlockLossView = "productiondata:flockloss:view";

        [Description("Dostęp do IRZplus")] public const string IrzPlusView = "productiondata:irzplus:view";
    }

    [PermissionGroup("Gaz")]
    public static class Gas
    {
        [Description("Dostęp do modułu Gaz")] public const string View = "gas:view";

        [Description("Przeglądanie dostaw gazu")]
        public const string DeliveriesView = "gas:deliveries:view";

        [Description("Przeglądanie zużycia gazu")]
        public const string ConsumptionsView = "gas:consumptions:view";

        [Description("Przeglądanie kontrahentów gazu")]
        public const string ContractorsView = "gas:contractors:view";
    }

    [PermissionGroup("Notatki z Wylęgarni")]
    public static class HatcheryNotes
    {
        [Description("Dostęp do notatek z wylęgarni")]
        public const string View = "hatcherynotes:view";
    }

    [PermissionGroup("Pracownicy")]
    public static class Employees
    {
        [Description("Dostęp do modułu Pracownicy")]
        public const string View = "employees:view";

        [Description("Przeglądanie listy kadr")]
        public const string ListView = "employees:list:view";

        [Description("Przeglądanie rozliczeń wypłat")]
        public const string PayslipsView = "employees:payslips:view";
    }

    [PermissionGroup("Podsumowanie")]
    public static class Summary
    {
        [Description("Dostęp do modułu Podsumowanie")]
        public const string View = "summary:view";

        [Description("Przeglądanie analizy produkcyjnej")]
        public const string ProductionAnalysisView = "summary:productionanalysis:view";

        [Description("Przeglądanie analizy finansowej")]
        public const string FinancialAnalysisView = "summary:financialanalysis:view";
    }

    [PermissionGroup("Dane")]
    public static class Data
    {
        [Description("Dostęp do danych podstawowych (słowników)")]
        public const string View = "data:view";

        [Description("Zarządzanie podmiotami gospodarczymi")] public const string TaxBusinessEntitiesManage = "data:taxbusinessentities:manage";
        [Description("Zarządzanie fermami")] public const string FarmsManage = "data:farms:manage";
        [Description("Zarządzanie kurnikami")] public const string HousesManage = "data:houses:manage";

        [Description("Zarządzanie wylęgarniami")]
        public const string HatcheriesManage = "data:hatcheries:manage";

        [Description("Przeglądanie ubojni")]
        public const string SlaughterhousesView = "data:slaughterhouses:view";

        [Description("Zarządzanie ubojniami")]
        public const string SlaughterhousesManage = "data:slaughterhouses:manage";

        [Description("Zarządzanie zakładami utylizacyjnymi")]
        public const string UtilizationPlantsManage = "data:utilizationplants:manage";
    }

    [PermissionGroup("Ustawienia")]
    public static class Settings
    {
        [Description("Dostęp do Ustawień")] public const string View = "settings:view";

        [PermissionGroup("Zarządzanie Użytkownikami")]
        public static class Users
        {
            [Description("Przeglądanie listy użytkowników")]
            public const string View = "settings:users:view";

            [Description("Zarządzanie użytkownikami (dodawanie, edycja)")]
            public const string Manage = "settings:users:manage";

            [Description("Zarządzanie uprawnieniami innych użytkowników")]
            public const string ManagePermissions = "settings:users:permissions";
        }

        [PermissionGroup("Zarządzanie Cyklami")]
        public static class Cycles
        {
            [Description("Zarządzanie cyklami")] public const string Manage = "settings:cycles:manage";
        }

        [PermissionGroup("Reguły przypisywania faktur")]
        public static class InvoiceRules
        {
            [Description("Zarządzanie regułami przypisywania faktur")] public const string Manage = "settings:invoicerules:manage";
        }
    }
}