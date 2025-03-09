namespace Application.ViewModel.Request;

public class PaymentInformationModel
{
    public long OrderId { get; set; }
    public string OrderType { get; set; }
    public double Amount { get; set; }
    public string OrderDescription { get; set; }
    public string Name { get; set; }
}