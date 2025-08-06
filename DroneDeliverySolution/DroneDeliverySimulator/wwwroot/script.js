const apiBaseUrl = 'http://localhost:5290/api/pedidos';

const pedidoForm = document.getElementById('pedidoForm');
const pedidoListPendentes = document.getElementById('pedidoListPendentes');
const btnSimular = document.getElementById('btnSimular');
const btnResetar = document.getElementById('btnResetar');
const btnRelatorio = document.getElementById('btnRelatorio');
const simulationResult = document.getElementById('simulationResult');

// Elementos para validação
const pesoInput = document.getElementById('peso');
const xInput = document.getElementById('x');
const yInput = document.getElementById('y');
const pesoError = document.getElementById('pesoError');
const xError = document.getElementById('xError');
const yError = document.getElementById('yError');

// Atualiza lista de pedidos pendentes
async function atualizarPendentes() {
  pedidoListPendentes.innerHTML = 'Carregando...';
  try {
    const res = await fetch(`${apiBaseUrl}/pendentes`);
    if (!res.ok) {
      pedidoListPendentes.innerHTML = '<li>Não foi possível carregar os pedidos pendentes.</li>';
      return;
    }
    const pedidos = await res.json();

    if (pedidos.length === 0) {
      pedidoListPendentes.innerHTML = '<li>Nenhum pedido pendente.</li>';
      return;
    }

    pedidoListPendentes.innerHTML = '';
    pedidos.forEach(p => {
      const li = document.createElement('li');
      li.textContent = `#${p.id} - Peso: ${p.peso}kg - Destino: (${p.destinoX}, ${p.destinoY}) - Prioridade: ${p.prioridade}`;
      pedidoListPendentes.appendChild(li);
    });
  } catch (err) {
    pedidoListPendentes.innerHTML = `<li>Erro ao carregar pedidos pendentes: ${err.message}</li>`;
  }
}

// Calcula o tempo estimado de entrega com base na distância
function calcularTempoEntrega(x, y) {
  const velocidade = 50; // unidades por hora
  const distancia = Math.sqrt(x * x + y * y);
  const tempoHoras = distancia / velocidade;
  const tempoMinutos = tempoHoras * 60;
  return tempoMinutos.toFixed(1); // 1 casa decimal
}

// Funções de validação
function validarPeso() {
  const peso = parseFloat(pesoInput.value);
  if (peso < 0 || peso > 10) {
    pesoError.textContent = 'Peso deve estar entre 0 e 10 kg';
    return false;
  } else {
    pesoError.textContent = '';
    return true;
  }
}

function validarCoordenadaX() {
  const x = parseInt(xInput.value);
  if (x < -100 || x > 100) {
    xError.textContent = 'Coordenada X deve estar entre -100 e 100';
    return false;
  } else {
    xError.textContent = '';
    return true;
  }
}

function validarCoordenadaY() {
  const y = parseInt(yInput.value);
  if (y < -100 || y > 100) {
    yError.textContent = 'Coordenada Y deve estar entre -100 e 100';
    return false;
  } else {
    yError.textContent = '';
    return true;
  }
}

function validarFormulario() {
  const pesoValido = validarPeso();
  const xValido = validarCoordenadaX();
  const yValido = validarCoordenadaY();
  return pesoValido && xValido && yValido;
}

// Evento para cadastrar pedido
pedidoForm.addEventListener('submit', async (e) => {
  e.preventDefault();

  // Validar formulário antes de enviar
  if (!validarFormulario()) {
    alert('Por favor, corrija os erros no formulário antes de continuar.');
    return;
  }

  const pedido = {
    peso: parseFloat(document.getElementById('peso').value),
    destinoX: parseInt(document.getElementById('x').value),
    destinoY: parseInt(document.getElementById('y').value),
    prioridade: document.getElementById('prioridade').value
  };

  try {
    const res = await fetch(apiBaseUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(pedido)
    });

    const text = await res.text();

    if (res.ok) {
      alert('Pedido cadastrado com sucesso!');
      pedidoForm.reset();
      atualizarPendentes();
    } else {
      alert(`Erro ao cadastrar pedido. Status: ${res.status}\nResposta: ${text}`);
    }
  } catch (err) {
    alert('Erro de conexão: ' + err.message);
  }
});

// Evento para simular entregas
btnSimular.addEventListener('click', async () => {
  simulationResult.innerHTML = 'Simulando...';

  try {
    const res = await fetch(`${apiBaseUrl}/simular`);
    if (!res.ok) {
      const errorText = await res.text();
      simulationResult.innerHTML = `<li>Erro na simulação: ${errorText}</li>`;
      return;
    }

    const mensagensSimulacao = await res.json();

    if (mensagensSimulacao.length === 0) {
      simulationResult.innerHTML = '<li>Nenhuma entrega para simular.</li>';
      return;
    }

    // Mostrar as mensagens de simulação (drones e viagens)
    simulationResult.innerHTML = '';
    mensagensSimulacao.forEach(msg => {
      const li = document.createElement('li');
      li.textContent = msg;
      simulationResult.appendChild(li);
    });

    // Atualiza a lista de pendentes (que agora inclui todos os pedidos)
    atualizarPendentes();
  } catch (err) {
    simulationResult.innerHTML = `<li>Erro de conexão: ${err.message}</li>`;
  }
});

// Evento para resetar dados
btnResetar.addEventListener('click', async () => {
  if (confirm('Tem certeza que deseja resetar todos os dados? Isso irá limpar todos os pedidos.')) {
    try {
      const res = await fetch(`${apiBaseUrl}/resetar`, {
        method: 'POST'
      });

      if (res.ok) {
        alert('Dados resetados com sucesso!');
        atualizarPendentes();
        simulationResult.innerHTML = '';
        document.getElementById('relatorioContent').innerHTML = '';
      } else {
        alert('Erro ao resetar dados.');
      }
    } catch (err) {
      alert('Erro de conexão: ' + err.message);
    }
  }
});

// Event listeners para validação em tempo real
pesoInput.addEventListener('input', validarPeso);
xInput.addEventListener('input', validarCoordenadaX);
yInput.addEventListener('input', validarCoordenadaY);

// Função para exibir relatório
function exibirRelatorio(relatorio) {
  const relatorioContent = document.getElementById('relatorioContent');
  
  let temposPorDroneHtml = '';
  if (relatorio.temposPorDrone && relatorio.temposPorDrone.length > 0) {
    temposPorDroneHtml = '<h4>⏱️ Tempo de Entrega por Drone:</h4>';
    relatorio.temposPorDrone.forEach(drone => {
      temposPorDroneHtml += `
        <div class="drone-info">
          <strong>Drone ${drone.droneId}:</strong> ${drone.entregas} entregas, 
          ${drone.pesoTotal.toFixed(1)}kg total, 
          ${drone.distancia.toFixed(1)}km percorridos, 
          <span class="tempo-destaque">${drone.tempo.toFixed(1)} minutos</span>
        </div>
      `;
    });
  }
  
  relatorioContent.innerHTML = `
    <h3>📊 Relatório de Performance</h3>
    <p><span class="metric">Quantidade de entregas realizadas:</span> ${relatorio.totalEntregas}</p>
    <p><span class="metric">Tempo médio por entrega:</span> ${relatorio.tempoMedio} minutos</p>
    <p><span class="metric">Drone mais eficiente:</span> ${relatorio.droneMaisEficiente}</p>
    ${temposPorDroneHtml}
  `;
}

// Evento para gerar relatório
btnRelatorio.addEventListener('click', async () => {
  try {
    const res = await fetch(`${apiBaseUrl}/relatorio`);
    
    if (res.ok) {
      const relatorio = await res.json();
      exibirRelatorio(relatorio);
    } else {
      alert('Erro ao gerar relatório');
    }
  } catch (err) {
    alert('Erro de conexão: ' + err.message);
  }
});

// Inicializa listas ao carregar página
document.addEventListener('DOMContentLoaded', () => {
  atualizarPendentes();
});