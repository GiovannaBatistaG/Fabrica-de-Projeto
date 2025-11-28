import React, { forwardRef } from "react";
import { Card, CardContent, Typography, Button } from "@mui/material";
import "./TaksCard.css";


const TaskCard = forwardRef(({ task, ...props }, ref) => {
  return (
    <Card className="task-card"
      ref={ref}
      {...props} 
      sx={{ marginBottom: 1, cursor: "grab" }}
    >
      <CardContent className="card">
        <Typography variant="subtitle1">{task.nome}</Typography>
        <Typography variant="caption" display="block">
          Criado em: {task.data}
        </Typography>
        <Button size="small">Ver Cliente</Button>
      </CardContent>
    </Card>
  );
});

export default TaskCard;
