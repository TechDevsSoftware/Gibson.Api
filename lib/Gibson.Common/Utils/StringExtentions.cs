namespace Gibson.Common.Utils
{   
        public static class StringExtentions
        {
            public static string FirstLetterUpper(this string str)
            {
                var firstLetter = str.ToCharArray()?[0];
                if (firstLetter == null) return null;
                var newFirstLetter = firstLetter.ToString().ToUpperInvariant();
                var strBody = str.Substring(1);
                var result = $"{newFirstLetter}{strBody}";
                return result;
            }
        }

}