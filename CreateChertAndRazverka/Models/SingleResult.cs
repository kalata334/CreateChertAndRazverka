namespace CreateChertAndRazverka.Models
{
    public class SingleResult
    {
        public string       ComponentName      { get; set; } = "";
        public ResultStatus Status             { get; set; } = ResultStatus.Success;
        public string       Message            { get; set; } = "";
        public string       OutputPath         { get; set; } = "";
        public int          DimensionsInserted { get; set; }
    }
}
