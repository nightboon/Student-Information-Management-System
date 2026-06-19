using System;

namespace src.services
{
    /// <summary>Null-safe SqlDataReader value converters shared by all services.</summary>
    public static class ServiceMap
    {
        public static string Text(object value)
        {
            return value == null || value == DBNull.Value ? "" : Convert.ToString(value);
        }

        public static int IntValue(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            return Convert.ToInt32(value);
        }

        public static decimal? DecimalValue(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            return Convert.ToDecimal(value);
        }

        public static DateTime? DateValue(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            return Convert.ToDateTime(value);
        }

        public static TimeSpan? TimeValue(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            if (value is TimeSpan) return (TimeSpan)value;
            return TimeSpan.Parse(Convert.ToString(value));
        }
    }
}
