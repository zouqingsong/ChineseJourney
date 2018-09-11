using System.Text.RegularExpressions;

namespace ChineseJourney.Common.Validations
{
	public class ContainPWRequirement<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
			var val = value as string;
         
            if(string.IsNullOrEmpty(val))
            {
                return false;
            }

			var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
			var hasMiniMaxChars = new Regex(@".{10,}");

			if (!hasMiniMaxChars.IsMatch(val))
			{
				return false;
			}
			else if (!hasLowerChar.IsMatch(val))
            {
				return false;
            }
			else if (!hasUpperChar.IsMatch(val))
            {
				return false;
            }
			else if (!hasNumber.IsMatch(val))
            {
				return false;
            }
			else if (!hasSymbols.IsMatch(val))
            {
				return false;
            }

			else{
				return true;
			}

       }
    }
}