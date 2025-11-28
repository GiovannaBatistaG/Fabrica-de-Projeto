import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../services/api";
import {
  Box,
  Typography,
  Button,
  CircularProgress,
  Divider,
} from "@mui/material";

const LeadDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [lead, setLead] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get(`/api/Clientes/${id}`)
      .then((response) => {
        setLead(response.data);
        setLoading(false);
      })
      .catch((error) => {
        console.error("Erro ao carregar o lead:", error);
        setLoading(false);
      });
  }, [id]);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={10}>
        <CircularProgress />
      </Box>
    );
  }

  if (!lead) {
    return <Typography align="center">Lead não encontrado.</Typography>;
  }

  return (
    <Box
      sx={{
        backgroundColor: "#fff",
        p: 5,
        borderRadius: "12px",
        boxShadow: "0 3px 8px rgba(0,0,0,0.1)",
        maxWidth: 650,
        margin: "40px auto",
        fontFamily: "Montserrat, sans-serif",
        color: "#333",
      }}
    >
      <Button
        variant="outlined"
        onClick={() => navigate(-1)}
        sx={{
          mb: 3,
          borderColor: "#673ab7",
          color: "#673ab7",
          "&:hover": { backgroundColor: "#f3e5f5" },
        }}
      >
        Voltar
      </Button>

      <Typography
        variant="h4"
        sx={{ color: "#512da8", fontWeight: 600, mb: 1 }}
      >
        {lead.nome}
      </Typography>

      <Box
        sx={{ width: "40px", height: "3px", backgroundColor: "#ffb300", mb: 3 }}
      />

      <Typography variant="h6" sx={{ color: "#512da8" }}>
        Empresa: {lead.nome}
      </Typography>

      {lead.mensagem && (
        <Typography sx={{ mt: 1 }}>
          <strong>Proposta:</strong> {lead.mensagem}
        </Typography>
      )}

      <Divider sx={{ my: 3 }} />

      <Typography variant="h6" sx={{ color: "#512da8" }}>
        Contato:
      </Typography>

      {lead.telefone && (
        <Typography sx={{ mt: 1 }}>
          <strong>Telefone:</strong> {lead.telefone}
        </Typography>
      )}

      {lead.email && (
        <Typography sx={{ mt: 1 }}>
          <strong>Email:</strong> {lead.email}
        </Typography>
      )}

      <Divider sx={{ my: 3 }} />

      <Typography variant="h6" sx={{ color: "#512da8" }}>
        Solicitação:
      </Typography>
      <Typography sx={{ mt: 1 }}>{lead.mensagem || "—"}</Typography>

      <Divider sx={{ my: 3 }} />

      <Typography
        variant="body2"
        sx={{ mt: 4, color: "#757575", fontSize: "0.9rem" }}
      >
        <strong>Recebido:</strong>{" "}
        {lead.dataCadastro
          ? new Date(lead.dataCadastro).toLocaleDateString("pt-BR")
          : "—"}
        <Button
          variant="contained"
          onClick={() => navigate(`/proposta`, { state: { leadId: lead.id } })}
          sx={{
            mb: 4,
            backgroundColor: "#673ab7",
            color: "#fff",
            "&:hover": { backgroundColor: "#5e35b1" },
            float: "right",
          }}
        >
          Gerar Proposta
        </Button>
      </Typography>
    </Box>
  );
};

export default LeadDetails;
