import type { SaveTaskRequest, Task } from '../types/task'

const apiBase = '/api/tasks'

async function parseError(response: Response, fallbackMessage: string) {
  try {
    const payload = (await response.json()) as { message?: string }
    return payload.message ?? fallbackMessage
  } catch {
    return fallbackMessage
  }
}

async function readJson<T>(response: Response, fallbackMessage: string): Promise<T> {
  if (!response.ok) {
    throw new Error(await parseError(response, fallbackMessage))
  }

  return (await response.json()) as T
}

export async function fetchTasks(): Promise<Task[]> {
  const response = await fetch(apiBase)
  return readJson<Task[]>(response, 'Unable to load tasks')
}

export async function createTask(payload: SaveTaskRequest): Promise<Task> {
  const response = await fetch(apiBase, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })

  return readJson<Task>(response, 'Failed to save task')
}

export async function updateTask(taskId: number, payload: SaveTaskRequest): Promise<Task> {
  const response = await fetch(`${apiBase}/${taskId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })

  return readJson<Task>(response, 'Failed to save task')
}

export async function saveTask(payload: SaveTaskRequest, taskId?: number): Promise<Task> {
  return typeof taskId === 'number' ? updateTask(taskId, payload) : createTask(payload)
}

export async function deleteTask(taskId: number): Promise<void> {
  const response = await fetch(`${apiBase}/${taskId}`, {
    method: 'DELETE',
  })

  if (!response.ok) {
    throw new Error(await parseError(response, 'Failed to delete task'))
  }
}
