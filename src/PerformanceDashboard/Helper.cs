using System;

namespace PerformanceDashboard
{
    public static class Helper
    {
        public static string ToTurkishString(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }
    }
}