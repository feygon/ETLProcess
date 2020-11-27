using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ETLProcessFactory.Algorithms
{
    public static class Parse
	{
		public static decimal DecimalParse(string s)
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

		public static float FloatParse(string s)
		{
			try
			{
				// Perform a Regex to only maintain characters which SHOULD be valid
				s = Regex.Replace(s, "[^0123456789.-]", "");
				return float.Parse(s, CultureInfo.InvariantCulture);
			}
			// Always return a 0 instead of an error
			catch (Exception)
			{
				return 0.00f;
			}
		}

		public static int IntParse(string s)
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

		public static string TrimSubstring(string inputString, int startPos, int length)
            => GetSubstring(inputString, startPos, length).Trim();

		public static string GetSubstring(string inputString, int startPos, int length)
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

		public static string Get2DSubstring(
			string[] inputString
			, (int row, int col) startPos
			, (int height, int width) length)
        {
			throw new NotImplementedException();
        }
	}
}