namespace E_CommerceAPP
{
    public class CategoriesDTO
    {
        public int Category_ID { get; set; }
        public string Category_Name { get; set; }
       
        public List<ProductDTO> Products { get; set; }
    }
}
