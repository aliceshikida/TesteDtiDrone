using Microsoft.AspNetCore.Mvc;
using DroneDeliverySimulator.Models;
using DroneDeliverySimulator.Services;

namespace DroneDeliverySimulator.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidoController : ControllerBase
    {
        private readonly DeliveryService _service;

        public PedidoController(DeliveryService service)
        {
            _service = service;
        }

        // Nova rota para obter todos os pedidos (pendentes e entregues)
        [HttpGet]
        public IActionResult Get()
        {
            var todosPedidos = _service.ObterTodosOsPedidos();
            return Ok(todosPedidos);
        }

        // Cadastra um novo pedido
        [HttpPost]
        public IActionResult Post([FromBody] Pedido pedido)
        {
            if (pedido == null)
                return BadRequest("Pedido não pode ser nulo");

            _service.CadastrarPedido(pedido);
            return Ok(pedido);
        }

        // Obtém todos os pedidos pendentes (não entregues)
        [HttpGet("pendentes")]
        public IActionResult GetPendentes()
        {
            var pendentes = _service.ObterPedidosPendentes();
            return Ok(pendentes);
        }

        // Obtém todos os pedidos já entregues
        [HttpGet("entregues")]
        public IActionResult GetEntregues()
        {
            var entregues = _service.ObterPedidosEntregues();
            return Ok(entregues);
        }

        // Simula a entrega dos pedidos pendentes
        [HttpGet("simular")]
        public IActionResult Simular()
        {
            var resultado = _service.SimularEntregas();
            
            if (resultado.Count == 0)
            {
                return Ok(new List<string> { "Nenhum pedido pendente para entrega." });
            }

            var mensagens = new List<string>();
            var pedidosNaoAlocados = new List<Pedido>();
            
            foreach (var entrega in resultado)
            {
                if (entrega.Key == -1)
                {
                    // Pedidos não alocados
                    pedidosNaoAlocados = entrega.Value;
                }
                else
                {
                    // Calcular peso total do drone
                    double pesoTotal = entrega.Value.Sum(p => p.Peso);
                    mensagens.Add($"Drone {entrega.Key} entregou {entrega.Value.Count} pedido(s) - Peso total: {pesoTotal:F1}kg:");
                    foreach (var pedido in entrega.Value)
                    {
                        mensagens.Add($"  - Pedido #{pedido.Id} (Peso: {pedido.Peso}kg, Destino: ({pedido.DestinoX}, {pedido.DestinoY}))");
                    }
                }
            }

            // Adicionar notificação sobre pedidos não alocados
            if (pedidosNaoAlocados.Any())
            {
                mensagens.Add("");
                mensagens.Add("⚠️ ATENÇÃO: Os seguintes pedidos NÃO puderam ser alocados:");
                mensagens.Add("Todos os drones estão ocupados e já ultrapassaram o limite de peso (10kg por drone).");
                foreach (var pedido in pedidosNaoAlocados)
                {
                    mensagens.Add($"  - Pedido #{pedido.Id} (Peso: {pedido.Peso}kg, Destino: ({pedido.DestinoX}, {pedido.DestinoY}))");
                }
            }

            return Ok(mensagens);
        }

        // Reseta todos os dados (para reiniciar do zero)
        [HttpPost("resetar")]
        public IActionResult Resetar()
        {
            _service.ResetarDados();
            return Ok("Dados resetados com sucesso!");
        }

        // Gera relatório de performance
        [HttpGet("relatorio")]
        public IActionResult GerarRelatorio()
        {
            var relatorio = _service.GerarRelatorio();
            return Ok(relatorio);
        }
    }
}