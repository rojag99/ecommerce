namespace E_CommerceAPP
{
    public class ProductDTO
    {
        public int Product_ID { get; set; }
        public string Product_Name { get; set; }
        public string Product_Description { get; set; }
        public decimal Product_Price { get; set; }
        public int Category_ID { get; set; } // Foreign key for Category

        // If you need to include reviews in the DTO for some operations,
        // you might consider adding a list of ReviewDTOs here.
        // public List<ReviewDTO> Reviews { get; set; }
    }
}

