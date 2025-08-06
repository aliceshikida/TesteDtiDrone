using Microsoft.AspNetCore.Mvc;
using DroneDeliverySimulator.Services;

[ApiController]
[Route("api/simulacao")]
public class SimulacaoController : ControllerBase
{
    private readonly DeliveryService _service;

    public SimulacaoController(DeliveryService service)
    {
        _service = service;
    }

   
}
