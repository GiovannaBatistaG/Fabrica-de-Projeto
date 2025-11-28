import React, { useState, useEffect } from "react";
import {
  Box,
  Card,
  CardContent,
  Typography,
  Chip,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  CircularProgress,
  Alert,
  Divider,
} from "@mui/material";
import { FaRegUserCircle } from "react-icons/fa";
import { MdExpandMore, MdCheckCircle, MdPending, MdSchedule } from "react-icons/md";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "./Forms.css";

const ClientesPage = () => {
  const navigate = useNavigate();
  const [clientesComHistorico, setClientesComHistorico] = useState([]);
  const [filteredClientes, setFilteredClientes] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [erro, setErro] = useState(null);

  useEffect(() => {
    const buscarDados = async () => {
    setIsLoading(true);
      setErro(null);
      try {
        const clientesResponse = await api.get("/api/Clientes");
        const clientes = clientesResponse.data || [];

        // Buscar todas as propostas
        const propostasResponse = await api.get("/api/Propostas");
        const todasPropostas = propostasResponse.data || [];

        // Agrupar propostas por cliente
        const propostasPorCliente = {};
        todasPropostas.forEach((proposta) => {
          const clienteId = proposta.clienteId || proposta.ClienteId || 0;
          if (!propostasPorCliente[clienteId]) {
            propostasPorCliente[clienteId] = [];
          }
          propostasPorCliente[clienteId].push(proposta);
        });

        // Ordenar 
        Object.keys(propostasPorCliente).forEach((clienteId) => {
          propostasPorCliente[clienteId].sort((a, b) => {
            const dataA = new Date(a.dataCriacao || a.DataCriacao || 0);
            const dataB = new Date(b.dataCriacao || b.DataCriacao || 0);
            return dataB - dataA;
          });
        });

        // Criar estrutura com histórico
        const clientesCompleto = clientes.map((cliente) => {
          const propostas = propostasPorCliente[cliente.id] || [];
          const propostaMaisRecente = propostas[0] || null;

          return {
            id: cliente.id,
            nome: cliente.nome || "",
            email: cliente.email || "",
            telefone: cliente.phone || "",
            empresa: cliente.nome || "",
            dataEnvioFormulario: cliente.dataCadastro || null,
            propostas: propostas,
            propostaMaisRecente: propostaMaisRecente,
            statusAtual: propostaMaisRecente
              ? propostaMaisRecente.statusValidacao
              : "Sem proposta",
          };
        });

        setClientesComHistorico(clientesCompleto);
        setFilteredClientes(clientesCompleto);
      } catch (err) {
        console.error("Erro ao buscar dados:", err);
        setErro("Erro ao carregar dados. Tente novamente.");
      } finally {
    setIsLoading(false);
      }
    };

    buscarDados();
  }, []);

  const handleSearchClick = () => {
    if (!searchText.trim()) {
      setFilteredClientes(clientesComHistorico);
      return;
    }
    const lowerCaseSearch = searchText.toLowerCase();
    const newFiltered = clientesComHistorico.filter(
      (c) =>
        c.nome.toLowerCase().includes(lowerCaseSearch) ||
        c.email.toLowerCase().includes(lowerCaseSearch) ||
        c.empresa.toLowerCase().includes(lowerCaseSearch)
    );
    setFilteredClientes(newFiltered);
  };

  // Formatar data
  const formatarData = (data) => {
    if (!data) return "Não informado";
    try {
      const date = new Date(data);
      return date.toLocaleDateString("pt-BR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      });
    } catch {
      return "Data inválida";
    }
  };


  const getStatusColor = (status) => {
    const statusLower = status?.toLowerCase() || "";
    if (statusLower.includes("finalizada") || statusLower.includes("concluída")) {
      return "success";
    }
    if (statusLower.includes("rejeitada")) {
      return "error";
    }
    if (statusLower.includes("validar") || statusLower.includes("aguardando")) {
      return "warning";
    }
    return "default";
  };

  if (isLoading) {
    return (
      <div className="corpo">
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
          <CircularProgress />
        </Box>
      </div>
    );
  }

  return (
    <div className="corpo">
      <div className="search-controls">
        <h1>Clientes e Histórico de Propostas</h1>

        <div className="search-bar-container">
          <input
            type="text"
            placeholder="Pesquisar por nome, email ou empresa..."
            className="search-bar"
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter") handleSearchClick();
            }}
          />
        </div>
      </div>

      {erro && (
        <Alert severity="error" sx={{ mt: 2, mb: 2 }}>
          {erro}
        </Alert>
      )}

      <Box sx={{ mt: 3, display: "flex", flexDirection: "column", gap: 2 }}>
        {filteredClientes.length === 0 ? (
          <Typography textAlign="center" mt={5}>
            Nenhum cliente encontrado.
          </Typography>
        ) : (
          filteredClientes.map((cliente) => (
            <Card key={cliente.id} sx={{ boxShadow: 3 }}>
              <CardContent>
                <Box sx={{ display: "flex", alignItems: "center", gap: 2, mb: 2 }}>
                  <FaRegUserCircle size={32} color="#555" />
                  <Box sx={{ flex: 1 }}>
                    <Typography variant="h6" component="div">
                      {cliente.nome}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {cliente.email} {cliente.telefone && `• ${cliente.telefone}`}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {cliente.empresa}
                    </Typography>
                  </Box>
                  <Chip
                    label={cliente.statusAtual}
                    color={getStatusColor(cliente.statusAtual)}
                    size="small"
                  />
                </Box>

                <Divider sx={{ my: 2 }} />

                <Accordion>
                  <AccordionSummary expandIcon={<MdExpandMore />}>
                    <Typography variant="subtitle2">
                      Histórico Completo ({cliente.propostas.length} proposta(s))
                    </Typography>
                  </AccordionSummary>
                  <AccordionDetails>
                    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
                      {/* Data de envio do formulário */}
      <Box
        sx={{
                          display: "flex",
                          alignItems: "center",
                          gap: 1,
                          p: 1,
                          bgcolor: "#f5f5f5",
                          borderRadius: 1,
                        }}
                      >
                        <MdSchedule color="#666" />
                        <Typography variant="body2">
                          <strong>Data de Envio do Formulário:</strong>{" "}
                          {formatarData(cliente.dataEnvioFormulario)}
                        </Typography>
                      </Box>

                      {cliente.propostas.length === 0 ? (
                        <Typography variant="body2" color="text.secondary" sx={{ fontStyle: "italic" }}>
                          Nenhuma proposta gerada ainda.
                        </Typography>
                      ) : (
                        cliente.propostas.map((proposta, index) => (
                          <Box
                            key={proposta.id}
          sx={{
                              p: 2,
                              border: "1px solid #e0e0e0",
                              borderRadius: 2,
                              bgcolor: index === 0 ? "#f0f7ff" : "#fff",
                            }}
                          >
                            <Typography variant="subtitle2" gutterBottom>
                              Proposta #{proposta.id} {index === 0 && "(Mais Recente)"}
                            </Typography>

                            <Box sx={{ display: "flex", flexDirection: "column", gap: 1, mt: 1 }}>
                              <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                <MdCheckCircle color="#2196f3" size={18} />
                                <Typography variant="body2">
                                  <strong>📅 Gerada em:</strong>{" "}
                                  {formatarData(proposta.dataCriacao || proposta.DataCriacao)}
                                </Typography>
                              </Box>

                              {proposta.statusValidacao === "Aguardando Validação" && (
                                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                  <MdPending color="#ff9800" size={18} />
                                  <Typography variant="body2">
                                    <strong>⏳ Enviada para Validação em:</strong>{" "}
                                    {formatarData(proposta.dataCriacao || proposta.DataCriacao)}
                                  </Typography>
                                </Box>
                              )}

                              {proposta.statusValidacao === "Finalizada" && (
                                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                  <MdCheckCircle color="#4caf50" size={18} />
                                  <Typography variant="body2">
                                    <strong>✅ Validada e Finalizada em:</strong>{" "}
                                    {formatarData(proposta.dataCriacao || proposta.DataCriacao)}
                                  </Typography>
                                </Box>
                              )}

                              {proposta.statusValidacao === "Rejeitada" && (
                                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                  <MdCheckCircle color="#f44336" size={18} />
                                  <Typography variant="body2">
                                    <strong>❌ Rejeitada em:</strong>{" "}
                                    {formatarData(proposta.dataCriacao || proposta.DataCriacao)}
                                  </Typography>
                                </Box>
                              )}

                              {proposta.mensagemEquipe && (
                                <Box sx={{ display: "flex", alignItems: "center", gap: 1, mt: 1, p: 1, bgcolor: '#fff5f5', borderRadius: 1, border: '1px solid #fcc' }}>
                                  <Typography variant="body2" color="error">
                                    <strong>Motivo da Rejeição:</strong>{" "}
                                    {proposta.mensagemEquipe}
                                  </Typography>
                                </Box>
                              )}

                              <Typography variant="body2">
                                <strong>Status:</strong>{" "}
                                <Chip
                                  label={proposta.statusValidacao}
                                  color={getStatusColor(proposta.statusValidacao)}
                                  size="small"
                                />
                              </Typography>

                              {proposta.valor > 0 && (
                                <Typography variant="body2">
                                  <strong>Valor:</strong> R$ {proposta.valor.toFixed(2)}
                                </Typography>
                              )}

                              {proposta.pdfUrl && (
                                <Typography variant="body2" color="primary">
                                  <strong>PDF:</strong> Disponível
                                </Typography>
                              )}
                            </Box>
                          </Box>
                        ))
                      )}
                    </Box>
                  </AccordionDetails>
                </Accordion>
              </CardContent>
            </Card>
          ))
        )}
      </Box>
    </div>
  );
};

export default ClientesPage;
