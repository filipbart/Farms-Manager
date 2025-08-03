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
}