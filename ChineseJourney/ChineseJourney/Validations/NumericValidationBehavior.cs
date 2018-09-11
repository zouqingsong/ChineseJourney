using System.Linq;
using Xamarin.Forms;

namespace ChineseJourney.Common.Validations
{
    
    public class NumericValidationBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        public static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {

            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(IsDigit); //Make sure all characters are numbers

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }


        }

        /// <summary>
        /// Check to see if a character is a digit.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character is between <c>0</c> and <c>9</c>.</returns>
        private static bool IsDigit(char c)
        {
            if (c >= 48)
            {
                return c <= 57;
            }

            return false;
        }
    }
}