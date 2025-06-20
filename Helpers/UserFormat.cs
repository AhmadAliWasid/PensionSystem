using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebAPI.Helpers
{
    public static class UserFormat
    {
        public static string GetBPS(int bps)
        {
            return bps.ToString("D2"); // Formats single digits as two-digit numbers (01-09)
        }
        public static string GetStringFormat(string text, int requiredLength)
        {
            if (text.Length >= requiredLength) return text;
            int totalPadding = requiredLength - text.Length;
            int padLeft = totalPadding / 2;
            int padRight = totalPadding - padLeft;
            return text.PadLeft(text.Length + padLeft, '_').PadRight(requiredLength, '_');
        }

        public static string GetCNIC(string? CNIC) =>
            string.IsNullOrEmpty(CNIC) ? "" :
            CNIC.Length == 13 ? Regex.Replace(CNIC, @"^(.....)(.......)(.)$", "$1-$2-$3") : CNIC;

        public static string GetCurrentDate() =>
            DateTime.UtcNow.AddHours(5).ToString("yyyy-MM-dd");

        public static string GetDate(DateTime? dateTime) =>
            dateTime.HasValue && dateTime.Value.ToString("dd-MM-yyyy") != "01-01-0001"
                ? dateTime.Value.ToString("dd-MM-yyyy")
                : "N/A";

        public static string GetDate(DateOnly dateTime) =>
            dateTime.ToString("dd-MM-yyyy");

        public static string GetDateTime(DateTime dateTime) =>
            dateTime.ToString("dd-MM-yyyy hh:mm tt");

        public static string GetMobile(string number) =>
            number.Length == 13 ? Regex.Replace(number, @"^(...)(...)(.......)", "$1-$2-$3") : number;

        public static string GetAccount(string? number)
        {
            if (string.IsNullOrEmpty(number)) return "";
            return number.Length switch
            {
                16 => Regex.Replace(number.Substring(2, 14), @"^(....)(........)(..)", "$1-$2-$3"),
                19 => number.Substring(3),
                _ => number
            };
        }

        public static IEnumerable<string> Split(this string str, int n)
        {
            if (string.IsNullOrEmpty(str) || n < 1) throw new ArgumentException();
            for (int i = 0; i < str.Length; i += n)
                yield return str.Substring(i, Math.Min(n, str.Length - i));
        }

        public static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        public static string GetAccountInFull(string number = "") =>
            number.Replace("'", "").Replace(" ", "").Replace("-", "").Replace(",", "").PadLeft(16, '0');

        public static string GetForHBL(string number)
        {
            var complete = number.Replace("-", "");
            return complete.Length == 16 ? complete.Substring(2, 14) : complete;
        }

        public static string GetAmount(decimal Amount) =>
            Amount == 0 ? " - " : $"{Amount:n0}";

        public static string GetMonthYear(DateTime dateTime) =>
            dateTime.ToString("MM-yyyy");

        public static string GetMonthYear(DateOnly dateTime) =>
            dateTime.ToString("MM-yyyy");

        public static string GetMonthYearWWF(DateOnly dateTime) =>
            dateTime.Day == 1 || dateTime.AddDays(1).Month != dateTime.Month
                ? dateTime.ToString("M/yy")
                : dateTime.ToString("d/M/yy");

        public static string GetLastDate(DateTime dateTime) =>
            dateTime.AddMonths(1).AddDays(-1).ToString("dd-MM-yyyy");

        public static DateTime GetLastMonth(DateTime dateTime) =>
            dateTime.AddMonths(-1);

        /// <summary>
        /// Get Cheque number in format of eight digits, right-aligned in 22 spaces
        /// </summary>
        public static string GetChequeNumber(int chequeNumber) =>
            $"{chequeNumber:D8}".PadLeft(22);

        public static string GetMobilePK(string number) =>
            number.Replace("+92-", "0");

        public static string EnsureCorrectFilename(string filename) =>
            filename.Contains("\\") ? filename[(filename.LastIndexOf("\\") + 1)..] : filename;
    }
}