using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPP.Models.Entities
{
    public class Customer
    {
        [Key] // Specify that Customer_ID is the primary key
        public int Customer_ID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public long Contact_Number { get; set; }
        public string Email_ID { get; set; }
        public string Password { get; set; }
        public string Confirm_Password { get; set; }
       

        // Navigation property for Reviews
        
    }
}
