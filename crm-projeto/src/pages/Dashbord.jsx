import React, { useState, useEffect } from "react";
import { DragDropContext, Droppable, Draggable } from "@hello-pangea/dnd";
import "./Dashbord.css";
import api from "../services/api";

const DashboardPropostas = () => {
  const [tasks, setTasks] = useState({
    recebidos: [],
    validar: [],
    finalizadas: [],
  });
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);


  const determinarColuna = (cliente, propostasDoCliente) => {
    if (!propostasDoCliente || propostasDoCliente.length === 0) {
      return "recebidos";
    }

    const propostaMaisRecente = propostasDoCliente[0];
    const status = propostaMaisRecente.statusValidacao;

    if (status === "Finalizada") {
      return "finalizadas";
    }

    if (status === "Aguardando Validação") {
      return "validar";
    }
 // volta para a coluna "Recebidos" para ser trabalhado.
    return "recebidos";
  };

 
  useEffect(() => {
    const fetchLeadsEPropostas = async () => {
      setIsLoading(true);
      setError(null);
      try {
      
        const clientesResponse = await api.get("/api/Clientes");
        const clientes = clientesResponse.data || [];

        // Buscar todas as propostas
        const propostasResponse = await api.get("/api/Propostas");
        const todasPropostas = propostasResponse.data || [];

        // Agrupar propostas por cliente
        const propostasPorCliente = {};
        todasPropostas.forEach((proposta) => {
          const clienteId = proposta.clienteId || proposta.ClienteId || 0;
          if (!propostasPorCliente[clienteId]) {
            propostasPorCliente[clienteId] = [];
          }
          propostasPorCliente[clienteId].push(proposta);
        });

        // Ordenar propostas por data (mais recente primeiro)
        Object.keys(propostasPorCliente).forEach((clienteId) => {
          propostasPorCliente[clienteId].sort((a, b) => {
            const dataA = new Date(a.dataCriacao || a.DataCriacao || 0);
            const dataB = new Date(b.dataCriacao || b.DataCriacao || 0);
            return dataB - dataA;
          });
        });

        // Organizar leads por status
        const tasksByStatus = {
          recebidos: [],
          validar: [],
          finalizadas: [],
        };

        clientes.forEach((cliente) => {
          const propostasDoCliente = propostasPorCliente[cliente.id] || [];
          const coluna = determinarColuna(cliente, propostasDoCliente);
          
          const task = {
            id: `cliente-${cliente.id}`,
            nome: cliente.nome || "Sem nome",
            descricao: cliente.email || "Sem descrição",
            email: cliente.email,
            leadOriginal: cliente,
            propostaMaisRecente: propostasDoCliente[0] || null,
          };

          tasksByStatus[coluna].push(task);
        });

        setTasks(tasksByStatus);
      } catch (err) {
        console.error("Erro ao buscar dados:", err);
        setError("Erro ao carregar dados. Tente novamente.");
      } finally {
        setIsLoading(false);
      }
    };

    fetchLeadsEPropostas();
  }, []);

  const handleDragEnd = async (result) => {
    const { source, destination } = result;
    if (!destination) return;
    if (
      source.droppableId === destination.droppableId &&
      source.index === destination.index
    )
      return;

    const startList = Array.from(tasks[source.droppableId]);
    const [moved] = startList.splice(source.index, 1);
    const endList = Array.from(tasks[destination.droppableId]);
    endList.splice(destination.index, 0, moved);

    setTasks({
      ...tasks,
      [source.droppableId]: startList,
      [destination.droppableId]: endList,
    });

    if (destination.droppableId !== "recebidos" && !moved.propostaMaisRecente) {
      setTasks({
        ...tasks,
        [source.droppableId]: [...startList, moved],
        [destination.droppableId]: endList.filter((t) => t.id !== moved.id),
      });
      alert("É necessário gerar uma proposta antes de mover para esta coluna.");
      return;
    }

    if (moved.propostaMaisRecente) {
      let novoStatus;
      switch (destination.droppableId) {
        case "recebidos":
          novoStatus = "Pendente";
          break;
        case "validar":
          novoStatus = "Aguardando Validação";
          break;
        case "finalizadas":
          novoStatus = "Finalizada";
          break;
        default:
          novoStatus = "Pendente";
      }

      const propostaAtualizada = {
        ...moved.propostaMaisRecente,
        statusValidacao: novoStatus,
      };

      try {
        await api.put(`/api/Propostas/${moved.propostaMaisRecente.id}`, propostaAtualizada);
      } catch (err) {
        console.error("Erro ao atualizar proposta:", err);
        // Reverter mudança local em caso de erro
        setTasks({
          ...tasks,
          [source.droppableId]: [...startList, moved],
          [destination.droppableId]: endList.filter((t) => t.id !== moved.id),
        });
        alert("Erro ao atualizar status. Tente novamente.");
      }
    }
  };

  if (isLoading) {
    return (
      <div className="dashboard-container">
        <h1>Painel de Propostas</h1>
        <p>Carregando propostas...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="dashboard-container">
        <h1>Painel de Propostas</h1>
        <p style={{ color: "red" }}>{error}</p>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      <h1>Painel de Propostas</h1>

      {/* Cards de status */}
      <div className="status-cards">
        <div className="card recebidos">
          <h3>Recebidos</h3>
          <div
            className="progress-bar"
            style={{ width: `${tasks.recebidos.length * 20}%` }}
          />
          <p>{tasks.recebidos.length} leads aguardando processamento</p>
        </div>
        <div className="card validar">
          <h3>Para Validar</h3>
          <div
            className="progress-bar validar-bar"
            style={{ width: `${tasks.validar.length * 25}%` }}
          />
          <p>{tasks.validar.length} propostas aguardando validação</p>
        </div>
        <div className="card finalizadas">
          <h3>Concluídas</h3>
          <div
            className="progress-bar finalizadas-bar"
            style={{ width: `${tasks.finalizadas.length * 33}%` }}
          />
          <p>{tasks.finalizadas.length} propostas concluídas</p>
        </div>
      </div>

      {/* Kanban */}
      <DragDropContext onDragEnd={handleDragEnd}>
        <div className="kanban-container">
          {Object.entries(tasks).map(([status, lista]) => (
            <div className="kanban-column" key={status}>
              <h2 className={status}>{status}</h2>
              <Droppable droppableId={status}>
                {(provided) => (
                  <div
                    ref={provided.innerRef}
                    {...provided.droppableProps}
                    className="task-list"
                  >
                    {lista.map((task, index) => (
                      <Draggable
                        key={task.id}
                        draggableId={task.id}
                        index={index}
                      >
                        {(provided2) => (
                          <div
                            ref={provided2.innerRef}
                            {...provided2.draggableProps}
                            {...provided2.dragHandleProps}
                            className={`task-card ${status}`}
                          >
                            <strong className="task-name">{task.nome}</strong>
                            <p className="task-desc">{task.descricao}</p>
                            <div className="task-footer">
                              <div className="avatar">
                                {task.nome.charAt(0)}
                              </div>
                              <span className="task-id">ID: {task.id}</span>
                            </div>
                          </div>
                        )}
                      </Draggable>
                    ))}
                    {provided.placeholder}
                  </div>
                )}
              </Droppable>
            </div>
          ))}
        </div>
      </DragDropContext>
    </div>
  );
};

export default DashboardPropostas;
