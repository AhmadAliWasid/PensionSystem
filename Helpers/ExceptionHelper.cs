namespace Pension.Entities.Helpers
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// Get Complete Detal Of Exception and its Detail
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetDetail(Exception exception)
        {
            string Message = "Message : " + exception.Message.ToString();
            if (exception != null && exception.InnerException != null)
            {
                Message += $"<br> Detail : {exception.InnerException.Message}";
            }
            return Message;
        }
    }
}