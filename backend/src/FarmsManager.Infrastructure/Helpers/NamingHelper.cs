using System.Text;

namespace FarmsManager.Infrastructure.Helpers;

public static class NamingHelper
{
    public static string PascalCaseToSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var builder = new StringBuilder();

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];

            if (char.IsUpper(c))
            {
                if (i > 0 && (char.IsLower(text[i - 1]) || (i + 1 < text.Length && char.IsLower(text[i + 1]))))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}