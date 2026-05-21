import { Box, Chip, Typography } from '@mui/material'
import { DataGrid } from '@mui/x-data-grid'
import type { GridColDef, GridEventListener } from '@mui/x-data-grid'
import type { Task } from '../types/task'
import { formatDate } from '../utils/taskDate'

type TaskGridProps = {
  loading: boolean
  tasks: Task[]
  onRowClick: (task: Task) => void
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

export function TaskGrid({ loading, tasks, onRowClick }: TaskGridProps) {
  const rows = tasks.map((task) => ({
    ...task,
    dueDate: formatDate(task.dueDate),
    createdAt: formatDate(task.createdAt),
  }))

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
    if (task) onRowClick(task)
  }

  return (
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
  )
}
