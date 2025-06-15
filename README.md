# DTS Developer Technical Test

## Objective
To assess your ability to build a simple API and frontend using best coding practices.

## Scenario
HMCTS requires a new system to be developed so caseworkers can keep track of their tasks. Your technical test is to develop that new system so caseworkers can efficiently manage their tasks.

## Task Requirements

### Backend API
const express = require('express');
const app = express();
const cors = require('cors');

app.use(cors());
app.use(express.json());

app.use('/api/tasks', require('./routes/tasks'));

app.listen(3001, () => console.log("Server running on port 3001"));


### Frontend Application
The frontend should be able to:
- Create, view, update, and delete tasks
- Display tasks in a user-friendly interface

### Basic Task Model (in-memory for simplicity)
// server/routes/tasks.js
const express = require('express');
const router = express.Router();

let tasks = []; // In-memory task store
let id = 1;

// Get all tasks
router.get('/', (req, res) => {
  res.json(tasks);
});

// Create a task
router.post('/', (req, res) => {
  const task = { id: id++, ...req.body };
  tasks.push(task);
  res.status(201).json(task);
});

// Update task
router.put('/:id', (req, res) => {
  const index = tasks.findIndex(t => t.id == req.params.id);
  if (index === -1) return res.sendStatus(404);
  tasks[index] = { ...tasks[index], ...req.body };
  res.json(tasks[index]);
});

// Delete task
router.delete('/:id', (req, res) => {
  tasks = tasks.filter(t => t.id != req.params.id);
  res.sendStatus(204);
});

module.exports = router;

###  Simple Frontend (React)
npx create-react-app client
cd client
npm install axios


import React, { useState, useEffect } from 'react';
import axios from 'axios';

function App() {
  const [tasks, setTasks] = useState([]);
  const [form, setForm] = useState({ title: '', status: 'To Do' });

  useEffect(() => {
    axios.get('http://localhost:3001/api/tasks').then(res => setTasks(res.data));
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    const res = await axios.post('http://localhost:3001/api/tasks', form);
    setTasks([...tasks, res.data]);
    setForm({ title: '', status: 'To Do' });
  };

  return (
    <div className="p-6 max-w-md mx-auto">
      <h1 className="text-xl font-bold mb-4">Caseworker Task Manager</h1>
      <form onSubmit={handleSubmit} className="mb-4">
        <input className="border p-2 w-full mb-2" value={form.title}
               onChange={e => setForm({ ...form, title: e.target.value })}
               placeholder="Task Title" required />
        <select className="border p-2 w-full mb-2" value={form.status}
                onChange={e => setForm({ ...form, status: e.target.value })}>
          <option>To Do</option>
          <option>In Progress</option>
          <option>Done</option>
        </select>
        <button className="bg-blue-600 text-white px-4 py-2 rounded">Add Task</button>
      </form>

      <ul>
        {tasks.map(task => (
          <li key={task.id} className="border p-2 mb-2">
            <strong>{task.title}</strong> - {task.status}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;

### How to run
### Backend
cd server
npm install
node index.js

### How to run
### Frontend
cd client
npm install
npm start
