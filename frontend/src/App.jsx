import React, { useEffect, useState } from 'react'

export default function App() {
  const [notes, setNotes] = useState([])

  useEffect(() => {
    fetch('/api/notes')
      .then((r) => r.json())
      .then(setNotes)
      .catch(() => setNotes([]))
  }, [])

  return (
    <div style={{ padding: 20 }}>
      <h1>Notes</h1>
      <ul>
        {notes.map((n) => (
          <li key={n.id}>{n.text}</li>
        ))}
      </ul>
    </div>
  )
}
