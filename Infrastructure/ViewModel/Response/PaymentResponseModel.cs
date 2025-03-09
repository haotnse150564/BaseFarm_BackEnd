namespace Infrastructure.ViewModel.Response;

public class PaymentResponseModel
{
    public string OrderDescription { get; set; }
    public string TransactionId { get; set; }
    public long OrderId { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentId { get; set; }
    public bool Success { get; set; }
    public string Token { get; set; }
    public string VnPayResponseCode { get; set; }
}