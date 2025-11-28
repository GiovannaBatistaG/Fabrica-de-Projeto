import "./Forms.css";
import React, { useState, useEffect } from "react";
import api from "../services/api";
import { DataGrid } from "@mui/x-data-grid";
import { Box, IconButton } from "@mui/material";
import { FaRegUserCircle } from "react-icons/fa";
import { FaSearch } from 'react-icons/fa';
import { useNavigate } from "react-router-dom";



const FormsPage = () => {
  const [allRows, setAllRows] = useState([]);
  const [filteredRows, setFilteredRows] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  const navigate = useNavigate();

  // COLUNAS DA TABELA
  const columns = [
    {
      field: "icon",
      headerName: "",
      width: 60,
      sortable: false,
      renderCell: () => <FaRegUserCircle size={22} color="#555" />,
    },
    {
      field: "nome",
      headerName: "Nome do Cliente",
      flex: 1,
      minWidth: 200,
    },
    {
      field: "email",
      headerName: "E-mail",
      flex: 1,
      minWidth: 200,
    },
    {
      field: "telefone",
      headerName: "Telefone",
      flex: 1,
      minWidth: 150,
    },
    {
      field: "mensagem", 
      headerName: "Mensagem",
      flex: 2,
      minWidth: 250,
    },
    {
      field: "actions",
      headerName: "Ver",
      width: 100,
      sortable: false,
      renderCell: (params) => (
        <IconButton
          color="primary"
          onClick={() => navigate(`/lead/${params.row.id}`)}
          sx={{ padding: '4px' }}
          title="Visualizar detalhes"
        >
          <FaSearch size={18} color="#1976d2" />
        </IconButton>
      ),
    },
  ];

  useEffect(() => {
    setIsLoading(true);
    api.get(`/api/Clientes`)
      .then((response) => {
        const dataWithIds = response.data.map((row, index) => ({
          ...row,
          id: row.id || index + 1,
          telefone: row.telefone || "N/A",
          mensagem: row.mensagem || "N/A",
        }));
        setAllRows(dataWithIds);
        setFilteredRows(dataWithIds);
        setIsLoading(false);
      })
      .catch((error) => {
        console.error("Erro ao buscar os dados:", error);
        setIsLoading(false);
      });
  }, []);
  // FUNÇÃO DE FILTRO
  const handleSearchClick = () => {
    if (!searchText.trim()) {
      setFilteredRows(allRows);
      return;
    }

    const lowerCaseSearch = searchText.toLowerCase();
    const newFilteredRows = allRows.filter((row) =>
      row.nome.toLowerCase().includes(lowerCaseSearch)
    );
    setFilteredRows(newFilteredRows);
  };

  return (
    <div className="corpo">
      <div className="search-controls">
        <h1>Formulários Recebidos:</h1>

        <div className="search-bar-container">
          <input
            type="text"
            placeholder="Pesquisar leads por nome..."
            className="search-bar"
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter") handleSearchClick();
            }}
          />
        </div>
      </div>

      <Box
        sx={{
          height: 500,
          marginTop: 5,
          position: "relative",
          backgroundColor: "#fff",
          borderRadius: "10px",
          boxShadow: "0 3px 6px rgba(0,0,0,0.1)",
          padding: 2,
        }}
      >
        <DataGrid
          rows={filteredRows}
          columns={columns}
          loading={isLoading}
          pageSizeOptions={[5, 10, 25]}
          initialState={{
            pagination: { paginationModel: { pageSize: 10 } },
          }}
          disableRowSelectionOnClick
          sx={{
            "& .MuiDataGrid-columnHeaders": {
              backgroundColor: "#f5f5f5",
              fontWeight: "bold",
              fontSize: "1rem",
            },
          }}
        />
      </Box>
    </div>
  );
};

export default FormsPage;
