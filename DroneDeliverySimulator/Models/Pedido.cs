namespace DroneDeliverySimulator.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public double Peso { get; set; }
        public double DestinoX { get; set; }
        public double DestinoY { get; set; }
        public string Prioridade { get; set; } = "baixa";
        public bool Entregue { get; set; } = false;
    }
}