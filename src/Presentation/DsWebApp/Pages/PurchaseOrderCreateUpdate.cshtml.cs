
namespace DsWebApp.Pages
{
    public class PurchaseOrderCreateUpdateModel : BasePageModel
    {
        public int? Id { get; set; }
        public string ReferenceId { get; set; }
        public string OrderNumber { get; set; }

        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public string Remark { get; set; }
        public PurchaseOrderCreateUpdateModel(IConfiguration configuration) : base(configuration)
        {
        }
        public void OnGet(int? id)
        {
            if(id != null)
            {
                Id = id.Value;
            }
        }
    }
}
