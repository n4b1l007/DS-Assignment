using System.ComponentModel.DataAnnotations.Schema;

namespace Ds.Application.Dto
{
    public class OrderDetailDto
    {
        public int? Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
