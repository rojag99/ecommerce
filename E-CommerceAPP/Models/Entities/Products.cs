namespace E_CommerceAPP.Models.Entities
{
    public class Products
    {
        public int Product_ID { get; set; }
        public string Product_Name { get; set; }
        public string Product_Description { get; set; }
        public decimal Product_Price { get; set; }

        // Foreign keys
        public int Category_ID { get; set; } // Foreign key for Category


        // Navigation properties
       

        public Categories Categories{get; set; }
        
    }
}
