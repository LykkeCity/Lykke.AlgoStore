using Lykke.AlgoStore.Core.Constants;
using System;
using System.Globalization;

namespace Lykke.AlgoStore.Core.Utils
{
    public static class DateTimeFormatValidator
    {
        public static bool IsDateTimeStringValid(string dateTimeToValidate)
        {
            try
            {
                if (!DateTime.TryParseExact(dateTimeToValidate, AlgoStoreConstants.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
