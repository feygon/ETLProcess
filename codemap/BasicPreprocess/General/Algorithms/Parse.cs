using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BasicPreprocess
{
    internal static class Parse
	{
        internal static decimal DecimalParse(string s)
		{
			try
			{
				// Perform a Regex to only maintain characters which SHOULD be valid
				s = Regex.Replace(s, "[^0123456789.-]", "");
				return decimal.Parse(s, CultureInfo.InvariantCulture);
			}
			// Always return a 0 instead of an error
			catch (Exception)
			{
				return 0.00m;
			}
		}

        internal static int IntParse(string s)
		{
			try
			{
				// Perform a Regex to only maintain characters which SHOULD be valid
				s = Regex.Replace(s, "[^0123456789-]", "");
				return int.Parse(s, CultureInfo.InvariantCulture);
			}
			// Always return a 0 instead of an error
			catch (Exception)
			{
				return 0;
			}
		}

        internal static string TrimSubstring(string inputString, int startPos, int length)
            => GetSubstring(inputString, startPos, length).Trim();

        internal static string GetSubstring(string inputString, int startPos, int length)
		{
            if (startPos > inputString.Length)
            {
                return "";
			}
            if (startPos + length > inputString.Length)
            {
                length = inputString.Length - startPos;
			}
            return inputString.Substring(startPos, length);
		}
	}
}
