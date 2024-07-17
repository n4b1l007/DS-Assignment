using Ds.Core;

namespace Ds.Application.Dto
{
    public class PurchaseOrderDto
    {
        public int? Id { get; set; }
        public string ReferenceId { get; set; }
        public string OrderNumber { get; set; }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = "";

        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public string Remark { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
