import React from 'react';
import ReactDOM from 'react-dom/client';
export function TaskForm({ draft, setDraft, onSubmit, isEditing, onCancel }) {
  return (
    <form className="stack-form" onSubmit={onSubmit}>
      <label>
        Title
        <input
          value={draft.title}
          onChange={(event) => setDraft({ ...draft, title: event.target.value })}
          required
        />
      </label>

      <label>
        Description
        <textarea
          rows="4"
          value={draft.description}
          onChange={(event) => setDraft({ ...draft, description: event.target.value })}
        />
      </label>

      <div className="split-fields">
        <label>
          Status
          <select
            value={draft.status}
            onChange={(event) => setDraft({ ...draft, status: Number(event.target.value) })}
          >
            <option value="1">Pending</option>
            <option value="2">In Progress</option>
            <option value="3">Completed</option>
          </select>
        </label>

        <label>
          Due date
          <input
            type="date"
            value={draft.dueDate}
            onChange={(event) => setDraft({ ...draft, dueDate: event.target.value })}
            required
          />
        </label>
      </div>

      <div className="button-row">
        <button className="primary-button" type="submit">
          {isEditing ? 'Save changes' : 'Create task'}
        </button>
        {isEditing ? (
          <button className="ghost-button" type="button" onClick={onCancel}>
            Cancel
          </button>
        ) : null}
      </div>
    </form>
  );
}
