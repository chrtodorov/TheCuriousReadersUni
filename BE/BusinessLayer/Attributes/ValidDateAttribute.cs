using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            DateTime dateValue = Convert.ToDateTime(value);
            var now = DateTime.UtcNow;
            return dateValue >= new DateTime(now.Year, now.Month, now.Day);
        }
    }
}