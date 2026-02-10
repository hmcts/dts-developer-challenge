const express = require('express');
const notesRouter = require('./routes/notes');
const app = express();

app.use(express.json());
app.use('/api/notes', notesRouter);

const port = process.env.PORT || 3000;
app.listen(port, () => console.log(`Server listening on ${port}`));

module.exports = app;
