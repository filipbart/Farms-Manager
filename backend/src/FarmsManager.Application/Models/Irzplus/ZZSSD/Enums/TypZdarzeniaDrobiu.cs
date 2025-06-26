using System.ComponentModel;
using System.Runtime.Serialization;

namespace FarmsManager.Application.Models.Irzplus.ZZSSD.Enums;

public enum TypZdarzeniaDrobiu
{
    [EnumMember(Value = "ZADAK")]
    [Description("Aktualizacja")]
    Aktualizacja,

    [EnumMember(Value = "ZADAKAR")]
    [Description("Aktualizacja (archiwizacja)")]
    AktualizacjaArchiwizacja,

    [EnumMember(Value = "ZZSSDPA")]
    [Description("Padnięcie/Stłuczki")]
    Padniecie,

    [EnumMember(Value = "ZRDPR")]
    [Description("Pierwsza rejestracja")]
    PierwszaRejestracja,

    [EnumMember(Value = "ZZSSDPI")]
    [Description("Przemieszczenie do państwa niebędącego członkiem UE")]
    PrzemieszczeniePozaUE,

    [EnumMember(Value = "ZZSSDPU")]
    [Description("Przemieszczenie do państwa UE")]
    PrzemieszczenieDoUE,

    [EnumMember(Value = "ZRDPI")]
    [Description("Przemieszczenie z państwa niebędącego członkiem UE")]
    PrzemieszczenieZPozaUE,

    [EnumMember(Value = "ZRDPU")]
    [Description("Przemieszczenie z państwa UE")]
    PrzemieszczenieZUE,

    [EnumMember(Value = "ZURDUR")]
    [Description("Przybycie do rzeźni i ubój")]
    PrzybycieDoRzezni,

    [EnumMember(Value = "ZZSSDPD")]
    [Description("Przybycie (zwiększenie)")]
    Przybycie,

    [EnumMember(Value = "ZURDUW")]
    [Description("Ubój po wwozie")]
    UbojPoWwozie,

    [EnumMember(Value = "ZZSSDUG")]
    [Description("Ubój w gospodarstwie")]
    UbojWGospodarstwie,

    [EnumMember(Value = "ZUZDUZ")]
    [Description("Unieszkodliwienie zwłok")]
    Unieszkodliwienie,

    [EnumMember(Value = "ZZSSDPZ")]
    [Description("Wybycie (zmniejszenie)")]
    Wybycie,

    [EnumMember(Value = "ZZSSDZA")]
    [Description("Zabicie z nakazu PLW")]
    ZabicieZNakazuPLW,

    [EnumMember(Value = "ZZSSDUK")]
    [Description("Zamknięcie cyklu")]
    ZamkniecieCyklu
}