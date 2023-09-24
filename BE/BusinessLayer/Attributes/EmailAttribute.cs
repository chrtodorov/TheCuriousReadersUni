using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BusinessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return Regex.IsMatch(value.ToString(),
                "^(([^<>()[\\]\\\\.,;:\\s@\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\"]+)*)|(\".+\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$");
        }
    }    
}