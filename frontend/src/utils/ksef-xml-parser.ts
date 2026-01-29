import type {
  KSeFParsedXmlData,
  KSeFPartyData,
  KSeFLineItem,
  KSeFVatBreakdown,
  KSeFPaymentData,
  KSeFBankAccount,
  KSeFAdditionalDescription,
} from "../models/accounting/ksef-invoice";

const getElementText = (
  parent: Element | Document,
  tagName: string
): string | undefined => {
  const el = parent.getElementsByTagName(tagName)[0];
  return el?.textContent?.trim() || undefined;
};

const getElementNumber = (
  parent: Element | Document,
  tagName: string
): number | undefined => {
  const text = getElementText(parent, tagName);
  if (!text) return undefined;
  const num = parseFloat(text.replace(",", "."));
  return isNaN(num) ? undefined : num;
};

const parsePartyData = (element: Element | null): KSeFPartyData | undefined => {
  if (!element) return undefined;

  const daneId = element.getElementsByTagName("DaneIdentyfikacyjne")[0];
  const adres = element.getElementsByTagName("Adres")[0];
  const kontakt = element.getElementsByTagName("DaneKontaktowe")[0];

  return {
    nip: daneId ? getElementText(daneId, "NIP") : undefined,
    name: daneId ? getElementText(daneId, "Nazwa") : undefined,
    vatEuNumber: daneId ? getElementText(daneId, "NrVatUE") : undefined,
    idNumber: daneId ? getElementText(daneId, "NrID") : undefined,
    address: adres
      ? {
          countryCode: getElementText(adres, "KodKraju"),
          addressLine1: getElementText(adres, "AdresL1"),
          addressLine2: getElementText(adres, "AdresL2"),
          gln: getElementText(adres, "GLN"),
        }
      : undefined,
    contact: kontakt
      ? {
          email: getElementText(kontakt, "Email"),
          phone: getElementText(kontakt, "Telefon"),
        }
      : undefined,
  };
};

const parseLineItems = (faElement: Element | null): KSeFLineItem[] => {
  if (!faElement) return [];

  const wiersze = faElement.getElementsByTagName("FaWiersz");
  const items: KSeFLineItem[] = [];

  for (let i = 0; i < wiersze.length; i++) {
    const wiersz = wiersze[i];
    items.push({
      lineNumber: getElementNumber(wiersz, "NrWierszaFa") || i + 1,
      name: getElementText(wiersz, "P_7"),
      unit: getElementText(wiersz, "P_8A"),
      quantity: getElementNumber(wiersz, "P_8B"),
      unitPriceNet: getElementNumber(wiersz, "P_9A"),
      unitPriceGross: getElementNumber(wiersz, "P_9B"),
      netAmount: getElementNumber(wiersz, "P_11"),
      grossAmount: getElementNumber(wiersz, "P_11A"),
      vatRate: getElementNumber(wiersz, "P_12"),
      pkwiu: getElementText(wiersz, "PKWiU"),
      cn: getElementText(wiersz, "CN"),
      gtu: getElementText(wiersz, "GTU"),
    });
  }

  return items;
};

const parseVatBreakdown = (faElement: Element | null): KSeFVatBreakdown[] => {
  if (!faElement) return [];

  const breakdown: KSeFVatBreakdown[] = [];

  const rates = [
    { rate: "23%", netField: "P_13_1", vatField: "P_14_1" },
    { rate: "8%", netField: "P_13_2", vatField: "P_14_2" },
    { rate: "5%", netField: "P_13_3", vatField: "P_14_3" },
    { rate: "0%", netField: "P_13_4", vatField: null },
    { rate: "zw.", netField: "P_13_5", vatField: null },
    { rate: "np.", netField: "P_13_6_1", vatField: null },
  ];

  for (const r of rates) {
    const netAmount = getElementNumber(faElement, r.netField);
    if (netAmount !== undefined && netAmount > 0) {
      breakdown.push({
        rate: r.rate,
        netAmount,
        vatAmount: r.vatField
          ? getElementNumber(faElement, r.vatField)
          : undefined,
      });
    }
  }

  return breakdown;
};

const parseAdditionalDescriptions = (
  faElement: Element | null
): KSeFAdditionalDescription[] => {
  if (!faElement) return [];

  const opisy = faElement.getElementsByTagName("DodatkowyOpis");
  const descriptions: KSeFAdditionalDescription[] = [];

  for (let i = 0; i < opisy.length; i++) {
    const opis = opisy[i];
    descriptions.push({
      key: getElementText(opis, "Klucz"),
      value: getElementText(opis, "Wartosc"),
    });
  }

  return descriptions;
};

const parseBankAccounts = (
  platnoscElement: Element | null
): KSeFBankAccount[] => {
  if (!platnoscElement) return [];

  const rachunki = platnoscElement.getElementsByTagName("RachunekBankowy");
  const accounts: KSeFBankAccount[] = [];

  for (let i = 0; i < rachunki.length; i++) {
    const rachunek = rachunki[i];
    accounts.push({
      accountNumber: getElementText(rachunek, "NrRB"),
      bankName: getElementText(rachunek, "NazwaBanku"),
      description: getElementText(rachunek, "OpisRachunku"),
    });
  }

  return accounts;
};

const parsePaymentData = (
  platnoscElement: Element | null
): KSeFPaymentData | undefined => {
  if (!platnoscElement) return undefined;

  const terminy = platnoscElement.getElementsByTagName("TerminPlatnosci");
  let dueDate: string | undefined;
  if (terminy.length > 0) {
    dueDate = getElementText(terminy[0], "Termin");
  }

  const formaPlatnosci = getElementText(platnoscElement, "FormaPlatnosci");
  const paymentMethodMap: Record<string, string> = {
    "1": "Gotówka",
    "2": "Przelew",
    "3": "Karta",
    "4": "Bon",
    "5": "Barterowa",
    "6": "Inna",
  };

  return {
    isPaid: getElementText(platnoscElement, "Zaplacono") === "1",
    paymentDate: getElementText(platnoscElement, "DataZaplaty"),
    dueDate,
    paymentMethod: formaPlatnosci
      ? paymentMethodMap[formaPlatnosci] || formaPlatnosci
      : undefined,
    paymentMethodOther: getElementText(platnoscElement, "PlatnoscInna"),
    paymentDescription: getElementText(platnoscElement, "OpisPlatnosci"),
    bankAccounts: parseBankAccounts(platnoscElement),
  };
};

export const parseKSeFInvoiceXml = (
  xmlString: string | null | undefined
): KSeFParsedXmlData | null => {
  if (!xmlString) return null;

  try {
    const parser = new DOMParser();
    const doc = parser.parseFromString(xmlString, "text/xml");

    const parseError = doc.getElementsByTagName("parsererror")[0];
    if (parseError) {
      console.error("XML parse error:", parseError.textContent);
      return null;
    }

    const naglowek = doc.getElementsByTagName("Naglowek")[0];
    const podmiot1 = doc.getElementsByTagName("Podmiot1")[0];
    const podmiot2 = doc.getElementsByTagName("Podmiot2")[0];
    const podmiot3 = doc.getElementsByTagName("Podmiot3")[0];
    const fa = doc.getElementsByTagName("Fa")[0];
    const platnosc = fa?.getElementsByTagName("Platnosc")[0];
    const stopka = doc.getElementsByTagName("Stopka")[0];

    const kodFormularza = naglowek?.getElementsByTagName("KodFormularza")[0];

    return {
      header: naglowek
        ? {
            formCode: kodFormularza?.textContent?.trim(),
            formVariant: getElementNumber(naglowek, "WariantFormularza"),
            createdDate: getElementText(naglowek, "DataWytworzeniaFa"),
            systemInfo: getElementText(naglowek, "SystemInfo"),
          }
        : undefined,
      seller: parsePartyData(podmiot1),
      buyer: parsePartyData(podmiot2),
      thirdParty: podmiot3
        ? {
            ...parsePartyData(podmiot3),
            role: getElementText(podmiot3, "Rola"),
          }
        : undefined,
      invoiceData: fa
        ? {
            invoiceType: getElementText(fa, "RodzajFaktury"),
            issueDate: getElementText(fa, "P_1"),
            issuePlace: getElementText(fa, "P_1M"),
            invoiceNumber: getElementText(fa, "P_2"),
            saleDate: getElementText(fa, "P_6"),
            currency: getElementText(fa, "KodWaluty"),
            grossTotal: getElementNumber(fa, "P_15"),
            vatBreakdown: parseVatBreakdown(fa),
          }
        : undefined,
      lineItems: parseLineItems(fa),
      payment: parsePaymentData(platnosc),
      footer: stopka
        ? getElementText(
            stopka.getElementsByTagName("Informacje")[0] || stopka,
            "StopkaFaktury"
          )
        : undefined,
      additionalDescriptions: parseAdditionalDescriptions(fa),
    };
  } catch (error) {
    console.error("Error parsing KSeF XML:", error);
    return null;
  }
};
