namespace RookieEcommerce.SharedViewModels.PaymentDtos
{
    public class CreatePaymentDto
    {
        public decimal TotalAmount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}