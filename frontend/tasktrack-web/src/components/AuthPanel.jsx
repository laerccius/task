import { useState } from 'react';
import React from 'react';
import ReactDOM from 'react-dom/client';
export function AuthPanel({ onSubmit, message }) {
  const [mode, setMode] = useState('login');
  const [form, setForm] = useState({
    fullName: '',
    email: 'demo@tasktrack.local',
    password: 'Demo123!'
  });

  return (
    <section className="auth-card">
      <div className="panel-header">
        <div>
          <p className="eyebrow">Access</p>
          <h2>{mode === 'login' ? 'Sign in' : 'Create account'}</h2>
        </div>
        <button
          className="ghost-button"
          onClick={() => setMode(mode === 'login' ? 'register' : 'login')}
        >
          {mode === 'login' ? 'Need an account?' : 'Use existing account'}
        </button>
      </div>

      <form
        className="stack-form"
        onSubmit={(event) => {
          event.preventDefault();
          onSubmit(mode, form);
        }}
      >
        {mode === 'register' ? (
          <label>
            Full name
            <input
              value={form.fullName}
              onChange={(event) => setForm({ ...form, fullName: event.target.value })}
              required
            />
          </label>
        ) : null}

        <label>
          Email
          <input
            type="email"
            value={form.email}
            onChange={(event) => setForm({ ...form, email: event.target.value })}
            required
          />
        </label>

        <label>
          Password
          <input
            type="password"
            value={form.password}
            onChange={(event) => setForm({ ...form, password: event.target.value })}
            required
          />
        </label>

        <button className="primary-button" type="submit">
          {mode === 'login' ? 'Login' : 'Register'}
        </button>
      </form>

      {message ? <p className="status-message compact">{message}</p> : null}
    </section>
  );
}
