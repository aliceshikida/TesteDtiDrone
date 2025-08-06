namespace DroneDeliverySimulator.Models;

public class Delivery
{
    public int Id { get; set; }
    public string Destino { get; set; }
    public string Status { get; set; } = "Pendente";
}
