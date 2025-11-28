import React, { forwardRef } from "react";
import "./StatusCards.css";

const StatusCard = forwardRef(({ title, count, color, children }, ref) => {
  return (
    <div className={`status-card ${color}`} ref={ref}>
      <h3 className="card-title">{title}</h3>
      <p className="card-count">{count}</p>
      <div className="tasks-container">{children}</div>
    </div>
  );
});

export default StatusCard;
