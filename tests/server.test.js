const request = require('supertest');
const express = require('express');
const notesRouter = require('../src/routes/notes');

const app = express();
app.use(express.json());
app.use('/api/notes', notesRouter);

describe('Notes API', () => {
  test('GET /api/notes returns array', async () => {
    const res = await request(app).get('/api/notes');
    expect(res.statusCode).toBe(200);
    expect(Array.isArray(res.body)).toBe(true);
  });
});
