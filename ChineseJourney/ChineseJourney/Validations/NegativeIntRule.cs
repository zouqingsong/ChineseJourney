namespace ChineseJourney.Common.Validations
{
    public class NegativeIntRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            int val = 0;
            if (value is string)
            {
                val = int.TryParse(value as string, out val) ? val : 0;
            }
            else if(value is int)
            {
                val = int.Parse(value.ToString());
            }
            return val >= 0;
        }
    }
}