using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Core.Extensions
{
    public static class StringExtensions
    {
        private static readonly CultureInfo TurkishCulture = new CultureInfo("tr-TR");

        /// <summary>
        /// Converts string to Turkish Title Case (e.g. "ayşe ışık" -> "Ayşe Işık", "İSMAİL" -> "İsmail", "ömer" -> "Ömer")
        /// </summary>
        public static string ToTurkishTitleCase(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string cleaned = Regex.Replace(text.Trim(), @"\s+", " ");
            string lower = cleaned.ToLower(TurkishCulture);
            return TurkishCulture.TextInfo.ToTitleCase(lower);
        }

        /// <summary>
        /// Converts string to Turkish Upper Case (e.g. "ışık" -> "IŞIK", "ismail" -> "İSMAİL")
        /// </summary>
        public static string ToTurkishUpper(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Trim().ToUpper(TurkishCulture);
        }

        /// <summary>
        /// Converts string to Turkish Lower Case (e.g. "IŞIK" -> "ışık", "İSMAİL" -> "ismail")
        /// </summary>
        public static string ToTurkishLower(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Trim().ToLower(TurkishCulture);
        }

        /// <summary>
        /// Masks the name (e.g. "John Doe" -> "J*** D***")
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string MaskName(this string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var maskedParts = new System.Collections.Generic.List<string>();

            foreach (var part in parts)
            {
                if (part.Length > 1)
                {
                    string masked = part[0] + new string('*', part.Length - 1);
                    maskedParts.Add(masked);
                }
                else
                {
                    maskedParts.Add(part);
                }
            }

            return string.Join(" ", maskedParts);
        }
    }
}
