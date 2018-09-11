using System.Linq;

namespace ChineseJourney.Common.Validations
{
    public class IPAddressRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            var str = value as string;
            //  Split string by ".", check that array length is 4
            if (str != null)
            {
                string[] arrOctets = str.Split('.');
                if (arrOctets.Length != 4)
                {
                    return false;
                }

                //Check each substring checking that parses to byte
                byte obyte;
                return arrOctets.All(strOctet => byte.TryParse(strOctet, out obyte));
            }
            return false;
        }
    }
}