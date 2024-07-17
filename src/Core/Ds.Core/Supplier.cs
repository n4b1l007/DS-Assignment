using System.ComponentModel.DataAnnotations;

namespace Ds.Core
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}