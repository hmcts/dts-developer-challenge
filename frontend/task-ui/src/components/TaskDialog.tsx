import { format as formatDateValue } from 'date-fns'
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from '@mui/material'
import { DatePicker } from '@mui/x-date-pickers/DatePicker'
import { statusOptions, type TaskForm } from '../types/task'

type TaskDialogProps = {
  open: boolean
  editing: boolean
  formTask: TaskForm
  onClose: () => void
  onDelete: () => void
  onSave: () => void
  onFormChange: (task: TaskForm) => void
}

const chromeSx = {
  bgcolor: '#1976d2',
  color: '#fff',
}

export function TaskDialog({
  open,
  editing,
  formTask,
  onClose,
  onDelete,
  onSave,
  onFormChange,
}: TaskDialogProps) {
  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle
        sx={{
          ...chromeSx,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
        }}
      >
        {editing ? 'Edit Task' : 'Create Task'}
        <IconButton edge="end" size="small" onClick={onClose} aria-label="close" sx={{ color: '#fff' }}>
          ×
        </IconButton>
      </DialogTitle>
      <DialogContent dividers>
        <Stack spacing={3} sx={{ mt: 1 }}>
          <TextField
            label="Title"
            value={formTask.title}
            onChange={(event) => onFormChange({ ...formTask, title: event.target.value })}
            fullWidth
            required
          />
          <TextField
            label="Description"
            value={formTask.description}
            onChange={(event) => onFormChange({ ...formTask, description: event.target.value })}
            multiline
            rows={4}
            fullWidth
          />
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <DatePicker
              label="Due date"
              value={formTask.dueDate ? new Date(`${formTask.dueDate}T00:00:00`) : null}
              onChange={(value) =>
                onFormChange({
                  ...formTask,
                  dueDate: value ? formatDateValue(value, 'yyyy-MM-dd') : '',
                })
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
                onChange={(event) => onFormChange({ ...formTask, status: event.target.value })}
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
        {editing ? (
          <Button color="error" variant="contained" onClick={onDelete}>
            Delete
          </Button>
        ) : (
          <Box />
        )}
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button color="inherit" onClick={onClose}>
            Cancel
          </Button>
          <Button variant="contained" color="inherit" onClick={onSave} sx={{ color: '#1976d2' }}>
            {editing ? 'Save changes' : 'Create task'}
          </Button>
        </Box>
      </DialogActions>
    </Dialog>
  )
}
