using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ds.Core
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("PurchaseOrder")]
        public int OrderId { get; set; }
        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public Item Item { get; set; }
    }
}