// src/App.jsx

import { Routes, Route } from "react-router-dom";
import Layout from "../elementes/layout.jsx";

// Importe todas as suas páginas
import FormsPage from "./pages/forms.jsx";
import ValidarPage from "./pages/validar.jsx";
import PropostaPage from "./pages/proposta.jsx";
import ClientesPage from "./pages/clientes.jsx";
import DashboardPage from "./pages/dashbord.jsx";
import LeadDetails from "./pages/LeadDetails";

import "./App.css";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {/* ROTA INICIAL: Define qual página aparece ao acessar a raiz '/'  */}
        <Route index element={<FormsPage />} />
        {}
        <Route path="forms" element={<FormsPage />} />
        <Route path="/lead/:id" element={<LeadDetails />} />
        <Route path="Validar" element={<ValidarPage />} />
        <Route path="proposta" element={<PropostaPage />} />
        <Route path="Dashbord" element={<DashboardPage />} />{" "}
        {/* Usando DashboardPage para "Análise" */}
        <Route path="clientes" element={<ClientesPage />} />
        {/* Opcional: Rota para páginas não encontradas */}
        <Route path="*" element={<h1>404 | Página Não Encontrada</h1>} />
      </Route>
    </Routes>
  );
}

export default App;
