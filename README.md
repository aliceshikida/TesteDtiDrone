# Teste Prático dti digital Simulador de Encomendas em Drone

**- Restrições Assumidas:**
Limite de Peso por Drone
Capacidade máxima: 10kg por drone

Sistema de Prioridade
Escala: Numérica (quanto maior o número, maior a prioridade)
Aplicação: Pedidos com prioridade mais alta são atendidos primeiro
Critério secundário: Distância (pedidos mais próximos têm preferência)

Limite de Distância
Coordenadas X: -100 a +100
Coordenadas Y: -100 a +100
Cálculo: Distância euclidiana a partir da origem (0,0)

Limite de Drones
Quantidade máxima: 3 drones simultâneos, para simula uma frota pequena e gerenciável

**-  Decisões do Projeto**
1.⁠ ⁠Estratégia de Alocação: "Menos Entregas" - Alocar pedidos no drone com menos entregas atuais
Por que: Distribui a carga uniformemente entre os drones
Benefício: Evita sobrecarregar um único drone e otimiza o tempo total

2.⁠ ⁠Persistência de Dados - Armazenamento em memória com reset manual
Por que: Simplicidade para demonstração e testes
Benefício: Permite reiniciar facilmente e testar diferentes cenários

3.⁠ ⁠Cálculo de Tempo Realista
Por que: Velocidade típica de drones de entrega urbanos
Benefício: Oferece uma estimativa mais próxima da realidade para simulações e análises de desempenho


4.⁠ ⁠Validação dos dados - Validação com feedback visual
Por que: Melhora a experiência do usuário
Benefício: Reduz erros no preenchimento dos formulários e agiliza o uso do sistema

5.⁠ ⁠Relatório de Performance
Decisão: Métricas de eficiência baseadas em entregas/peso
Por que: Permite avaliar qual drone é mais produtivo
Benefício: Auxilia na tomada de decisões para ajustes na estratégia de alocação e uso dos drones


🛠️ Tecnologias Utilizadas
Backend: ASP.NET Core 6.0 (C#)
Frontend: HTML5, CSS3, JavaScript (Vanilla)
Arquitetura: RESTful API
Padrão: MVC (Model-View-Controller)
Serviços: Singleton para gerenciamento de estado

**- Instruções para Executar**
Pré-requisitos
.NET 6.0 SDK ou superior
Navegador web moderno

**Passos para Execução**

![A704125A-277A-44BA-8F3A-4357E034A03B_1_201_a](https://github.com/user-attachments/assets/638492b4-3cd0-415c-b095-61c3bd366050)

**Como Usar a Aplicação**
1. Preencha o formulário com:
Peso: 0-10kg
Coordenada X: -100 a +100
Coordenada Y: -100 a +100
Prioridade: Número (maior = mais prioritário)
Clique em "Cadastrar Pedido"

2. Simular Entregas
Clique em "Simular Entrega"
O sistema alocará automaticamente os pedidos entre os drones
Visualize a distribuição e pedidos não alocados (se houver)

4. Gerar Relatório
Clique em "Gerar Relatório"
Veja estatísticas de performance:
Total de entregas
Tempo médio por entrega
Drone mais eficiente
Tempo detalhado por drone

6. Resetar Dados
Clique em "Resetar Dados" para limpar tudo
Útil para testar novos cenários

** 🎮 Funcionalidades Principais **
Validação Inteligente
Limites de peso e coordenadas em tempo real
Mensagens de erro claras
Prevenção de dados inválidos

Alocação Otimizada
Distribuição automática entre drones
Respeito aos limites de peso
Priorização por prioridade e distância

Relatórios Detalhados
Métricas de eficiência
Tempo de entrega por drone
Análise de performance

Interface Responsiva
Design moderno e intuitivo
Feedback visual imediato
Compatível com diferentes dispositivos


