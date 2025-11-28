const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');

const app = express();
const port = process.env.PORT || 3000;

app.use(cors());
app.use(bodyParser.json());

// In-memory fake data
let leads = [
  {
    id: 1,
    clientName: 'Acme Corp',
    email: 'contato@acme.com',
    phone: '+55 11 99999-0001',
    company: 'Acme Corp',
    message: 'Interessado em pacote empresarial',
    servico: 'Desenvolvimento Web',
    data: '2025-11-01',
  },
  {
    id: 2,
    clientName: 'Beta Ltda',
    email: 'vendas@beta.com',
    phone: '+55 21 98888-0002',
    company: 'Beta Ltda',
    message: 'Solicitou proposta para design',
    servico: 'Design UX/UI',
    data: '2025-10-28',
  },
  {
    id: 3,
    clientName: 'Gamma S.A.',
    email: 'hello@gamma.com',
    phone: '+55 31 97777-0003',
    company: 'Gamma S.A.',
    message: 'Consulta sobre manutenção',
    servico: 'Suporte e manutenção',
    data: '2025-10-20',
  }
];

// List all leads
app.get('/leads', (req, res) => {
  res.json(leads);
});

// Get lead by id
app.get('/leads/:id', (req, res) => {
  const id = Number(req.params.id);
  const lead = leads.find(l => l.id === id);
  if (!lead) return res.status(404).json({ message: 'Lead not found' });
  res.json(lead);
});

// Create a new lead (optional)
app.post('/leads', (req, res) => {
  const body = req.body;
  const nextId = leads.length ? Math.max(...leads.map(l => l.id)) + 1 : 1;
  const newLead = {
    id: nextId,
    clientName: body.clientName || `Cliente ${nextId}`,
    email: body.email || '',
    phone: body.phone || '',
    company: body.company || '',
    message: body.message || '',
    servico: body.servico || '',
    data: body.data || new Date().toISOString().slice(0,10),
  };
  leads.push(newLead);
  res.status(201).json(newLead);
});

// Update lead (optional)
app.put('/leads/:id', (req, res) => {
  const id = Number(req.params.id);
  const index = leads.findIndex(l => l.id === id);
  if (index === -1) return res.status(404).json({ message: 'Lead not found' });
  const updated = { ...leads[index], ...req.body };
  leads[index] = updated;
  res.json(updated);
});

// Delete lead (optional)
app.delete('/leads/:id', (req, res) => {
  const id = Number(req.params.id);
  const index = leads.findIndex(l => l.id === id);
  if (index === -1) return res.status(404).json({ message: 'Lead not found' });
  leads.splice(index, 1);
  res.status(204).end();
});

app.listen(port, () => {
  console.log(`Fake API server running on http://localhost:${port}`);
});
