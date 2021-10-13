
using System.Threading.Tasks;
using com.loitzl.userinviter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace com.loitzl.userinviter
{

    public class WithAlertResult : IActionResult
    {
        public IActionResult Result { get; }
        public string Type { get; }
        public string Message { get; }
        public string DebugInfo { get; }

        public WithAlertResult(IActionResult result,
                                    string type,
                                    string message,
                                    string debugInfo)
        {
            Result = result;
            Type = type;
            Message = message;
            DebugInfo = debugInfo;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();

            var tempData = factory.GetTempData(context.HttpContext);

            tempData["_alertType"] = Type;
            tempData["_alertMessage"] = Message;
            tempData["_alertDebugInfo"] = DebugInfo;

            await Result.ExecuteResultAsync(context);
        }
    }

    public static class AlertExtensions
    {

        public static IActionResult WithError(this IActionResult result,
                                            string message,
                                            string debugInfo = null)
        {
            return Alert(result, "danger", message, debugInfo);
        }


        private static IActionResult Alert(IActionResult result,
                                        string type,
                                        string message,
                                        string debugInfo)
        {
            return new WithAlertResult(result, type, message, debugInfo);
        }
    }
}