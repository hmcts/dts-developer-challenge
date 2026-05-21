import { useEffect, useState } from 'react'
import {
  AppBar,
  Alert,
  Box,
  Button,
  Container,
  CssBaseline,
  Paper,
  Snackbar,
  Stack,
  Toolbar,
  Typography,
} from '@mui/material'
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider'
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns'
import enGB from 'date-fns/locale/en-GB'
import { deleteTask, fetchTasks, saveTask } from './api/tasks'
import { TaskDialog } from './components/TaskDialog'
import { TaskGrid } from './components/TaskGrid'
import { emptyTaskForm, statusOptions, type Task, type TaskForm } from './types/task'
import { formatInputDate } from './utils/taskDate'
import './App.css'

const chromeSx = {
  bgcolor: '#1976d2',
  color: '#fff',
}

function App() {
  const [tasks, setTasks] = useState<Task[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingTask, setEditingTask] = useState<Task | null>(null)
  const [formTask, setFormTask] = useState<TaskForm>(emptyTaskForm)

  const loadTasks = async () => {
    setLoading(true)
    try {
      const data = await fetchTasks()
      setTasks(data)
    } catch (err) {
      setError((err as Error).message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const load = async () => {
      await loadTasks()
    }
    void load()
  }, [])

  const openDialog = (task?: Task) => {
    if (task) {
      setEditingTask(task)
      setFormTask({
        title: task.title,
        description: task.description ?? '',
        dueDate: formatInputDate(task.dueDate),
        status: statusOptions.includes(task.status) ? task.status : 'Pending',
      })
    } else {
      setEditingTask(null)
      setFormTask(emptyTaskForm)
    }
    setDialogOpen(true)
  }

  const closeDialog = () => {
    setDialogOpen(false)
    setEditingTask(null)
    setFormTask(emptyTaskForm)
  }

  const handleSaveTask = async () => {
    if (!formTask.title.trim() || !formTask.dueDate) {
      setError('Title and due date are required.')
      return
    }

    const payload = {
      title: formTask.title.trim(),
      description: formTask.description.trim() || null,
      dueDate: formTask.dueDate,
      status: formTask.status,
    }

    try {
      const savedTask = await saveTask(payload, editingTask?.id)
      setTasks((current) => {
        if (editingTask) {
          return current.map((task) => (task.id === savedTask.id ? savedTask : task))
        }
        return [savedTask, ...current]
      })
      setSuccess(editingTask ? 'Task updated successfully' : 'Task created successfully')
      closeDialog()
    } catch (err) {
      setError((err as Error).message)
    }
  }

  const handleDeleteTask = async () => {
    if (!editingTask) return
    try {
      await deleteTask(editingTask.id)
      setTasks((current) => current.filter((task) => task.id !== editingTask.id))
      setSuccess('Task deleted successfully')
      closeDialog()
    } catch (err) {
      setError((err as Error).message)
    }
  }

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={enGB}>
      <Box sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
        <CssBaseline />
        <AppBar position="static" sx={chromeSx}>
          <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              Task Manager
            </Typography>
            <Button color="inherit" onClick={loadTasks} disabled={loading}>
              Refresh
            </Button>
          </Toolbar>
        </AppBar>
        <Container maxWidth="lg" sx={{ py: 4, flex: 1 }}>
          <Stack spacing={4}>
            <Paper sx={{ p: 3 }} elevation={3}>
              <Stack direction={{ xs: 'column', sm: 'row' }} alignItems="center" justifyContent="space-between" spacing={2}>
                <Box>
                  <Typography variant="h5">All Tasks</Typography>
                  <Typography color="text.secondary" variant="body2">
                    {tasks.length} task{tasks.length === 1 ? '' : 's'}
                  </Typography>
                </Box>
                <Button variant="contained" onClick={() => openDialog()}>
                  Create new task
                </Button>
              </Stack>

              <TaskGrid loading={loading} tasks={tasks} onRowClick={openDialog} />
            </Paper>
          </Stack>
        </Container>
        <TaskDialog
          open={dialogOpen}
          editing={Boolean(editingTask)}
          formTask={formTask}
          onClose={closeDialog}
          onDelete={handleDeleteTask}
          onSave={handleSaveTask}
          onFormChange={setFormTask}
        />

        <Snackbar open={Boolean(error)} autoHideDuration={5000} onClose={() => setError(null)}>
          <Alert severity="error" sx={{ width: '100%' }} onClose={() => setError(null)}>
            {error}
          </Alert>
        </Snackbar>
        <Snackbar open={Boolean(success)} autoHideDuration={4000} onClose={() => setSuccess(null)}>
          <Alert severity="success" sx={{ width: '100%' }} onClose={() => setSuccess(null)}>
            {success}
          </Alert>
        </Snackbar>
      </Box>
    </LocalizationProvider>
  )
}

export default App
