using System.Text.Json.Serialization;

namespace E_CommerceAPP.Models.Entities
{
    public class Categories
    {
        public int Category_ID { get; set; }
        public string Category_Name { get; set; }
        // Navigation property to Products

        [JsonIgnore]
        public ICollection<Products> Products { get; set; }

    }
}
