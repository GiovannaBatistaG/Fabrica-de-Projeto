import React, { useEffect, useState } from "react";
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
  Typography,
  Divider,
  Alert,
} from "@mui/material";
import getProposta from "../services/proposta";
import api from "../services/api";

const ValidarPage = () => {
  const [propostas, setPropostas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [pdfSelecionado, setPdfSelecionado] = useState(null);
  const [dialogAberto, setDialogAberto] = useState(false);
  const [motivoRejeicao, setMotivoRejeicao] = useState("");
  const [propostaRejeitada, setPropostaRejeitada] = useState(null);
  const [erro, setErro] = useState(null);

  // Buscar propostas geradas que precisam de validação
  useEffect(() => {
    const buscarPropostasParaValidar = async () => {
      setLoading(true);
      setErro(null);
      try {
        const todasPropostas = await getProposta();
        
        // Filtrar apenas propostas que têm PDF gerado e estão aguardando validação
        const propostasParaValidar = todasPropostas.filter(
          (p) => p.pdfUrl && 
          (p.statusValidacao === "Aguardando Validação" || 
           p.statusValidacao === "Pendente") &&
          p.statusValidacao !== "Finalizada" && 
          p.statusValidacao !== "Rejeitada"
        );

        // Transformar para o formato esperado pela UI
        const propostasFormatadas = propostasParaValidar.map((p) => ({
          id: p.id,
          clientName: p.nomeCliente,
          email: p.emailCliente,
          company: p.nomeCliente,
          pdfUrl: p.pdfUrl,
          status: p.statusValidacao?.toLowerCase() || "pendente",
          propostaOriginal: p,
        }));

        setPropostas(propostasFormatadas);
      } catch (err) {
        console.error("Erro ao buscar propostas:", err);
        setErro("Erro ao carregar propostas. Tente novamente.");
      } finally {
        setLoading(false);
      }
    };

    buscarPropostasParaValidar();
  }, []);


  const handleDownload = (pdfUrl, nomeCliente) => {
    const link = document.createElement("a");
    link.href = pdfUrl;
    link.download = `Proposta-${nomeCliente}.pdf`;
    link.click();
  };

  const handleFinalizar = async (id) => {
    const proposta = propostas.find((p) => p.id === id);
    if (!proposta?.propostaOriginal) return;

    try {
      const propostaAtualizada = {
        ...proposta.propostaOriginal,
        statusValidacao: "Finalizada",
      };

      await api.put(`/api/Propostas/${id}`, propostaAtualizada);
      
      setPropostas((prev) =>
        prev.map((p) => (p.id === id ? { ...p, status: "finalizada" } : p))
      );
      
      alert("Proposta finalizada com sucesso!");
    } catch (err) {
      console.error("Erro ao finalizar proposta:", err);
      alert("Erro ao finalizar proposta. Tente novamente.");
    }
  };

  const handleRejeitar = async () => {
    if (!propostaRejeitada) return;
    
    const proposta = propostas.find((p) => p.id === propostaRejeitada);
    if (!proposta?.propostaOriginal) return;

    try {
      const propostaAtualizada = {
        ...proposta.propostaOriginal,
        statusValidacao: "Rejeitada",
        mensagemEquipe: motivoRejeicao,
      };

      await api.put(`/api/Propostas/${propostaRejeitada}`, propostaAtualizada);
    
      setPropostas((prev) =>
        prev.map((p) =>
          p.id === propostaRejeitada
            ? { ...p, status: "rejeitada", motivoRejeicao }
            : p
        )
      );
      
      setDialogAberto(false);
      setMotivoRejeicao("");
      setPropostaRejeitada(null);
      alert("Proposta rejeitada com sucesso!");
    } catch (err) {
      console.error("Erro ao rejeitar proposta:", err);
      alert("Erro ao rejeitar proposta. Tente novamente.");
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={10}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ padding: "30px", maxWidth: "1000px", margin: "auto" }}>
      <Typography variant="h4" color="gray" mb={4} gutterBottom>
        Validação de Propostas
      </Typography>

      {erro && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {erro}
        </Alert>
      )}

      {propostas.length === 0 && !loading ? (
        <Typography textAlign="center" mt={5}>
          Nenhuma proposta gerada aguardando validação.
        </Typography>
      ) : (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
          {propostas.map((prop) => (
            <Box
              key={prop.id}
              sx={{
                backgroundColor: "#fff",
                p: 3,
                borderRadius: 2,
                boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
              }}
            >
              <Typography variant="h6" gutterBottom>
                {prop.clientName}
              </Typography>
              <Typography>
                <strong>Empresa:</strong> {prop.company}
              </Typography>
              <Typography>
                <strong>Email:</strong> {prop.email}
              </Typography>
              <Typography sx={{ mt: 1 }}>
                <strong>Status:</strong>{" "}
                <span
                  style={{
                    color:
                      prop.status === "finalizada"
                        ? "green"
                        : prop.status === "rejeitada"
                        ? "red"
                        : "orange",
                    fontWeight: "bold",
                  }}
                >
                  {prop.status || "pendente"}
                </span>
              </Typography>

              {prop.motivoRejeicao && (
                <Typography sx={{ mt: 1, color: "red" }}>
                  <strong>Motivo da Rejeição:</strong> {prop.motivoRejeicao}
                </Typography>
              )}

              <Divider sx={{ my: 2 }} />

              {prop.pdfUrl && (
                <Box>
                  {pdfSelecionado === prop.id && (
                    <iframe
                      src={prop.pdfUrl}
                      title="Visualização da proposta"
                      width="100%"
                      height="300px"
                      style={{
                        border: "1px solid #ccc",
                        borderRadius: "8px",
                        marginBottom: "10px",
                      }}
                    ></iframe>
                  )}

                  <Box sx={{ display: "flex", gap: 2, flexWrap: "wrap" }}>
                    <Button
                      variant="outlined"
                      onClick={() =>
                        setPdfSelecionado(
                          pdfSelecionado === prop.id ? null : prop.id
                        )
                      }
                    >
                      {pdfSelecionado === prop.id
                        ? "Fechar Visualização"
                        : "Visualizar PDF"}
                    </Button>

                    <Button
                      variant="contained"
                      color="primary"
                      onClick={() =>
                        handleDownload(prop.pdfUrl, prop.clientName)
                      }
                    >
                      Baixar
                    </Button>                    
                    
                    {/* Botões de ação só aparecem se a proposta não estiver finalizada ou rejeitada */}
                    {prop.status !== "finalizada" && prop.status !== "rejeitada" && (
                    <>
                      <Button
                        variant="contained"
                        color="success"
                        onClick={() => handleFinalizar(prop.id)}
                      >
                        Finalizar
                      </Button>
                      <Button
                        variant="contained"
                        color="error"
                        onClick={() => {
                          setPropostaRejeitada(prop.id);
                          setDialogAberto(true);
                        }}
                      >
                        Rejeitar
                      </Button>
                    </>
                    )}
                  </Box>
                </Box>
              )}
            </Box>
          ))}
        </Box>
      )}

      {/* Dialog de rejeição */}
      <Dialog open={dialogAberto} onClose={() => setDialogAberto(false)}>
        <DialogTitle>Motivo da Rejeição</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            multiline
            minRows={3}
            label="Explique o motivo"
            value={motivoRejeicao}
            onChange={(e) => setMotivoRejeicao(e.target.value)}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogAberto(false)}>Cancelar</Button>
          <Button
            variant="contained"
            color="error"
            disabled={!motivoRejeicao.trim()}
            onClick={handleRejeitar}
          >
            Enviar Rejeição
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default ValidarPage;
