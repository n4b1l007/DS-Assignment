using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ds.Core
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public string OrderNumber { get; set; }

        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public string Remark { get; set; }

        public Supplier Supplier { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

    }
}