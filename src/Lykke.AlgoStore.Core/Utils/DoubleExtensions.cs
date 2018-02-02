using System.Globalization;

namespace Lykke.AlgoStore.Core.Utils
{
    public static class DoubleExtensions
    {
        public static int GetAccuracy(this double number)
        {
            string num = number.ToString(CultureInfo.InvariantCulture);
            string[] nums = num.Split(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            if (nums.Length > 2)
                return -1;
            if (nums.Length == 1)
                return 0;

            return nums[1].Length;
        }
    }
}
