import axios from "axios";

const AI_API_BASE = import.meta.env.VITE_AI_BASE_URL || "http://localhost:5000";

const aiApi = axios.create({
  baseURL: AI_API_BASE,
  headers: {
    "Content-Type": "application/json",
  },
});

export default aiApi;
