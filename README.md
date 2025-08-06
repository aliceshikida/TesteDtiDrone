# Teste Prático – DTI Digital  
## **Simulador de Encomendas com Drones**



## Restrições Assumidas

### Limite de Peso por Drone  
- **Capacidade máxima:** 10 kg por drone - Caso ultrapasse, uma notificação aparece.

### Sistema de Prioridade  
- **Escala:** Numérica (quanto maior o número, maior a prioridade)  
- **Aplicação:** Pedidos com prioridade mais alta são atendidos primeiro  
- **Critério secundário:** Distância (pedidos mais próximos têm preferência)

### Limite de Distância  
- **Coordenadas válidas:**  
  - X: -100 a +100  - Caso ultrapasse, uma notificação aparece.
  - Y: -100 a +100  - Caso ultrapasse, uma notificação aparece.
- **Cálculo da distância:** Euclidiana a partir da origem (0, 0)

### Limite de Drones  
- **Máximo de drones simultâneos:** 3  
- **Motivo:** Simular uma frota pequena e gerenciável
- *Caso não haja mais drones disponiveis, uma mensagem de ATENÇÃO aparecerá na caixa de Pedidos Realizados, mostrando quais os pedidos foram barrados, excluindo-os da Simulação de Entregas*

---

## Decisões do Projeto

### 1. Estratégia de Alocação: *"Menos Entregas"*  - Alocar pedidos no drone com menos entregas atuais  
- **Por que:** Distribui a carga uniformemente entre os drones  
- **Benefício:** Evita sobrecarregar um único drone e otimiza o tempo total

### 2. Persistência de Dados - Armazenamento em memória com reset manual  
- **Por que:** Simplicidade para demonstração e testes  
- **Benefício:** Permite reiniciar facilmente e testar diferentes cenários

### 3. Cálculo de Tempo Realista  
- **Por que:** Velocidade típica de drones de entrega urbanos  
- **Benefício:** Oferece uma estimativa mais próxima da realidade para simulações e análises de desempenho

### 4. Validação dos Dados - Validação com feedback visual  
- **Por que:** Melhora a experiência do usuário  
- **Benefício:** Reduz erros no preenchimento dos formulários e agiliza o uso do sistema

### 5. Relatório de Performance - Métricas de eficiência baseadas em entregas/peso  
- **Por que:** Permite avaliar qual drone é mais produtivo  
- **Benefício:** Auxilia na tomada de decisões para ajustes na estratégia de alocação e uso dos drones

---

## Tecnologias Utilizadas

- **Backend:** ASP.NET Core 6.0 (C#)  
- **Frontend:** HTML5, CSS3, JavaScript (Vanilla)  
- **Arquitetura:** RESTful API  
- **Padrão:** MVC (Model-View-Controller)  
- **Serviços:** Singleton para gerenciamento de estado
- **ChatGPT:** Auxílio na criação da lógica de alocação de drones, validação de dados, organização do projeto e documentação técnica
- **Gemini:** Comparação de abordagens algorítmicas e revisão de estruturação de código
- **Cursor:** Ambiente de desenvolvimento com integração direta a IA para geração de código, sugestões de melhoria e automação de testes

## Alguns Prompts Utilizados
 **ChatGPT**
- Como distribuir pedidos entre 3 drones respeitando peso máximo e prioridade?
- Como calcular o tempo de entrega de um drone com velocidade constante
- Como organizar um projeto ASP.NET com MVC simples e singleton para armazenamento em memória?

 **Gemini**
- Como calcular rota de ida e volta do drone da forma mais eficiente?
- Melhores práticas para organizar regras de negócio em projetos com ASP.NET MVC


 **Cursor**
- Gere um método em C# para calcular distância euclidiana entre dois pontos
- Refatore esta classe DeliveryService para melhor legibilidade
- Crie um endpoint POST para receber pedidos com validação


---

## Instruções para Executar

### Pré-requisitos

- [.NET 6.0 SDK ou superior](https://dotnet.microsoft.com/en-us/download)
- Navegador web moderno

---

### 👇​ Passos para Execução

**1. Clone o repositório**
  ```
   git clone https://github.com/aliceshikida/TesteDtiDrone.git
   cd TesteDtiDrone
   ```
**2. Navegue para o projeto**
  ```
   cd DroneDeliverySimulator
   ```
**3. Execute a aplicação**
 ```
  dotnet run

   ```
**4. Acesse o navegador**
```
  http://localhost:5290

   ```

##​ Como Usar a Aplicação

### 1. Cadastrar Pedido

- Preencha o formulário com:  
  - **Peso:** 0–10 kg  
  - **Coordenada X:** -100 a +100  
  - **Coordenada Y:** -100 a +100  
  - **Prioridade:** Número (maior = mais prioritário)  
- Clique em **"Cadastrar Pedido"**

<img src="https://github.com/user-attachments/assets/a74084c1-3f4a-47d7-b03a-4a1262636de3" width="400"/>
  
### 2. Simular Entregas

- Clique em **"Simular Entrega"**  
- O sistema alocará automaticamente os pedidos entre os drones  
- Visualize a distribuição e pedidos não alocados, arrastando um pouco pra baixo. (se houver)
 <img src="https://github.com/user-attachments/assets/9555fd6a-4d6f-4c22-b8f8-b1a1abbfe68a" width="400"/>
  <img src="https://github.com/user-attachments/assets/a7cde1cf-78cd-45bb-a098-0619d216e10b" width="400"/>
  
### 3. Gerar Relatório

- Clique em **"Gerar Relatório"**  
- Veja estatísticas de performance:  
  - Total de entregas  
  - Tempo médio por entrega  
  - Drone mais eficiente  
  - Tempo detalhado por drone

<img src="https://github.com/user-attachments/assets/a32f86c4-a950-4ccf-9155-dd5cf93d35e6" width="400"/>


### 4. Resetar Dados

- Clique em **"Resetar Dados"** para limpar tudo.
- Os dados só conseguem ser resetados usando esse botão.
- Útil para testar novos cenários
<img src="https://github.com/user-attachments/assets/91bffef1-dbfb-4f92-b39f-f1dbeb9953f2" width="400"/>



---

## Funcionalidades Principais

### Validação Inteligente  
- Limites de peso e coordenadas em tempo real  
- Mensagens de erro claras  
- Prevenção de dados inválidos

### Alocação Otimizada  
- Distribuição automática entre drones  
- Respeito aos limites de peso  
- Priorização por prioridade e distância

### Relatórios Detalhados  
- Métricas de eficiência  
- Tempo de entrega por drone  
- Análise de performance

### Interface Responsiva  
- Design moderno e intuitivo  
- Feedback visual imediato  
- Compatível com diferentes dispositivos

---


