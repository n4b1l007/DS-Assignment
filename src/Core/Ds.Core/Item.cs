using System.ComponentModel.DataAnnotations;

namespace Ds.Core
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}