using System;
using System.Collections.Generic;

namespace com.loitzl.userinviter.Models
{
    public enum ResultType
    {
        Unkknown = 0,
        Success = 1,
        Warning = 2,
        Error = 3
    }

    public class ResultViewModel
    {
        public ResultType Type { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string DebugInfo { get; set; }
    }

    public class ResultsViewModel
    {
        public List<ResultViewModel> Results { get; set; } = new List<ResultViewModel>();
    }
}