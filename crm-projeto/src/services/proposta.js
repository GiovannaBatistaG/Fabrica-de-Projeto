import api from "./api";

async function getProposta() {
    return await api.get("/api/Propostas")
      .then((response) => {
        return response.data;
      })
      .catch((error) => {
        console.error("Erro ao buscar os dados:", error);
        return [];
      });
}

export default getProposta;



