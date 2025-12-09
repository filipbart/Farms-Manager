using System.Xml.Serialization;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.XmlModels;

/// <summary>
/// Model faktury KSeF zgodny ze schematem FA(3)
/// </summary>
[XmlRoot("Faktura", Namespace = "http://crd.gov.pl/wzor/2023/06/29/12648/")]
public class KSeFInvoiceXml
{
    [XmlElement("Naglowek")]
    public Naglowek Naglowek { get; set; }

    [XmlElement("Podmiot1")]
    public Podmiot1 Podmiot1 { get; set; }

    [XmlElement("Podmiot2")]
    public Podmiot2 Podmiot2 { get; set; }

    [XmlElement("Podmiot3")]
    public Podmiot3 Podmiot3 { get; set; }

    [XmlElement("Fa")]
    public FaDane Fa { get; set; }

    [XmlElement("Stopka")]
    public Stopka Stopka { get; set; }
}

/// <summary>
/// Nagłówek faktury - dane techniczne
/// </summary>
public class Naglowek
{
    [XmlElement("KodFormularza")]
    public KodFormularza KodFormularza { get; set; }

    [XmlElement("WariantFormularza")]
    public int WariantFormularza { get; set; }

    [XmlElement("DataWytworzeniaFa")]
    public DateTime DataWytworzeniaFa { get; set; }

    [XmlElement("SystemInfo")]
    public string SystemInfo { get; set; }
}

public class KodFormularza
{
    [XmlAttribute("kodSystemowy")]
    public string KodSystemowy { get; set; }

    [XmlAttribute("wersjaSchemy")]
    public string WersjaSchemy { get; set; }

    [XmlText]
    public string Value { get; set; }
}

/// <summary>
/// Podmiot1 - Sprzedawca
/// </summary>
public class Podmiot1
{
    [XmlElement("DaneIdentyfikacyjne")]
    public DaneIdentyfikacyjnePodmiotu DaneIdentyfikacyjne { get; set; }

    [XmlElement("Adres")]
    public AdresPodmiotu Adres { get; set; }

    [XmlElement("DaneKontaktowe")]
    public DaneKontaktowe DaneKontaktowe { get; set; }
}

/// <summary>
/// Podmiot2 - Nabywca
/// </summary>
public class Podmiot2
{
    [XmlElement("DaneIdentyfikacyjne")]
    public DaneIdentyfikacyjnePodmiotu DaneIdentyfikacyjne { get; set; }

    [XmlElement("Adres")]
    public AdresPodmiotu Adres { get; set; }

    [XmlElement("DaneKontaktowe")]
    public DaneKontaktowe DaneKontaktowe { get; set; }
}

/// <summary>
/// Podmiot3 - Dodatkowy podmiot (faktor, płatnik, itp.)
/// </summary>
public class Podmiot3
{
    [XmlElement("DaneIdentyfikacyjne")]
    public DaneIdentyfikacyjnePodmiotu DaneIdentyfikacyjne { get; set; }

    [XmlElement("Adres")]
    public AdresPodmiotu Adres { get; set; }

    [XmlElement("Rola")]
    public string Rola { get; set; }
}

public class DaneIdentyfikacyjnePodmiotu
{
    [XmlElement("NIP")]
    public string NIP { get; set; }

    [XmlElement("Nazwa")]
    public string Nazwa { get; set; }

    [XmlElement("NrVatUE")]
    public string NrVatUE { get; set; }

    [XmlElement("NrID")]
    public string NrID { get; set; }
}

public class AdresPodmiotu
{
    [XmlElement("KodKraju")]
    public string KodKraju { get; set; }

    [XmlElement("AdresL1")]
    public string AdresL1 { get; set; }

    [XmlElement("AdresL2")]
    public string AdresL2 { get; set; }

    [XmlElement("GLN")]
    public string GLN { get; set; }
}

public class DaneKontaktowe
{
    [XmlElement("Email")]
    public string Email { get; set; }

    [XmlElement("Telefon")]
    public string Telefon { get; set; }
}

/// <summary>
/// Dane faktury - szczegóły
/// </summary>
public class FaDane
{
    /// <summary>
    /// Rodzaj faktury (VAT, KOR, ZAL, etc.)
    /// </summary>
    [XmlElement("RodzajFaktury")]
    public string RodzajFaktury { get; set; }

    /// <summary>
    /// P_1 - Data wystawienia
    /// </summary>
    [XmlElement("P_1")]
    public DateTime P_1 { get; set; }

    /// <summary>
    /// P_1M - Miejsce wystawienia
    /// </summary>
    [XmlElement("P_1M")]
    public string P_1M { get; set; }

    /// <summary>
    /// P_2 - Numer faktury
    /// </summary>
    [XmlElement("P_2")]
    public string P_2 { get; set; }

    /// <summary>
    /// P_6 - Data sprzedaży / wykonania usługi
    /// </summary>
    [XmlElement("P_6")]
    public DateTime? P_6 { get; set; }

    /// <summary>
    /// P_13_1 - Suma wartości netto sprzedaży ze stawką 23%
    /// </summary>
    [XmlElement("P_13_1")]
    public decimal? P_13_1 { get; set; }

    /// <summary>
    /// P_14_1 - Kwota VAT od sprzedaży ze stawką 23%
    /// </summary>
    [XmlElement("P_14_1")]
    public decimal? P_14_1 { get; set; }

    /// <summary>
    /// P_13_2 - Suma wartości netto sprzedaży ze stawką 8%
    /// </summary>
    [XmlElement("P_13_2")]
    public decimal? P_13_2 { get; set; }

    /// <summary>
    /// P_14_2 - Kwota VAT od sprzedaży ze stawką 8%
    /// </summary>
    [XmlElement("P_14_2")]
    public decimal? P_14_2 { get; set; }

    /// <summary>
    /// P_13_3 - Suma wartości netto sprzedaży ze stawką 5%
    /// </summary>
    [XmlElement("P_13_3")]
    public decimal? P_13_3 { get; set; }

    /// <summary>
    /// P_14_3 - Kwota VAT od sprzedaży ze stawką 5%
    /// </summary>
    [XmlElement("P_14_3")]
    public decimal? P_14_3 { get; set; }

    /// <summary>
    /// P_13_4 - Suma wartości netto sprzedaży ze stawką 0%
    /// </summary>
    [XmlElement("P_13_4")]
    public decimal? P_13_4 { get; set; }

    /// <summary>
    /// P_13_5 - Suma wartości sprzedaży zwolnionej
    /// </summary>
    [XmlElement("P_13_5")]
    public decimal? P_13_5 { get; set; }

    /// <summary>
    /// P_13_6_1 - Suma wartości sprzedaży nie podlegającej opodatkowaniu
    /// </summary>
    [XmlElement("P_13_6_1")]
    public decimal? P_13_6_1 { get; set; }

    /// <summary>
    /// P_15 - Kwota należności ogółem (brutto)
    /// </summary>
    [XmlElement("P_15")]
    public decimal P_15 { get; set; }

    /// <summary>
    /// Waluta faktury
    /// </summary>
    [XmlElement("KodWaluty")]
    public string KodWaluty { get; set; }

    /// <summary>
    /// Pozycje faktury
    /// </summary>
    [XmlElement("FaWiersz")]
    public List<FaWiersz> FaWiersze { get; set; } = new();

    /// <summary>
    /// Dane płatności
    /// </summary>
    [XmlElement("Platnosc")]
    public Platnosc Platnosc { get; set; }
}

/// <summary>
/// Pozycja faktury
/// </summary>
public class FaWiersz
{
    /// <summary>
    /// Numer wiersza
    /// </summary>
    [XmlElement("NrWierszaFa")]
    public int NrWierszaFa { get; set; }

    /// <summary>
    /// Nazwa towaru/usługi
    /// </summary>
    [XmlElement("P_7")]
    public string P_7 { get; set; }

    /// <summary>
    /// Jednostka miary
    /// </summary>
    [XmlElement("P_8A")]
    public string P_8A { get; set; }

    /// <summary>
    /// Ilość
    /// </summary>
    [XmlElement("P_8B")]
    public decimal? P_8B { get; set; }

    /// <summary>
    /// Cena jednostkowa netto
    /// </summary>
    [XmlElement("P_9A")]
    public decimal? P_9A { get; set; }

    /// <summary>
    /// Cena jednostkowa brutto
    /// </summary>
    [XmlElement("P_9B")]
    public decimal? P_9B { get; set; }

    /// <summary>
    /// Wartość netto
    /// </summary>
    [XmlElement("P_11")]
    public decimal? P_11 { get; set; }

    /// <summary>
    /// Wartość brutto
    /// </summary>
    [XmlElement("P_11A")]
    public decimal? P_11A { get; set; }

    /// <summary>
    /// Stawka VAT (np. 23, 8, 5, 0)
    /// </summary>
    [XmlElement("P_12")]
    public decimal? P_12 { get; set; }

    /// <summary>
    /// PKWiU
    /// </summary>
    [XmlElement("PKWiU")]
    public string PKWiU { get; set; }

    /// <summary>
    /// CN (kod celny)
    /// </summary>
    [XmlElement("CN")]
    public string CN { get; set; }

    /// <summary>
    /// GTU (grupa towarowa)
    /// </summary>
    [XmlElement("GTU")]
    public string GTU { get; set; }
}

/// <summary>
/// Dane płatności
/// </summary>
public class Platnosc
{
    /// <summary>
    /// Termin płatności
    /// </summary>
    [XmlElement("TerminPlatnosci")]
    public TerminPlatnosci TerminPlatnosci { get; set; }

    /// <summary>
    /// Forma płatności (1 - gotówka, 2 - przelew, etc.)
    /// </summary>
    [XmlElement("FormaPlatnosci")]
    public string FormaPlatnosci { get; set; }

    /// <summary>
    /// Rachunek bankowy
    /// </summary>
    [XmlElement("RachunekBankowy")]
    public List<RachunekBankowy> RachunkiBankowe { get; set; } = new();
}

public class TerminPlatnosci
{
    [XmlElement("Termin")]
    public DateTime? Termin { get; set; }

    [XmlElement("TerminOpis")]
    public string TerminOpis { get; set; }
}

public class RachunekBankowy
{
    [XmlElement("NrRB")]
    public string NrRB { get; set; }

    [XmlElement("NazwaBanku")]
    public string NazwaBanku { get; set; }

    [XmlElement("OpisRachunku")]
    public string OpisRachunku { get; set; }
}

/// <summary>
/// Stopka faktury
/// </summary>
public class Stopka
{
    [XmlElement("Informacje")]
    public Informacje Informacje { get; set; }
}

public class Informacje
{
    [XmlElement("StopkaFaktury")]
    public string StopkaFaktury { get; set; }
}
