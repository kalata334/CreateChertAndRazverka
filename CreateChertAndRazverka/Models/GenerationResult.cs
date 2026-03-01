using System.Collections.Generic;

namespace CreateChertAndRazverka.Models
{
    public enum ResultStatus
    {
        Success,
        Skipped,
        Error
    }

    public class SingleResult
    {
        public string ComponentName { get; set; } = "";
        public ResultStatus Status  { get; set; } = ResultStatus.Success;
        public string Message       { get; set; } = "";
        public string OutputFile    { get; set; } = "";
    }

    public class GenerationResult
    {
        public List<SingleResult> Results { get; } = new List<SingleResult>();

        public int SuccessCount => Results.FindAll(r => r.Status == ResultStatus.Success).Count;
        public int SkippedCount => Results.FindAll(r => r.Status == ResultStatus.Skipped).Count;
        public int ErrorCount   => Results.FindAll(r => r.Status == ResultStatus.Error).Count;

        public void Add(SingleResult result) => Results.Add(result);

        public string Summary =>
            $"Готово: {SuccessCount}, Пропущено: {SkippedCount}, Ошибок: {ErrorCount}";
    }
}
