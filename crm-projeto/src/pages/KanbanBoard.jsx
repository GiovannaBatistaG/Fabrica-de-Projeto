
import { useState } from "react";
import { DragDropContext, Droppable, Draggable } from "@hello-pangea/dnd";
import StatusCard from "./StatusCards.jsx";
import TaskCard from "./TaskCard.jsx";
import "./TaksCard.css";




export default function KanbanBoard() {
  const [tasks, setTasks] = useState({
    recebidos: [
      { id: 1, nome: "Cliente A", data: "2025-10-01" },
      { id: 2, nome: "Cliente B", data: "2025-10-03" },
    ],
    validar: [],
    enviados: [],
  });

  function handleDragEnd(result) {
    const { source, destination } = result;
    if (!destination) return;

    if (
      source.droppableId === destination.droppableId &&
      source.index === destination.index
    )
      return;

    const sourceList = Array.from(tasks[source.droppableId]);
    const [movedItem] = sourceList.splice(source.index, 1);

    const destList = Array.from(tasks[destination.droppableId]);
    destList.splice(destination.index, 0, movedItem);

    setTasks({
      ...tasks,
      [source.droppableId]: sourceList,
      [destination.droppableId]: destList,
    });
  }

  return (
    <DragDropContext onDragEnd={handleDragEnd}>
      <div className="status-cards-container">
        <Droppable droppableId="recebidos">
          {(provided) => (
            <StatusCard
              title="Forms Recebidos"
              color="card-blue"
              count={tasks.recebidos.length}
              ref={provided.innerRef}
              {...provided.droppableProps}
            >
              {tasks.recebidos.map((task, index) => (
                <Draggable
                  key={task.id}
                  draggableId={task.id.toString()}
                  index={index}
                >
                  {(provided2) => (
                    <TaskCard
                      task={task}
                      ref={provided2.innerRef}
                      {...provided2.draggableProps}
                      {...provided2.dragHandleProps}
                    />
                  )}
                </Draggable>
              ))}
              {provided.placeholder}
            </StatusCard>
          )}
        </Droppable>

        <Droppable droppableId="validar">
          {(provided) => (
            <StatusCard
              title="Para Validar"
              color="card-green"
              count={tasks.validar.length}
              ref={provided.innerRef}
              {...provided.droppableProps}
            >
              {tasks.validar.map((task, index) => (
                <Draggable
                  key={task.id}
                  draggableId={task.id.toString()}
                  index={index}
                >
                  {(provided2) => (
                    <TaskCard
                      task={task}
                      ref={provided2.innerRef}
                      {...provided2.draggableProps}
                      {...provided2.dragHandleProps}
                    />
                  )}
                </Draggable>
              ))}
              {provided.placeholder}
            </StatusCard>
          )}
        </Droppable>

        <Droppable droppableId="enviados">
          {(provided) => (
            <StatusCard
              title="Propostas Finalizadas"
              color="card-purple"
              count={tasks.enviados.length}
              ref={provided.innerRef}
              {...provided.droppableProps}
            >
              {tasks.enviados.map((task, index) => (
                <Draggable
                  key={task.id}
                  draggableId={task.id.toString()}
                  index={index}
                >
                  {(provided2) => (
                    <TaskCard
                      task={task}
                      ref={provided2.innerRef}
                      {...provided2.draggableProps}
                      {...provided2.dragHandleProps}
                    />
                  )}
                </Draggable>
              ))}
              {provided.placeholder}
            </StatusCard>
          )}
        </Droppable>
      </div>
    </DragDropContext>
  );
}

