namespace LLMS.ViewModel
{
    public class MainWindowViewModel
    {
        public int LeaseId { get; set; }
        public int PaymentDueDay { get; set; }
        public decimal RentAmount { get; set; }
        public string Address { get; set; }
        public string TenantName { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNo { get; set; }
    }
}
