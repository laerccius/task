const resolvedApiOrigin = import.meta.env.VITE_API_URL || 'http://localhost:5050';
const API_BASE_URL = `${resolvedApiOrigin}/api`;

async function request(path, options = {}) {
  var headers = {
    ...(options.headers || {}),
    'Content-Type': 'application/json',
  }

  let req = {
    ...options,
    headers
  }

  const response = await fetch(`${API_BASE_URL}${path}`, req);

  if (!response.ok) {
    const payload = await response.json().catch(() => ({}));
    throw new Error(payload.message || 'Request failed.');
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export const api = {
  login: (payload) => request('/auth/login', { method: 'POST', body: JSON.stringify(payload) }),
  register: (payload) => request('/auth/register', { method: 'POST', body: JSON.stringify(payload) }),
  getTasks: (token) => request('/tasks', { headers: { Authorization: `Bearer ${token}` } }),
  createTask: (payload, token) => request('/tasks', {
    method: 'POST',
    headers: { Authorization: `Bearer ${token}` },
    body: JSON.stringify({
      ...payload,
      dueDate: new Date(payload.dueDate).toISOString()
    }),
  }),
  updateTask: (id, payload, token) => request(`/tasks/${id}`, {
    method: 'PUT',
    headers: { Authorization: `Bearer ${token}` },
    body: JSON.stringify({
      ...payload,
      dueDate: new Date(payload.dueDate).toISOString()
    })
  }),
  deleteTask: (id, token) => request(`/tasks/${id}`, {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` }
  })
};
