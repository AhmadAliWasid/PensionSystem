namespace Pension.Entities.Helpers
{
    public class JsonResponseHelper
    {
        /// <summary>
        /// 0 for Error and 1 for any other
        /// </summary>
        public int RCode { get; set; }

        public string RText { get; set; } = string.Empty;
    }
}