
using System.Text.Json;
using System.Threading.Tasks;
using com.loitzl.userinviter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace com.loitzl.userinviter
{
    public class WithOutcomeResult : IActionResult
    {
        public IActionResult Result { get; }
        public ResultsViewModel Results { get; }
        public string DebugInfo { get; }

        public WithOutcomeResult(IActionResult result,
                                    ResultsViewModel results,
                                    string debugInfo)
        {
            Result = result;
            Results = results;
            DebugInfo = debugInfo;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();

            var tempData = factory.GetTempData(context.HttpContext);

            tempData["_results"] = JsonSerializer.Serialize(Results);
            tempData["_alertDebugInfo"] = DebugInfo;

            await Result.ExecuteResultAsync(context);
        }
    }

    public static class ReesultsExtensions
    {
        private static IActionResult Outcome(IActionResult result,
                           ResultsViewModel results,
                           string debugInfo)
        {
            return new WithOutcomeResult(result, results, debugInfo);
        }

        public static IActionResult WithOutcome(this IActionResult result,
                                    ResultsViewModel results,
                                    string debugInfo = null)
        {
            return Outcome(result, results, debugInfo);
        }

    }
}