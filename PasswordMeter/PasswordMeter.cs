using System;
using System.Linq;

namespace PasswordMeter
{
    class PasswordMeter
    {
        public PasswordMeter()
        {
            PunctuationDigits = GetPunctuationCharactersCount();
            SymbolDigits = GetSymbolCharactersCount();
        }

        public void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException();

            else Password = password;   
        }

        private int PunctuationDigits;
        private int SymbolDigits;

        /// <summary>
        /// 2GHz
        /// </summary>
        private long _selectedSpeed = 2000000000;

        private string Password { get; set; }

        private int Length => Password.Length;

        public string SecondsToCrack => GetSecondsToCrackPassword();

        private string GetSecondsToCrackPassword()
        {
            if (PossiblePasswordCombinations == "∞")
                return "∞";

            decimal seconds;
            decimal Combinations = decimal.Parse(PossiblePasswordCombinations);

            seconds = Combinations / _selectedSpeed;

            return seconds.ToString();
        }

        public int CharacterCombinations => GetCharacterCombinations();

        public string PossiblePasswordCombinations
        {
            get
            {
                decimal comb;
                try
                {
                    comb = (decimal)Math.Pow(CharacterCombinations, Length);
                }
                catch (OverflowException)
                {
                    return "∞";
                }

                return comb.ToString();
            }
        }

        private int GetCharacterCombinations()
        {
            int combinations = 0;

            // 1 - Check for letters (incl. upper & lower)
            var letters = Password.Where(c => char.IsLetter(c));
            if (letters.Count() > 0)
            {
                if (letters.Where(c => char.IsUpper(c)).Count() > 0)
                    combinations += 26;
                if (letters.Where(c => char.IsLower(c)).Count() > 0)
                    combinations += 26;
            }

            // 2 - Check for digits 
            if (Password.Where(c => char.IsDigit(c)).Count() > 0)
                combinations += 10;

            // 3 - Check for punctuation
            if (Password.Where(c => char.IsPunctuation(c)).Count() > 0)
                combinations += PunctuationDigits;

            // 3 - Check for symbols
            if (Password.Where(c => char.IsSymbol(c)).Count() > 0)
                combinations += SymbolDigits;

            return combinations;
        }

        private int GetSymbolCharactersCount()
        {
            int count = 0;

            for (int i = 0; i < Int16.MaxValue; i++)
            {
                if (char.IsSymbol((char)i))
                    ++count;
            }

            return count;
        }

        private int GetPunctuationCharactersCount()
        {
            int count = 0;

            for (int i = 0; i < 128; i++)
            {
                if (char.IsPunctuation((char)i))
                    ++count;
            }

            return count;
        }
    }
}
