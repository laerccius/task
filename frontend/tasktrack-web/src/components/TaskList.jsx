import React from 'react';
import ReactDOM from 'react-dom/client';
export function TaskList({ tasks, onEdit, onDelete }) {
  if (!tasks.length) {
    return <p className="empty-state">No tasks yet. Create one to get started.</p>;
  }

  return (
    <div className="task-list">
      {tasks.map((task) => (
        <article key={task.id} className="task-card">
          <div className="task-card-header">
            <div>
              <h3>{task.title}</h3>
              <p className="task-meta">{task.status} · Due {new Date(task.dueDate).toLocaleDateString()}</p>
            </div>
            <div className="button-row">
              <button className="ghost-button" onClick={() => onEdit(task)}>Edit</button>
              <button className="danger-button" onClick={() => onDelete(task.id)}>Delete</button>
            </div>
          </div>
          <p>{task.description || 'No description provided.'}</p>
        </article>
      ))}
    </div>
  );
}
