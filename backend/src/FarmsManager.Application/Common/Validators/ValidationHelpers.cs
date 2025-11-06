using System.Text.RegularExpressions;

namespace FarmsManager.Application.Common.Validators;

public static class ValidationHelpers
{
    public static bool IsValidNip(string nip)
    {
        if (string.IsNullOrWhiteSpace(nip))
            return false;

        nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();

        if (nip.Length != 10 || !nip.All(char.IsDigit))
            return false;

        int[] weights = [6, 5, 7, 2, 3, 4, 5, 6, 7];
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += (nip[i] - '0') * weights[i];
        }

        return sum % 11 == nip[9] - '0';
    }

    /// <summary>
    /// Waliduje format numeru producenta lub IRZ w formacie "liczba-liczba" (np. 00011233-123)
    /// </summary>
    public static bool IsValidProducerOrIrzNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return false;

        // Format: liczba-liczba (np. 00011233-123)
        var regex = new Regex(@"^\d+-\d+$");
        return regex.IsMatch(number.Trim());
    }
}