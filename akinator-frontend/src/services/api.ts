import axios from "axios";
import type { GameResponse, UserStats, QuestionTree, Answer } from "../types";

const API_URL = "https://localhost:5001";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  success: boolean;
  token: string | null;
  message: string;
}

const api = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authApi = {
  login: async (data: LoginRequest): Promise<AuthResponse> => {
    try {
      const response = await api.post<AuthResponse>("/auth/login", data);
      if (response.data.token) {
        localStorage.setItem("token", response.data.token);
      }
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response) {
        return error.response.data;
      }
      throw error;
    }
  },

  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    try {
      const response = await api.post<AuthResponse>("/auth/register", data);
      if (response.data.token) {
        localStorage.setItem("token", response.data.token);
      }
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response) {
        return error.response.data;
      }
      throw error;
    }
  },

  logout: () => {
    localStorage.removeItem("token");
  },
};

export const gameApi = {
  startGame: async (): Promise<GameResponse> => {
    const response = await api.post<GameResponse>("/game/start", { userId: "default" });
    return response.data;
  },

  submitAnswer: async (questionId: number, answer: Answer): Promise<GameResponse> => {
    const response = await api.post<GameResponse>("/game/answer", { [questionId]: answer });
    return response.data;
  },

  addCharacter: async (name: string, properties: Record<string, Answer>): Promise<GameResponse> => {
    const response = await api.post<GameResponse>("/game/add-character", { name, properties });
    return response.data;
  },

  addQuestion: async (text: string): Promise<GameResponse> => {
    const response = await api.post<GameResponse>("/game/add-question", { text });
    return response.data;
  },
};

export const analyticsApi = {
  getUserStats: async (): Promise<UserStats> => {
    const response = await api.get<UserStats>("/analytics/user-stats");
    return response.data;
  },

  getQuestionTree: async (): Promise<QuestionTree> => {
    const response = await api.get<QuestionTree>("/analytics/question-tree");
    return response.data;
  },
};
