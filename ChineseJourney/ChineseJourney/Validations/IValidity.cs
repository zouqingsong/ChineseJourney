using System.Collections.Generic;

namespace ChineseJourney.Common.Validations
{
    public interface IValidity
    {
        bool IsValid { get; set; }
        List<string> Errors { get; set; }
        bool Validate();
    }
}