namespace DsApi.ViewModel
{
    public class DataTableParameters
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string Search { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }
}
