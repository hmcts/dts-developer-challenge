const express = require('express');
const cors = require('cors');

const app = express();

app.use(cors());
app.use(express.json());

let tasks = [];
let nextId = 1;

app.get('/', (req, res) => res.send('Backend server is running!'));

app.get('/tasks', (req, res) => {
  try {
    if (!tasks.length) {
      return res.status(200).json({ message: 'No tasks found', tasks: [] });
    }
    res.status(200).json(tasks);
  } catch (err) {
    console.error('Error fetching tasks:', err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

app.post('/tasks', (req, res) => {
  try {
    const { title, dueDate } = req.body;

    if (!title || !title.trim()) {
      return res.status(400).json({ error: 'Title is required' });
    }

    if (!dueDate || isNaN(Date.parse(dueDate))) {
      return res.status(400).json({ error: 'Invalid due date' });
    }

    const task = {
      id: nextId++,
      title: title.trim(),
      dueDate,
      completed: false
    };

    tasks.push(task);
    res.status(201).json(task);
  } catch (err) {
    console.error('Error creating task:', err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

app.put('/tasks/:id', (req, res) => {
  try {
    const taskId = parseInt(req.params.id, 10);
    const { title, dueDate, completed } = req.body;

    const task = tasks.find(t => t.id === taskId);
    if (!task) return res.status(404).json({ error: 'Task not found' });

    if (title) task.title = title.trim();
    if (dueDate && !isNaN(Date.parse(dueDate))) task.dueDate = dueDate;
    if (typeof completed === 'boolean') task.completed = completed;

    res.json(task);
  } catch (err) {
    console.error('Error updating task:', err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

app.delete('/tasks/:id', (req, res) => {
  try {
    const taskId = parseInt(req.params.id, 10);
    const index = tasks.findIndex(t => t.id === taskId);

    if (index === -1) {
      return res.status(404).json({ error: 'Task not found' });
    }

    const deletedTask = tasks.splice(index, 1)[0];
    res.json({ message: 'Task deleted successfully', task: deletedTask });
  } catch (err) {
    console.error('Error deleting task:', err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

module.exports = app;

if (require.main === module) {
  const PORT = 5000;
  app.listen(PORT, () => console.log(`Server running on port ${PORT}`));
}