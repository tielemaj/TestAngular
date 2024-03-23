using System;

namespace API.Extentions
{
    public static class DateTimeExtentions
    {
        public static int CalculateAge(this DateOnly dod)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dod.Year;
            if (dod > today.AddYears(-age)) age--;
            return age;
        }
    }
}