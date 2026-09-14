using System;
using System.Text.RegularExpressions;

namespace Core.Extensions
{
    public static class StringExtensions
    {
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
