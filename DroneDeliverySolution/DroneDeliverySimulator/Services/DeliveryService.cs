using DroneDeliverySimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DroneDeliverySimulator.Services
{
    public class DeliveryService
    {
        private List<Pedido> _pedidos = new();
        private Dictionary<int, List<Pedido>> _entregasPorDrone = new();
        private const int CapacidadeMaxima = 10;
        private const int DistanciaMaxima = 100;
        private const int MaximoDrones = 3;
        private int _proximoIdPedido = 1;

        public DeliveryService()
        {
            // Garantir que tudo seja inicializado corretamente
            ResetarDados();
        }

        public void ResetarDados()
        {
            _pedidos.Clear();
            _entregasPorDrone.Clear();
            _proximoIdPedido = 1;
            Console.WriteLine("Dados resetados. Lista limpa e ID reiniciado.");
        }

        public object GerarRelatorio()
        {
            if (!_entregasPorDrone.Any())
            {
                return new
                {
                    totalEntregas = 0,
                    tempoMedio = 0,
                    droneMaisEficiente = "Nenhuma entrega realizada",
                    temposPorDrone = new List<object>()
                };
            }

            // Filtrar apenas drones válidos (excluir pedidos não alocados com chave -1)
            var dronesValidos = _entregasPorDrone.Where(kvp => kvp.Key > 0).ToList();
            
            if (!dronesValidos.Any())
            {
                return new
                {
                    totalEntregas = 0,
                    tempoMedio = 0,
                    droneMaisEficiente = "Nenhuma entrega realizada",
                    temposPorDrone = new List<object>()
                };
            }

            // Calcular total de entregas
            int totalEntregas = dronesValidos.Sum(kvp => kvp.Value.Count);

            // Calcular tempo de entrega para cada drone
            var temposPorDrone = new List<object>();
            double tempoTotal = 0;
            
            foreach (var drone in dronesValidos)
            {
                if (drone.Value.Any())
                {
                    // Calcular distância total percorrida pelo drone
                    double distanciaTotal = 0;
                    var coordenadas = new List<(double x, double y)>();
                    
                    // Adicionar origem (0,0)
                    coordenadas.Add((0, 0));
                    
                    // Adicionar destinos dos pedidos
                    foreach (var pedido in drone.Value)
                    {
                        coordenadas.Add((pedido.DestinoX, pedido.DestinoY));
                    }
                    
                    // Calcular distância total (ida e volta para origem)
                    for (int i = 0; i < coordenadas.Count - 1; i++)
                    {
                        double dx = coordenadas[i + 1].x - coordenadas[i].x;
                        double dy = coordenadas[i + 1].y - coordenadas[i].y;
                        distanciaTotal += Math.Sqrt(dx * dx + dy * dy);
                    }
                    
                    // Volta para origem
                    if (coordenadas.Count > 1)
                    {
                        double dx = coordenadas[0].x - coordenadas[coordenadas.Count - 1].x;
                        double dy = coordenadas[0].y - coordenadas[coordenadas.Count - 1].y;
                        distanciaTotal += Math.Sqrt(dx * dx + dy * dy);
                    }
                    
                    // Calcular tempo (assumindo velocidade de 20 km/h = 0.333 km/min)
                    double tempoDrone = distanciaTotal / 0.333; // em minutos
                    tempoTotal += tempoDrone;
                    
                    double pesoTotal = drone.Value.Sum(p => p.Peso);
                    
                    temposPorDrone.Add(new
                    {
                        droneId = drone.Key,
                        entregas = drone.Value.Count,
                        pesoTotal = pesoTotal,
                        distancia = distanciaTotal,
                        tempo = tempoDrone
                    });
                }
            }
            
            // Calcular tempo médio por entrega
            double tempoMedioPorEntrega = totalEntregas > 0 ? tempoTotal / totalEntregas : 0;

            // Encontrar drone mais eficiente (melhor relação entre entregas e peso)
            var droneEficiencia = dronesValidos
                .Where(kvp => kvp.Value.Any())
                .Select(kvp => new
                {
                    DroneId = kvp.Key,
                    TotalEntregas = kvp.Value.Count,
                    PesoTotal = kvp.Value.Sum(p => p.Peso),
                    PesoMedio = kvp.Value.Average(p => p.Peso),
                    // Eficiência = número de entregas / peso total (mais entregas com menos peso = mais eficiente)
                    Eficiencia = kvp.Value.Count / (kvp.Value.Sum(p => p.Peso) + 0.1) // +0.1 para evitar divisão por zero
                })
                .OrderByDescending(d => d.Eficiencia)
                .FirstOrDefault();

            return new
            {
                totalEntregas = totalEntregas,
                tempoMedio = Math.Round(tempoMedioPorEntrega, 1),
                droneMaisEficiente = droneEficiencia != null ? $"Drone {droneEficiencia.DroneId} ({droneEficiencia.TotalEntregas} entregas, {droneEficiencia.PesoTotal:F1}kg total)" : "Nenhum",
                temposPorDrone = temposPorDrone
            };
        }

        public void CadastrarPedido(Pedido pedido)
        {
            pedido.Id = _proximoIdPedido++; 
            _pedidos.Add(pedido);
        }

        public List<Pedido> ObterTodosOsPedidos()
        {
            return _pedidos.ToList();
        }

        public List<Pedido> ObterPedidosPendentes()
        {
            // Retorna todos os pedidos, não apenas os não entregues
            return _pedidos.ToList();
        }

        public List<Pedido> ObterPedidosEntregues()
        {
            return _pedidos.Where(p => p.Entregue).ToList();
        }

        public Dictionary<int, List<Pedido>> SimularEntregas()
        {
            var pedidosOrdenados = _pedidos
                .OrderByDescending(p => p.Prioridade)
                .ThenBy(p => Math.Sqrt(p.DestinoX * p.DestinoX + p.DestinoY * p.DestinoY))
                .ToList();

            _entregasPorDrone.Clear();
            
            // Inicializar os 3 drones
            for (int i = 1; i <= MaximoDrones; i++)
            {
                _entregasPorDrone[i] = new List<Pedido>();
            }
            
            var pedidosNaoAlocados = new List<Pedido>();
            
            // Alocar cada pedido no drone com menos entregas
            foreach (var pedido in pedidosOrdenados)
            {
                var droneEscolhido = EscolherDroneComMenosEntregas(pedido.Peso);
                if (droneEscolhido.HasValue)
                {
                    _entregasPorDrone[droneEscolhido.Value].Add(pedido);
                }
                else
                {
                    pedidosNaoAlocados.Add(pedido);
                }
            }

            // Adicionar informação sobre pedidos não alocados
            if (pedidosNaoAlocados.Any())
            {
                _entregasPorDrone[-1] = pedidosNaoAlocados; // Usar chave -1 para pedidos não alocados
            }

            return _entregasPorDrone;
        }

        private int? EscolherDroneComMenosEntregas(double pesoPedido)
        {
            // Verificar todos os drones em ordem de menos entregas
            var dronesOrdenados = _entregasPorDrone
                .OrderBy(kvp => kvp.Value.Count)
                .ToList();

            foreach (var drone in dronesOrdenados)
            {
                double pesoAtual = drone.Value.Sum(p => p.Peso);
                double pesoTotal = pesoAtual + pesoPedido;
                
                // Verificar se o drone pode carregar o novo pedido sem ultrapassar o limite
                if (pesoTotal <= CapacidadeMaxima)
                {
                    return drone.Key;
                }
            }
            
            return null; // Nenhum drone pode carregar o pedido
        }

        private List<List<Pedido>> OtimizarAlocacaoDrones(List<Pedido> pedidos)
        {
            if (!pedidos.Any()) return new List<List<Pedido>>();

            var melhorAlocacao = new List<List<Pedido>>();
            var melhorNumeroDrones = int.MaxValue;

            // Tentar diferentes estratégias de alocação
            var estrategias = new List<Func<List<Pedido>, List<List<Pedido>>>>
            {
                AlocacaoPorPeso,
                AlocacaoPorDistancia,
                AlocacaoPorPrioridade,
                AlocacaoGulosaMelhorada
            };

            foreach (var estrategia in estrategias)
            {
                var alocacao = estrategia(pedidos.ToList());
                var numeroDrones = alocacao.Count;

                if (numeroDrones < melhorNumeroDrones && ValidarAlocacao(alocacao))
                {
                    melhorAlocacao = alocacao;
                    melhorNumeroDrones = numeroDrones;
                }
            }

            return melhorAlocacao;
        }

        private bool ValidarAlocacao(List<List<Pedido>> alocacao)
        {
            foreach (var carga in alocacao)
            {
                if (!carga.Any()) continue;

                double pesoTotal = carga.Sum(p => p.Peso);
                if (pesoTotal > CapacidadeMaxima) return false;

                foreach (var pedido in carga)
                {
                    double distancia = Math.Sqrt(pedido.DestinoX * pedido.DestinoX + pedido.DestinoY * pedido.DestinoY);
                    if (distancia > DistanciaMaxima) return false;
                }
            }
            return true;
        }

        private List<List<Pedido>> AlocacaoPorPeso(List<Pedido> pedidos)
        {
            var alocacao = new List<List<Pedido>>();
            var pedidosRestantes = pedidos.OrderByDescending(p => p.Peso).ToList();

            while (pedidosRestantes.Any())
            {
                var carga = new List<Pedido>();
                double pesoAtual = 0;

                for (int i = 0; i < pedidosRestantes.Count; i++)
                {
                    var pedido = pedidosRestantes[i];
                    double distancia = Math.Sqrt(pedido.DestinoX * pedido.DestinoX + pedido.DestinoY * pedido.DestinoY);

                    if (pesoAtual + pedido.Peso <= CapacidadeMaxima && distancia <= DistanciaMaxima)
                    {
                        carga.Add(pedido);
                        pesoAtual += pedido.Peso;
                        pedidosRestantes.RemoveAt(i);
                        i--;
                    }
                }

                if (carga.Any())
                {
                    alocacao.Add(carga);
                }
                else
                {
                    break;
                }
            }

            return alocacao;
        }

        private List<List<Pedido>> AlocacaoPorDistancia(List<Pedido> pedidos)
        {
            var alocacao = new List<List<Pedido>>();
            var pedidosRestantes = pedidos.OrderBy(p => Math.Sqrt(p.DestinoX * p.DestinoX + p.DestinoY * p.DestinoY)).ToList();

            while (pedidosRestantes.Count > 0)
            {
                var carga = new List<Pedido>();
                double pesoAtual = 0;

                for (int i = 0; i < pedidosRestantes.Count; i++)
                {
                    var pedido = pedidosRestantes[i];
                    double distancia = Math.Sqrt(pedido.DestinoX * pedido.DestinoX + pedido.DestinoY * pedido.DestinoY);

                    if (pesoAtual + pedido.Peso <= CapacidadeMaxima && distancia <= DistanciaMaxima)
                    {
                        carga.Add(pedido);
                        pesoAtual += pedido.Peso;
                        pedidosRestantes.RemoveAt(i);
                        i--;
                    }
                }

                if (carga.Any())
                {
                    alocacao.Add(carga);
                }
                else
                {
                    break;
                }
            }

            return alocacao;
        }

        private List<List<Pedido>> AlocacaoPorPrioridade(List<Pedido> pedidos)
        {
            var alocacao = new List<List<Pedido>>();
            var pedidosRestantes = pedidos.OrderByDescending(p => p.Prioridade).ToList();

            while (pedidosRestantes.Count > 0)
            {
                var carga = new List<Pedido>();
                double pesoAtual = 0;

                for (int i = 0; i < pedidosRestantes.Count; i++)
                {
                    var pedido = pedidosRestantes[i];
                    double distancia = Math.Sqrt(pedido.DestinoX * pedido.DestinoX + pedido.DestinoY * pedido.DestinoY);

                    if (pesoAtual + pedido.Peso <= CapacidadeMaxima && distancia <= DistanciaMaxima)
                    {
                        carga.Add(pedido);
                        pesoAtual += pedido.Peso;
                        pedidosRestantes.RemoveAt(i);
                        i--;
                    }
                }

                if (carga.Any())
                {
                    alocacao.Add(carga);
                }
                else
                {
                    break;
                }
            }

            return alocacao;
        }

        private List<List<Pedido>> AlocacaoGulosaMelhorada(List<Pedido> pedidos)
        {
            var alocacao = new List<List<Pedido>>();
            var pedidosRestantes = pedidos.ToList();

            while (pedidosRestantes.Count > 0)
            {
                var carga = new List<Pedido>();
                double pesoAtual = 0;

                // Tentar encher o drone de forma mais eficiente
                var candidatos = pedidosRestantes
                    .Where(p => pesoAtual + p.Peso <= CapacidadeMaxima)
                    .Where(p => Math.Sqrt(p.DestinoX * p.DestinoX + p.DestinoY * p.DestinoY) <= DistanciaMaxima)
                    .OrderByDescending(p => p.Peso) // Preferir pedidos mais pesados primeiro
                    .ThenBy(p => Math.Sqrt(p.DestinoX * p.DestinoX + p.DestinoY * p.DestinoY)) // Depois por distância
                    .ToList();

                foreach (var pedido in candidatos)
                {
                    if (pesoAtual + pedido.Peso <= CapacidadeMaxima)
                    {
                        carga.Add(pedido);
                        pesoAtual += pedido.Peso;
                        pedidosRestantes.Remove(pedido);
                    }
                }

                if (carga.Any())
                {
                    alocacao.Add(carga);
                }
                else
                {
                    break;
                }
            }

            return alocacao;
        }

        public Dictionary<int, int> ObterTemposDeEntrega()
        {
            var tempos = new Dictionary<int, int>();

            foreach (var entrega in _entregasPorDrone)
            {
                double tempoTotal = 0;

                foreach (var pedido in entrega.Value)
                {
                    double distancia = Math.Sqrt(pedido.DestinoX * pedido.DestinoX + pedido.DestinoY * pedido.DestinoY);
                    tempoTotal += (distancia * 2) / 10;
                }

                tempos[entrega.Key] = (int)Math.Ceiling(tempoTotal);
            }

            return tempos;
        }
    }
}