using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace PensionSystem.Helpers
{
    public static class ModelStateExtension
    {
        public static string GetAllErrors(this ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(v => v.Errors);
            var sb = new StringBuilder();

            foreach (var error in errors)
            {
                sb.AppendLine(error.ErrorMessage);
            }

            return sb.ToString();
        }
    }
}