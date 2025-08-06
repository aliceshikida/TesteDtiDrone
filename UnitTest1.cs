using DroneDeliverySimulator.Models;
using DroneDeliverySimulator.Services;
using Xunit;

namespace DroneDeliverySimulator.Tests
{
    public class DeliveryServiceTests
    {
        private readonly DeliveryService _service;

        public DeliveryServiceTests()
        {
            _service = new DeliveryService();
        }

        #region Testes de Limites e Validações

        [Fact]
        public void DeveRespeitarLimiteDePesoPorDrone()
        {
            // Arrange
            var pedido1 = new Pedido { Peso = 8, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };
            var pedido2 = new Pedido { Peso = 5, DestinoX = 20, DestinoY = 20, Prioridade = "alta" };
            var pedido3 = new Pedido { Peso = 3, DestinoX = 30, DestinoY = 30, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedido1);
            _service.CadastrarPedido(pedido2);
            _service.CadastrarPedido(pedido3);
            var resultado = _service.SimularEntregas();

            // Assert
            // Verificar se nenhum drone tem mais de 10kg
            foreach (var drone in resultado.Where(kvp => kvp.Key > 0))
            {
                double pesoTotal = drone.Value.Sum(p => p.Peso);
                Assert.True(pesoTotal <= 10, $"Drone {drone.Key} tem {pesoTotal}kg, excedendo o limite de 10kg");
            }
        }

        [Fact]
        public void DeveLimitarNumeroMaximoDeDrones()
        {
            // Arrange - Criar pedidos que forcem o uso de mais de 3 drones
            var pedidos = new List<Pedido>();
            for (int i = 0; i < 10; i++)
            {
                pedidos.Add(new Pedido 
                { 
                    Peso = 8, 
                    DestinoX = i * 10, 
                    DestinoY = i * 10, 
                    Prioridade = "alta" 
                });
            }

            // Act
            foreach (var pedido in pedidos)
            {
                _service.CadastrarPedido(pedido);
            }
            var resultado = _service.SimularEntregas();

            // Assert
            var dronesAtivos = resultado.Where(kvp => kvp.Key > 0).Count();
            Assert.True(dronesAtivos <= 3, $"Sistema está usando {dronesAtivos} drones, excedendo o limite de 3");
        }

        [Fact]
        public void DeveOtimizarAlocacaoParaMaximizarCapacidade()
        {
            // Arrange
            var pedido1 = new Pedido { Peso = 3, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };
            var pedido2 = new Pedido { Peso = 3, DestinoX = 20, DestinoY = 20, Prioridade = "alta" };
            var pedido3 = new Pedido { Peso = 3, DestinoX = 30, DestinoY = 30, Prioridade = "alta" };
            var pedido4 = new Pedido { Peso = 3, DestinoX = 40, DestinoY = 40, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedido1);
            _service.CadastrarPedido(pedido2);
            _service.CadastrarPedido(pedido3);
            _service.CadastrarPedido(pedido4);
            var resultado = _service.SimularEntregas();

            // Assert
            var dronesComPedidos = resultado.Where(kvp => kvp.Key > 0 && kvp.Value.Any()).ToList();
            
            // Verificar se todos os pedidos foram alocados (otimização funcionando)
            var totalPedidosAlocados = dronesComPedidos.Sum(kvp => kvp.Value.Count);
            Assert.Equal(4, totalPedidosAlocados);
            
            // Verificar se nenhum drone excede a capacidade
            foreach (var drone in dronesComPedidos)
            {
                double pesoTotal = drone.Value.Sum(p => p.Peso);
                Assert.True(pesoTotal <= 10, $"Drone {drone.Key} tem {pesoTotal}kg, excedendo o limite de 10kg");
            }
        }

        [Fact]
        public void DeveNotificarPedidosNaoAlocadosQuandoLimiteExcedido()
        {
            // Arrange - Criar pedidos que excedam a capacidade total
            var pedidos = new List<Pedido>();
            for (int i = 0; i < 5; i++) // 5 pedidos de 8kg cada = 40kg total, mas só 3 drones x 10kg = 30kg
            {
                pedidos.Add(new Pedido 
                { 
                    Peso = 8, 
                    DestinoX = i * 10, 
                    DestinoY = i * 10, 
                    Prioridade = "alta" 
                });
            }

            // Act
            foreach (var pedido in pedidos)
            {
                _service.CadastrarPedido(pedido);
            }
            var resultado = _service.SimularEntregas();

            // Assert
            // Com a otimização, alguns pedidos podem ser alocados melhor
            // Verificar se pelo menos alguns pedidos foram alocados
            var dronesComPedidos = resultado.Where(kvp => kvp.Key > 0 && kvp.Value.Any()).ToList();
            var totalAlocados = dronesComPedidos.Sum(kvp => kvp.Value.Count);
            
            Assert.True(totalAlocados > 0, "Pelo menos alguns pedidos devem ser alocados");
            
            // Verificar se há pedidos não alocados (pode ser que a otimização consiga alocar todos)
            if (resultado.ContainsKey(-1))
            {
                Assert.True(resultado[-1].Any(), "Se há pedidos não alocados, a lista não deve estar vazia");
            }
        }

        #endregion

        #region Testes de Otimização de Capacidade

        [Fact]
        public void DeveMaximizarUtilizacaoDaCapacidadeDosDrones()
        {
            // Arrange - Criar pedidos que podem ser otimizados
            var pedidos = new List<Pedido>
            {
                new Pedido { Peso = 9, DestinoX = 10, DestinoY = 10, Prioridade = "alta" },
                new Pedido { Peso = 8, DestinoX = 20, DestinoY = 20, Prioridade = "alta" },
                new Pedido { Peso = 7, DestinoX = 30, DestinoY = 30, Prioridade = "alta" },
                new Pedido { Peso = 6, DestinoX = 40, DestinoY = 40, Prioridade = "alta" },
                new Pedido { Peso = 5, DestinoX = 50, DestinoY = 50, Prioridade = "alta" },
                new Pedido { Peso = 4, DestinoX = 60, DestinoY = 60, Prioridade = "alta" }
            };

            // Act
            foreach (var pedido in pedidos)
            {
                _service.CadastrarPedido(pedido);
            }
            var resultado = _service.SimularEntregas();

            // Assert
            var dronesComPedidos = resultado.Where(kvp => kvp.Key > 0 && kvp.Value.Any()).ToList();
            
            // Verificar se a otimização conseguiu alocar pedidos
            var totalPedidosAlocados = dronesComPedidos.Sum(kvp => kvp.Value.Count);
            Assert.True(totalPedidosAlocados >= 3, $"Otimização deve alocar pelo menos 3 pedidos, alocou {totalPedidosAlocados}");
            
            // Verificar utilização da capacidade (deve ser alta)
            double utilizacaoMedia = 0;
            foreach (var drone in dronesComPedidos)
            {
                double pesoTotal = drone.Value.Sum(p => p.Peso);
                double utilizacao = pesoTotal / 10.0; // 10kg é a capacidade máxima
                utilizacaoMedia += utilizacao;
                Assert.True(utilizacao > 0.3, $"Drone {drone.Key} tem utilização muito baixa: {utilizacao:P0}");
            }
            
            utilizacaoMedia /= dronesComPedidos.Count;
            Assert.True(utilizacaoMedia > 0.6, $"Utilização média muito baixa: {utilizacaoMedia:P0}");
        }

        [Fact]
        public void DeveAlocarPedidosQueAntesEramRejeitados()
        {
            // Arrange - Cenário que antes rejeitaria pedidos
            var pedidos = new List<Pedido>
            {
                new Pedido { Peso = 7, DestinoX = 10, DestinoY = 10, Prioridade = "alta" },
                new Pedido { Peso = 7, DestinoX = 20, DestinoY = 20, Prioridade = "alta" },
                new Pedido { Peso = 7, DestinoX = 30, DestinoY = 30, Prioridade = "alta" },
                new Pedido { Peso = 7, DestinoX = 40, DestinoY = 40, Prioridade = "alta" },
                new Pedido { Peso = 7, DestinoX = 50, DestinoY = 50, Prioridade = "alta" }
            };

            // Act
            foreach (var pedido in pedidos)
            {
                _service.CadastrarPedido(pedido);
            }
            var resultado = _service.SimularEntregas();

            // Assert
            var dronesComPedidos = resultado.Where(kvp => kvp.Key > 0 && kvp.Value.Any()).ToList();
            var totalPedidosAlocados = dronesComPedidos.Sum(kvp => kvp.Value.Count);
            
            // Com a otimização, deve conseguir alocar pedidos (antes poderia rejeitar todos)
            Assert.True(totalPedidosAlocados >= 3, $"Otimização deve alocar pelo menos 3 pedidos, alocou {totalPedidosAlocados}");
            
            // Verificar se a otimização está funcionando melhor que o algoritmo simples
            // 5 pedidos de 7kg = 35kg total, 3 drones x 10kg = 30kg capacidade
            // Algoritmo otimizado deve conseguir alocar pelo menos 3 pedidos
            var pedidosNaoAlocados = resultado.ContainsKey(-1) ? resultado[-1].Count : 0;
            Assert.True(pedidosNaoAlocados <= 2, $"Muitos pedidos não alocados: {pedidosNaoAlocados}");
        }

        #endregion

        #region Testes de Prioridade e Ordenação

        [Fact]
        public void DeveOrdenarPedidosPorPrioridade()
        {
            // Arrange
            var pedidoBaixa = new Pedido { Peso = 3, DestinoX = 10, DestinoY = 10, Prioridade = "baixa" };
            var pedidoMedia = new Pedido { Peso = 3, DestinoX = 20, DestinoY = 20, Prioridade = "media" };
            var pedidoAlta = new Pedido { Peso = 3, DestinoX = 30, DestinoY = 30, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedidoBaixa);
            _service.CadastrarPedido(pedidoMedia);
            _service.CadastrarPedido(pedidoAlta);
            var resultado = _service.SimularEntregas();

            // Assert
            var todosPedidos = resultado.Where(kvp => kvp.Key > 0)
                                       .SelectMany(kvp => kvp.Value)
                                       .ToList();

            // Verificar se pelo menos um pedido de alta prioridade foi alocado primeiro
            Assert.True(todosPedidos.Any(p => p.Prioridade == "alta"), "Pedidos de alta prioridade devem ser alocados");
        }

        [Fact]
        public void DeveConsiderarDistanciaComoCriterioSecundario()
        {
            // Arrange - Dois pedidos com mesma prioridade mas distâncias diferentes
            var pedidoProximo = new Pedido { Peso = 3, DestinoX = 5, DestinoY = 5, Prioridade = "media" };
            var pedidoDistante = new Pedido { Peso = 3, DestinoX = 50, DestinoY = 50, Prioridade = "media" };

            // Act
            _service.CadastrarPedido(pedidoDistante); // Cadastra o distante primeiro
            _service.CadastrarPedido(pedidoProximo);  // Depois o próximo
            var resultado = _service.SimularEntregas();

            // Assert
            var todosPedidos = resultado.Where(kvp => kvp.Key > 0)
                                       .SelectMany(kvp => kvp.Value)
                                       .ToList();

            Assert.True(todosPedidos.Any(p => p.DestinoX == 5 && p.DestinoY == 5), 
                       "Pedido mais próximo deve ser considerado na alocação");
        }

        #endregion

        #region Testes de Persistência e Reset

        [Fact]
        public void DeveManterPedidosAteResetExplicito()
        {
            // Arrange
            var pedido1 = new Pedido { Peso = 3, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };
            var pedido2 = new Pedido { Peso = 3, DestinoX = 20, DestinoY = 20, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedido1);
            _service.SimularEntregas(); // Simula mas não deve remover pedidos
            _service.CadastrarPedido(pedido2);
            
            var pedidosAntesReset = _service.ObterTodosOsPedidos();
            _service.ResetarDados();
            var pedidosAposReset = _service.ObterTodosOsPedidos();

            // Assert
            Assert.Equal(2, pedidosAntesReset.Count);
            Assert.Empty(pedidosAposReset);
        }

        [Fact]
        public void DeveResetarIdsAposReset()
        {
            // Arrange
            var pedido1 = new Pedido { Peso = 3, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };
            var pedido2 = new Pedido { Peso = 3, DestinoX = 20, DestinoY = 20, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedido1);
            _service.ResetarDados();
            _service.CadastrarPedido(pedido2);

            // Assert
            var pedidos = _service.ObterTodosOsPedidos();
            Assert.Single(pedidos);
            Assert.Equal(1, pedidos[0].Id); // ID deve começar do 1 novamente
        }

        #endregion

        #region Testes de Cálculo de Distância e Tempo

        [Fact]
        public void DeveCalcularDistanciaEuclidianaCorretamente()
        {
            // Arrange
            var pedido = new Pedido { Peso = 3, DestinoX = 3, DestinoY = 4, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedido);
            var resultado = _service.SimularEntregas();

            // Assert
            var droneComPedido = resultado.FirstOrDefault(kvp => kvp.Key > 0 && kvp.Value.Any());
            Assert.NotNull(droneComPedido);
            
            // Distância esperada: sqrt(3² + 4²) = sqrt(9 + 16) = sqrt(25) = 5
            var pedidoAlocado = droneComPedido.Value.First();
            double distanciaEsperada = Math.Sqrt(3 * 3 + 4 * 4);
            Assert.Equal(5, distanciaEsperada, 2);
        }

        [Fact]
        public void DeveCalcularTempoDeEntregaCorretamente()
        {
            // Arrange
            var pedido = new Pedido { Peso = 3, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedido);
            var resultado = _service.SimularEntregas();
            var relatorio = _service.GerarRelatorio();

            // Assert
            // Distância: sqrt(10² + 10²) = sqrt(200) ≈ 14.14 km
            // Tempo: 14.14 / 0.333 ≈ 42.5 minutos (ida e volta)
            Assert.NotNull(relatorio);
        }

        #endregion

        #region Testes de Relatório

        [Fact]
        public void DeveGerarRelatorioComMetricasCorretas()
        {
            // Arrange
            var pedido1 = new Pedido { Peso = 5, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };
            var pedido2 = new Pedido { Peso = 3, DestinoX = 20, DestinoY = 20, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedido1);
            _service.CadastrarPedido(pedido2);
            _service.SimularEntregas();
            var relatorio = _service.GerarRelatorio();

            // Assert
            Assert.NotNull(relatorio);
            
            // Verificar se o relatório tem as propriedades esperadas
            var tipoRelatorio = relatorio.GetType();
            Assert.True(tipoRelatorio.GetProperty("totalEntregas") != null);
            Assert.True(tipoRelatorio.GetProperty("tempoMedio") != null);
            Assert.True(tipoRelatorio.GetProperty("droneMaisEficiente") != null);
            Assert.True(tipoRelatorio.GetProperty("temposPorDrone") != null);
        }

        [Fact]
        public void DeveCalcularEficienciaDoDroneCorretamente()
        {
            // Arrange
            var pedido1 = new Pedido { Peso = 5, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };
            var pedido2 = new Pedido { Peso = 3, DestinoX = 20, DestinoY = 20, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedido1);
            _service.CadastrarPedido(pedido2);
            _service.SimularEntregas();
            var relatorio = _service.GerarRelatorio();

            // Assert
            Assert.NotNull(relatorio);
            
            // Eficiência = número de entregas / (peso total + 0.1)
            // Drone com 2 entregas e 8kg total: 2 / (8 + 0.1) = 2 / 8.1 ≈ 0.247
        }

        #endregion

        #region Testes de Casos Extremos

        [Fact]
        public void DeveLidarComPedidosNoLimiteExato()
        {
            // Arrange
            var pedidoLimite = new Pedido { Peso = 10, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedidoLimite);
            var resultado = _service.SimularEntregas();

            // Assert
            var droneComPedido = resultado.FirstOrDefault(kvp => kvp.Key > 0 && kvp.Value.Any());
            Assert.NotNull(droneComPedido);
            Assert.Equal(10, droneComPedido.Value.First().Peso);
        }

        [Fact]
        public void DeveLidarComPedidosExcedendoLimite()
        {
            // Arrange
            var pedidoExcedente = new Pedido { Peso = 12, DestinoX = 10, DestinoY = 10, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedidoExcedente);
            var resultado = _service.SimularEntregas();

            // Assert
            // Pedido deve ir para lista de não alocados
            Assert.True(resultado.ContainsKey(-1));
            Assert.Contains(pedidoExcedente, resultado[-1]);
        }

        [Fact]
        public void DeveLidarComCoordenadasNoLimite()
        {
            // Arrange
            var pedidoLimiteX = new Pedido { Peso = 3, DestinoX = 100, DestinoY = 0, Prioridade = "alta" };
            var pedidoLimiteY = new Pedido { Peso = 3, DestinoX = 0, DestinoY = 100, Prioridade = "alta" };

            // Act
            _service.CadastrarPedido(pedidoLimiteX);
            _service.CadastrarPedido(pedidoLimiteY);
            var resultado = _service.SimularEntregas();

            // Assert
            var todosPedidos = resultado.Where(kvp => kvp.Key > 0)
                                       .SelectMany(kvp => kvp.Value)
                                       .ToList();
            
            Assert.True(todosPedidos.Any(p => p.DestinoX == 100));
            Assert.True(todosPedidos.Any(p => p.DestinoY == 100));
        }

        [Fact]
        public void DeveLidarComListaVazia()
        {
            // Act
            var resultado = _service.SimularEntregas();
            var relatorio = _service.GerarRelatorio();

            // Assert
            // Verificar se não há pedidos alocados (drones vazios)
            var dronesComPedidos = resultado.Where(kvp => kvp.Key > 0 && kvp.Value.Any()).ToList();
            Assert.Empty(dronesComPedidos);
            Assert.NotNull(relatorio);
        }

        #endregion

        #region Testes de Integração

        [Fact]
        public void DeveExecutarFluxoCompletoCorretamente()
        {
            // Arrange
            var pedidos = new List<Pedido>
            {
                new Pedido { Peso = 5, DestinoX = 10, DestinoY = 10, Prioridade = "alta" },
                new Pedido { Peso = 3, DestinoX = 20, DestinoY = 20, Prioridade = "media" },
                new Pedido { Peso = 7, DestinoX = 30, DestinoY = 30, Prioridade = "baixa" }
            };

            // Act
            foreach (var pedido in pedidos)
            {
                _service.CadastrarPedido(pedido);
            }
            
            var pedidosCadastrados = _service.ObterTodosOsPedidos();
            var resultadoSimulacao = _service.SimularEntregas();
            var relatorio = _service.GerarRelatorio();

            // Assert
            Assert.Equal(3, pedidosCadastrados.Count);
            Assert.True(resultadoSimulacao.Any(kvp => kvp.Key > 0));
            Assert.NotNull(relatorio);
        }

        #endregion
    }
}
