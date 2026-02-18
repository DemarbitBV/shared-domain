using System.Text;
using System.Text.RegularExpressions;

namespace Demarbit.Shared.Domain.Extensions;

/// <summary>
/// Extension methods for string manipulation in domain contexts.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Converts a string to a URL-friendly slug.
    /// Removes diacritics (accents), lowercases, and replaces non-alphanumeric characters with hyphens.
    /// </summary>
    /// <example>
    /// <code>
    /// "Café au Lait".ToSlug()      // "cafe-au-lait"
    /// "Hello World!".ToSlug()      // "hello-world"
    /// "  Trimmed  ".ToSlug()       // "trimmed"
    /// "Ürban Köln".ToSlug()        // "urban-koln"
    /// </code>
    /// </example>
    public static string ToSlug(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        // Normalize and remove diacritics (accents)
        var normalizedString = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var category = char.GetUnicodeCategory(c);
            if (category != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        slug = NonAlphanumericRegex().Replace(slug, "-");
        slug = slug.Trim('-');

        return slug;
    }

    [GeneratedRegex("[^a-z0-9]+", RegexOptions.None, matchTimeoutMilliseconds: 2000)]
    private static partial Regex NonAlphanumericRegex();
}