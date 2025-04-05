namespace Project_SophieBakes.ViewModels
{
    public class SalesReportViewModel
    {
        public decimal TotalSales { get; set; } = 0;
        public decimal UserSales { get; set; } = 0;
        public string SelectedUserId { get; set; }
    }
}