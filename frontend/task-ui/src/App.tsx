import { useEffect, useMemo, useState } from 'react'
import {
  AppBar,
  Alert,
  Box,
  Button,
  Chip,
  Container,
  CssBaseline,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Snackbar,
  Stack,
  TextField,
  Toolbar,
  Typography,
} from '@mui/material'
import { DataGrid } from '@mui/x-data-grid'
import type { GridColDef, GridEventListener } from '@mui/x-data-grid'
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider'
import { DatePicker } from '@mui/x-date-pickers/DatePicker'
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns'
import { format as formatDateValue } from 'date-fns'
import './App.css'

type Task = {
  id: number
  title: string
  description: string | null
  status: string
  dueDate: string
  createdAt: string
  updatedAt: string | null
}

type TaskForm = {
  title: string
  description: string
  dueDate: string
  status: string
}

const statusOptions = ['Pending', 'In Progress', 'Completed']

function formatDate(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

function formatInputDate(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return ''
  return date.toISOString().slice(0, 10)
}

const emptyForm: TaskForm = {
  title: '',
  description: '',
  dueDate: '',
  status: 'Pending',
}

const chromeSx = {
  bgcolor: '#1976d2',
  color: '#fff',
}

const statusChipSx: Record<string, { bgcolor: string; color: string }> = {
  Pending: { bgcolor: '#fff3cd', color: '#7a5b00' },
  'In Progress': { bgcolor: '#dbeafe', color: '#1d4ed8' },
  Completed: { bgcolor: '#dcfce7', color: '#166534' },
}

function App() {
  const [tasks, setTasks] = useState<Task[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingTask, setEditingTask] = useState<Task | null>(null)
  const [formTask, setFormTask] = useState<TaskForm>(emptyForm)

  const apiBase = '/api/tasks'

  const fetchTasks = async () => {
    setLoading(true)
    try {
      const response = await fetch(apiBase)
      if (!response.ok) throw new Error('Unable to load tasks')
      const data = (await response.json()) as Task[]
      setTasks(data)
    } catch (err) {
      setError((err as Error).message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const load = async () => {
      await fetchTasks()
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
      setFormTask(emptyForm)
    }
    setDialogOpen(true)
  }

  const closeDialog = () => {
    setDialogOpen(false)
    setEditingTask(null)
    setFormTask(emptyForm)
  }

  const saveTask = async () => {
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
      const response = await fetch(editingTask ? `${apiBase}/${editingTask.id}` : apiBase, {
        method: editingTask ? 'PUT' : 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      })

      if (!response.ok) {
        const payloadResponse = await response.json()
        throw new Error(payloadResponse?.message || 'Failed to save task')
      }

      const savedTask = (await response.json()) as Task
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

  const deleteTask = async () => {
    if (!editingTask) return
    try {
      const response = await fetch(`${apiBase}/${editingTask.id}`, {
        method: 'DELETE',
      })
      if (!response.ok) {
        const payloadResponse = await response.json()
        throw new Error(payloadResponse?.message || 'Failed to delete task')
      }
      setTasks((current) => current.filter((task) => task.id !== editingTask.id))
      setSuccess('Task deleted successfully')
      closeDialog()
    } catch (err) {
      setError((err as Error).message)
    }
  }

  const rows = useMemo(
    () =>
      tasks.map((task) => ({
        ...task,
        dueDate: formatDate(task.dueDate),
        createdAt: formatDate(task.createdAt),
      })),
    [tasks],
  )

  const columns: GridColDef[] = [
    { field: 'title', headerName: 'Title', flex: 1, minWidth: 180 },
    {
      field: 'description',
      headerName: 'Description',
      flex: 1,
      minWidth: 240,
      renderCell: (params) => (
        <Typography variant="body2" noWrap title={params.value as string}>
          {params.value || '—'}
        </Typography>
      ),
    },
    { field: 'dueDate', headerName: 'Due date', width: 130 },
    {
      field: 'status',
      headerName: 'Status',
      width: 160,
      renderCell: (params) => {
        const status = params.value as string
        return (
          <Chip
            label={status}
            size="small"
            sx={statusChipSx[status] ?? { bgcolor: '#e5e7eb', color: '#374151' }}
          />
        )
      },
    },
    { field: 'createdAt', headerName: 'Created', width: 150 },
  ]

  const handleRowClick: GridEventListener<'rowClick'> = (params) => {
    const task = tasks.find((item) => item.id === params.id)
    if (task) openDialog(task)
  }

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <Box sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <CssBaseline />
      <AppBar position="static" sx={chromeSx}>
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Task Manager
          </Typography>
          <Button color="inherit" onClick={fetchTasks} disabled={loading}>
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

            <Box sx={{ mt: 3, width: '100%' }}>
              <DataGrid
                rows={rows}
                columns={columns}
                loading={loading}
                pageSizeOptions={[5, 10, 25]}
                initialState={{ pagination: { paginationModel: { pageSize: 10, page: 0 } } }}
                disableRowSelectionOnClick
                onRowClick={handleRowClick}
                autoHeight
                sx={{
                  '& .MuiDataGrid-columnHeaders': chromeSx,
                  '& .MuiDataGrid-columnHeader': {
                    color: '#fff',
                  },
                  '& .MuiDataGrid-columnSeparator': {
                    color: 'rgba(255, 255, 255, 0.35)',
                  },
                  '& .MuiDataGrid-sortIcon': {
                    color: '#fff',
                  },
                }}
              />
            </Box>
          </Paper>
        </Stack>
      </Container>
      <Dialog open={dialogOpen} onClose={closeDialog} fullWidth maxWidth="sm">
        <DialogTitle
          sx={{
            ...chromeSx,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
          }}
        >
          {editingTask ? 'Edit Task' : 'Create Task'}
          <IconButton edge="end" size="small" onClick={closeDialog} aria-label="close" sx={{ color: '#fff' }}>
            ×
          </IconButton>
        </DialogTitle>
        <DialogContent dividers>
          <Stack spacing={3} sx={{ mt: 1 }}>
            <TextField
              label="Title"
              value={formTask.title}
              onChange={(event) => setFormTask((current) => ({ ...current, title: event.target.value }))}
              fullWidth
              required
            />
            <TextField
              label="Description"
              value={formTask.description}
              onChange={(event) => setFormTask((current) => ({ ...current, description: event.target.value }))}
              multiline
              rows={4}
              fullWidth
            />
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <DatePicker
                label="Due date"
                value={formTask.dueDate ? new Date(`${formTask.dueDate}T00:00:00`) : null}
                onChange={(value) =>
                  setFormTask((current) => ({
                    ...current,
                    dueDate: value ? formatDateValue(value, 'yyyy-MM-dd') : '',
                  }))
                }
                slotProps={{
                  textField: {
                    fullWidth: true,
                    required: true,
                  },
                }}
              />
              <FormControl fullWidth>
                <InputLabel>Status</InputLabel>
                <Select
                  label="Status"
                  value={formTask.status}
                  onChange={(event) => setFormTask((current) => ({ ...current, status: event.target.value }))}
                >
                  {statusOptions.map((status) => (
                    <MenuItem key={status} value={status}>
                      {status}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions sx={{ ...chromeSx, justifyContent: 'space-between' }}>
          {editingTask ? (
            <Button color="error" variant="contained" onClick={deleteTask}>
              Delete
            </Button>
          ) : null}
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button color="inherit" onClick={closeDialog}>
              Cancel
            </Button>
            <Button variant="contained" color="inherit" onClick={saveTask} sx={{ color: '#1976d2' }}>
              {editingTask ? 'Save changes' : 'Create task'}
            </Button>
          </Box>
        </DialogActions>
      </Dialog>

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
