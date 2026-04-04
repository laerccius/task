import { useEffect, useState } from 'react';
import { AuthPanel } from './components/AuthPanel';
import { TaskForm } from './components/TaskForm';
import { TaskList } from './components/TaskList';
import { api } from './services/api';

const emptyTask = {
  title: '',
  description: '',
  status: 1,
  dueDate: ''
};

export default function App() {
  const [auth, setAuth] = useState(() => {
    const raw = window.localStorage.getItem('tasktrack-auth');
    return raw ? JSON.parse(raw) : null;
  });
  const [tasks, setTasks] = useState([]);
  const [draft, setDraft] = useState(emptyTask);
  const [editingId, setEditingId] = useState(null);
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!auth?.token) {
      return;
    }

    window.localStorage.setItem('tasktrack-auth', JSON.stringify(auth));
    loadTasks();
  }, [auth]);

  async function loadTasks() {
    try {
      setLoading(true);
      const data = await api.getTasks(auth.token);
      setTasks(data);
    } catch (error) {
      setMessage(error.message);
    } finally {
      setLoading(false);
    }
  }

  async function handleAuth(mode, payload) {
    try {
      const nextAuth = mode === 'login'
        ? await api.login(payload)
        : await api.register(payload);
      setAuth(nextAuth);
      setMessage(`Welcome, ${nextAuth.fullName}.`);
    } catch (error) {
      setMessage(error.message);
    }
  }

  async function handleSubmit(event) {
    event.preventDefault();
    try {
      if (editingId) {
        await api.updateTask(editingId, draft, auth.token);
        setMessage('Task updated.');
      } else {
        await api.createTask(draft, auth.token);
        setMessage('Task created.');
      }

      setDraft(emptyTask);
      setEditingId(null);
      await loadTasks();
    } catch (error) {
      setMessage(error.message);
    }
  }

  async function handleDelete(id) {
    try {
      await api.deleteTask(id, auth.token);
      setMessage('Task deleted.');
      await loadTasks();
    } catch (error) {
      setMessage(error.message);
    }
  }

  function handleEdit(task) {
    setEditingId(task.id);
    setDraft({
      title: task.title,
      description: task.description,
      status: task.status === 'Pending' ? 1 : task.status === 'InProgress' ? 2 : 3,
      dueDate: task.dueDate.slice(0, 10)
    });
  }

  function handleLogout() {
    setAuth(null);
    setTasks([]);
    setDraft(emptyTask);
    setEditingId(null);
    setMessage('Signed out.');
    window.localStorage.removeItem('tasktrack-auth');
  }

  return (
    <main className="app-shell">
      <section className="hero-card">
        <p className="eyebrow">Developer Test Project</p>
        <h1>TaskTrack</h1>
        <p className="hero-copy">
          A small task management app built to demonstrate clean backend boundaries,
          authentication, CRUD flows, and a practical frontend integration.
        </p>
        <div className="demo-badge">
          Demo login: <strong>demo@tasktrack.local / Demo123!</strong>
        </div>
      </section>

      {!auth ? (
        <AuthPanel onSubmit={handleAuth} message={message} />
      ) : (
        <section className="workspace-grid">
          <div className="panel">
            <div className="panel-header">
              <div>
                <p className="eyebrow">Authenticated</p>
                <h2>{auth.fullName}</h2>
              </div>
              <button className="ghost-button" onClick={handleLogout}>Log out</button>
            </div>
            <TaskForm
              draft={draft}
              setDraft={setDraft}
              onSubmit={handleSubmit}
              isEditing={Boolean(editingId)}
              onCancel={() => {
                setDraft(emptyTask);
                setEditingId(null);
              }}
            />
          </div>

          <div className="panel">
            <div className="panel-header">
              <div>
                <p className="eyebrow">Your Tasks</p>
                <h2>{loading ? 'Loading...' : `${tasks.length} items`}</h2>
              </div>
            </div>
            <TaskList tasks={tasks} onEdit={handleEdit} onDelete={handleDelete} />
          </div>
        </section>
      )}

      {message ? <p className="status-message">{message}</p> : null}
    </main>
  );
}
