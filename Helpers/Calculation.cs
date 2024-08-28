using System.Globalization;

namespace Pension.Entities.Helpers
{
    public class Calculation
    {
        private static readonly string[] units = { "Zero", "One", "Two", "Three","Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven","Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                                                    "Seventeen", "Eighteen", "Nineteen" };

        private static readonly string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public static string ConvertAmount(double amount)
        {
            try
            {
                long amount_int = (long)amount;
                long amount_dec = (long)Math.Round((amount - amount_int) * 100);
                if (amount_dec == 0)
                {
                    return Convert(amount_int) + " Only.";
                }
                else
                {
                    return Convert(amount_int) + " Point " + Convert(amount_dec) + " Only.";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string Convert(long i)
        {
            if (i < 20)
            {
                return units[i];
            }
            if (i < 100)
            {
                return tens[i / 10] + ((i % 10 > 0) ? " " + Convert(i % 10) : "");
            }
            if (i < 1000)
            {
                return units[i / 100] + " Hundred"
                        + ((i % 100 > 0) ? " And " + Convert(i % 100) : "");
            }
            if (i < 100000)
            {
                return Convert(i / 1000) + " Thousand "
                + ((i % 1000 > 0) ? " " + Convert(i % 1000) : "");
            }
            if (i < 10000000)
            {
                return Convert(i / 100000) + " Lakh "
                        + ((i % 100000 > 0) ? " " + Convert(i % 100000) : "");
            }
            if (i < 1000000000)
            {
                return Convert(i / 10000000) + " Crore "
                        + ((i % 10000000 > 0) ? " " + Convert(i % 10000000) : "");
            }
            return Convert(i / 1000000000) + " Arab "
                    + ((i % 1000000000 > 0) ? " " + Convert(i % 1000000000) : "");
        }

        public static IEnumerable<(string Month, int Year)> MonthsBetween(DateTime startDate, DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                yield return (
                    dateTimeFormat.GetMonthName(iterator.Month),
                    iterator.Year
                );

                iterator = iterator.AddMonths(1);
            }
        }

        public static List<Tuple<int, int>> GetMonths(DateTime d0, DateTime d1)
        {
            List<DateTime> datemonth = Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
                             .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m)).ToList();
            List<Tuple<int, int>> yearmonth = new();

            foreach (DateTime x in datemonth)
            {
                yearmonth.Add(new Tuple<int, int>(x.Year, x.Month));
            }
            return yearmonth;
        }
    }
    public class NumberToWordsConverter
    {
        private static string[] units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        private static string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static string[] tens = { "", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        private static string[] thousandsGroups = { "", "Thousand", "Million", "Billion" };

        public static string ConvertToWords(decimal number)
        {
            if (number == 0)
                return "Zero";

            string words = "";
            for (int i = 0; number > 0; i++)
            {
                if (number % 1000 != 0)
                {
                    words = GetWordsForGroup((int)(number % 1000)) + thousandsGroups[i] + " " + words;
                }
                number /= 1000;
            }
            words += " Only";
            return words.Trim();
        }

        private static string GetWordsForGroup(int number)
        {
            if (number == 0)
                return "";

            string words = "";

            if (number >= 100)
            {
                words += units[number / 100] + " Hundred ";
                number %= 100;
            }

            if (number >= 10 && number <= 19)
            {
                words += teens[number - 10] + " ";
            }
            else
            {
                words += tens[number / 10] + " ";
                number %= 10;
            }

            if (number > 0)
            {
                words += units[number] + " ";
            }

            return words;
        }
    }
    public class DecimalToWordsConverter
    {
        private static string[] units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        private static string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static string[] tens = { "", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public static string ConvertToWords(decimal number)
        {
            if (number < 0)
                return "Minus " + ConvertToWords(Math.Abs(number));

            string[] parts = number.ToString("F2").Split('.');
            int wholePart = int.Parse(parts[0]);
            int decimalPart = parts.Length > 1 ? int.Parse(parts[1]) : 0;

            if (wholePart < 0 || wholePart >= 1_000_000_000)
                return "Number out of range (0 to 999,999,999)";

            if (decimalPart < 0 || decimalPart >= 100)
                return "Decimal part out of range (0 to 99)";

            string wholePartWords = ConvertToWords(wholePart);
            string decimalPartWords = ConvertToWords(decimalPart);

            string result = wholePartWords;
            if (!string.IsNullOrWhiteSpace(wholePartWords) && !string.IsNullOrWhiteSpace(decimalPartWords))
                result += " and ";
            if (!string.IsNullOrWhiteSpace(decimalPartWords))
                result += $"{decimalPartWords} Cents";

            return result;
        }

        private static string ConvertToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + ConvertToWords(Math.Abs(number));

            string words = "";

            if (number >= 1000)
            {
                words += ConvertToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if (number >= 100)
            {
                words += units[number / 100] + " Hundred ";
                number %= 100;
            }

            if (number >= 10 && number <= 19)
            {
                words += teens[number - 10] + " ";
            }
            else
            {
                words += tens[number / 10] + " ";
                number %= 10;
            }

            if (number > 0)
            {
                words += units[number] + " ";
            }

            return words.Trim();
        }
    }

}