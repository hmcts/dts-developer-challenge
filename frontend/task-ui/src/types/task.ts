export type Task = {
  id: number
  title: string
  description: string | null
  status: string
  dueDate: string
  createdAt: string
  updatedAt: string | null
}

export type TaskForm = {
  title: string
  description: string
  dueDate: string
  status: string
}

export type SaveTaskRequest = {
  title: string
  description: string | null
  dueDate: string
  status: string
}

export const statusOptions = ['Pending', 'In Progress', 'Completed']

export const emptyTaskForm: TaskForm = {
  title: '',
  description: '',
  dueDate: '',
  status: 'Pending',
}
