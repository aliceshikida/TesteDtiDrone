# 🛰️ Teste Prático – DTI Digital  
## **Simulador de Encomendas com Drones**

---

## 📌 Restrições Assumidas

### 🚫 Limite de Peso por Drone  
- **Capacidade máxima:** 10 kg por drone

### 🎯 Sistema de Prioridade  
- **Escala:** Numérica (quanto maior o número, maior a prioridade)  
- **Aplicação:** Pedidos com prioridade mais alta são atendidos primeiro  
- **Critério secundário:** Distância (pedidos mais próximos têm preferência)

### 📍 Limite de Distância  
- **Coordenadas válidas:**  
  - X: -100 a +100  
  - Y: -100 a +100  
- **Cálculo da distância:** Euclidiana a partir da origem (0, 0)

### 🚁 Limite de Drones  
- **Máximo de drones simultâneos:** 3  
- **Motivo:** Simular uma frota pequena e gerenciável

---

## 🧠 Decisões do Projeto

### 1. 📦 Estratégia de Alocação: *"Menos Entregas"*  
- **Decisão:** Alocar pedidos no drone com menos entregas atuais  
- **Por que:** Distribui a carga uniformemente entre os drones  
- **Benefício:** Evita sobrecarregar um único drone e otimiza o tempo total

### 2. 🧾 Persistência de Dados  
- **Decisão:** Armazenamento em memória com reset manual  
- **Por que:** Simplicidade para demonstração e testes  
- **Benefício:** Permite reiniciar facilmente e testar diferentes cenários

### 3. ⏱️ Cálculo de Tempo Realista  
- **Decisão:** Velocidade de 20 km/h (0,333 km/min)  
- **Por que:** Velocidade típica de drones de entrega urbanos  
- **Cálculo:** Considera rota completa (origem → destinos → origem)  
- **Benefício:** Oferece uma estimativa mais próxima da realidade para simulações e análises de desempenho

### 4. ✅ Validação dos Dados  
- **Decisão:** Validação com feedback visual  
- **Por que:** Melhora a experiência do usuário  
- **Benefício:** Reduz erros no preenchimento dos formulários e agiliza o uso do sistema

### 5. 📊 Relatório de Performance  
- **Decisão:** Métricas de eficiência baseadas em entregas/peso  
- **Por que:** Permite avaliar qual drone é mais produtivo  
- **Benefício:** Auxilia na tomada de decisões para ajustes na estratégia de alocação e uso dos drones

---

## 🛠️ Tecnologias Utilizadas

- **Backend:** ASP.NET Core 6.0 (C#)  
- **Frontend:** HTML5, CSS3, JavaScript (Vanilla)  
- **Arquitetura:** RESTful API  
- **Padrão:** MVC (Model-View-Controller)  
- **Serviços:** Singleton para gerenciamento de estado
- **ChatGPT:** Auxílio na criação da lógica de alocação de drones, validação de dados, organização do projeto e documentação técnica
- **Gemini:** Comparação de abordagens algorítmicas e revisão de estruturação de código
- **Cursor:** Ambiente de desenvolvimento com integração direta a IA para geração de código, sugestões de melhoria e automação de testes

## 💡 Exemplos de Prompts Utilizados
 **ChatGPT**
- Como distribuir pedidos entre 3 drones respeitando peso máximo e prioridade?
- Me ajude a calcular o tempo de entrega de um drone com velocidade constante
- Como organizar um projeto ASP.NET com MVC simples e singleton para armazenamento em memória?

 **Gemini**
- Como calcular rota de ida e volta do drone da forma mais eficiente?
- Melhores práticas para organizar regras de negócio em projetos com ASP.NET MVC


 **Cursor**
- Gere um método em C# para calcular distância euclidiana entre dois pontos
- Refatore esta classe DeliveryService para melhor legibilidade
- Crie um endpoint POST para receber pedidos com validação


---

## 🚀 Instruções para Executar

### ✅ Pré-requisitos

- [.NET 6.0 SDK ou superior](https://dotnet.microsoft.com/en-us/download)
- Navegador web moderno

---

### 📋 Passos para Execução

![A704125A-277A-44BA-8F3A-4357E034A03B_1_201_a](https://github.com/user-attachments/assets/638492b4-3cd0-415c-b095-61c3bd366050)

---

## 🧪 Como Usar a Aplicação

### 1️⃣ Cadastrar Pedido

- Preencha o formulário com:  
  - **Peso:** 0–10 kg  
  - **Coordenada X:** -100 a +100  
  - **Coordenada Y:** -100 a +100  
  - **Prioridade:** Número (maior = mais prioritário)  
- Clique em **"Cadastrar Pedido"**

### 2️⃣ Simular Entregas

- Clique em **"Simular Entrega"**  
- O sistema alocará automaticamente os pedidos entre os drones  
- Visualize a distribuição e pedidos não alocados (se houver)

### 3️⃣ Gerar Relatório

- Clique em **"Gerar Relatório"**  
- Veja estatísticas de performance:  
  - Total de entregas  
  - Tempo médio por entrega  
  - Drone mais eficiente  
  - Tempo detalhado por drone

### 4️⃣ Resetar Dados

- Clique em **"Resetar Dados"** para limpar tudo  
- Útil para testar novos cenários

---

## 🎮 Funcionalidades Principais

### 🧠 Validação Inteligente  
- Limites de peso e coordenadas em tempo real  
- Mensagens de erro claras  
- Prevenção de dados inválidos

### ⚙️ Alocação Otimizada  
- Distribuição automática entre drones  
- Respeito aos limites de peso  
- Priorização por prioridade e distância

### 📈 Relatórios Detalhados  
- Métricas de eficiência  
- Tempo de entrega por drone  
- Análise de performance

### 💻 Interface Responsiva  
- Design moderno e intuitivo  
- Feedback visual imediato  
- Compatível com diferentes dispositivos

---


