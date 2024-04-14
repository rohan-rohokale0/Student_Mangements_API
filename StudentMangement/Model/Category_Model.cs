using System.ComponentModel.DataAnnotations;

namespace StudentMangement.Model
{
    public class Category_Model
    {
        public Guid ID { get; set; }

        [Required]
        public string? categoryName { get; set; }

        public static implicit operator Category_Model(string v)
        {
            throw new NotImplementedException();
        }
    }
}
