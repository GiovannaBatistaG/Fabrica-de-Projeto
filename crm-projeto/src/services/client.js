import api from "./api";

async function getClient() {
    return await api.get("/api/Clientes")
      .then((response) => {
        return response.data;
      })
      .catch((error) => {
        console.error("Erro ao buscar os dados:", error);
        return [];
      });
}

export default getClient;