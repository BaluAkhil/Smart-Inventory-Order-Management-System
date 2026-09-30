namespace OrderManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;


        public string Role { get; set; } = UserRoles.Customer;

        public Cart ? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
