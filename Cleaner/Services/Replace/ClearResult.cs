namespace Cleaner.Services.Replace
{
    enum ClearResultType
    {
        Fixed,
        Impossible,
        Unnecessary
    }

    class ClearResult
    {
        public ClearResultType Type { get; set; }
        public string Text { get; set; }
    }
}
