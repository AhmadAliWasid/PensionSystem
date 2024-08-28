using System.Text.RegularExpressions;

namespace Pension.Entities.Helpers
{
    public static class UserFormat
    {
        public static string GetCNIC(string? CNIC)
        {
            if (CNIC == null) return "";
            if (CNIC != null)
            {
                return CNIC.Length == 13 ? Regex.Replace(CNIC, @"^(.....)(.......)(.)$", "$1-$2-$3") : CNIC;
            }
            else
            {
                return "N/A";
            }
        }

        public static string GetDate(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                DateTimeOffset dateTimeOffset = dateTime.Value;
                return dateTimeOffset.ToString("dd-MM-yyyy") != "01-01-0001" ? dateTimeOffset.ToString("dd-MM-yyyy") : "";
            }
            else
            {
                return "N/A";
            }
        }

        public static string GetDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy hh:mm tt");
        }

        public static string GetMobile(string number)
        {
            return number.Length == 13 ? Regex.Replace(number, @"^(...)(...)(.......)", "$1-$2-$3") : number;
        }

        public static string GetAccount(string? number)
        {
            if (number == null) return "";
            if (number.Length == 16)
            {
                number = number.Substring(2, 14);
                return Regex.Replace(number, @"^(....)(........)(..)", "$1-$2-$3");
            }
            else if (number.Length == 19)
            {
                return number.Substring(3);
            }
            else
            {
                return number;
            }
        }

        public static IEnumerable<string> Split(this string str, int n)
        {
            if (String.IsNullOrEmpty(str) || n < 1)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < str.Length; i += n)
            {
                yield return str.Substring(i, Math.Min(n, str.Length - i));
            }
        }

        public static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        public static string GetAccountInFull(string number = "")
        {
            number = number.Replace("'", "");
            number = number.Replace(" ", "");
            number = number.Replace("-", "");
            number = number.Replace(",", "");
            int difference = 16 - number.Length;
            string complete = number.PadLeft(number.Length + difference, '0');
            return complete;
        }

        public static string GetForHBL(string number)
        {
            string complete = number.Replace("-", "");
            if (complete.Length == 16)
            {
                return complete.Substring(2, 14);
            }
            else
            {
                return complete;
            }
        }

        public static string GetAmount(decimal Amount)
        {
            if (Amount == 0)
                return " - ";
            return String.Format("{0:n0}", Amount);
        }

        public static string GetMonthYear(DateTime dateTime)
        {
            return dateTime.ToString("MM-yyyy");
        }

        public static string GetMonthYear(DateOnly dateOnly)
        {
            return dateOnly.ToString("MM-yyyy");
        }

        public static string GetLastDate(DateTime dateTime)
        {
            return dateTime.AddMonths(1).AddDays(-1).ToString("dd-MM-yyyy");
        }

        public static DateTime GetLastMonth(DateTime dateTime)
        {
            return dateTime.AddMonths(-1);
        }

        /// <summary>
        /// Get Cheque number in formate of eight digit
        /// </summary>
        /// <param name="chequeNumber"></param>
        /// <returns></returns>
        public static string GetChequeNumber(int chequeNumber)
        {
            return String.Format("{0,22}", chequeNumber.ToString("D8"));
        }

        public static string GetMobilePK(string number)
        {
            string mynumber = number.Replace("+92-", "0");
            return mynumber;
        }

        public static string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }
    }

}