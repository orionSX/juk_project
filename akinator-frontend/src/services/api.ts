import axios from 'axios';
import type { GameResponse, UserStats, QuestionTree, Answer } from '../types';

const API_BASE_URL = 'https://localhost:5001';

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export const gameApi = {
    startGame: async (): Promise<GameResponse> => {
        const response = await api.post<GameResponse>('/game/start', { userId: 'default' });
        return response.data;
    },

    submitAnswer: async (questionId: number, answer: Answer): Promise<GameResponse> => {
        const response = await api.post<GameResponse>('/game/answer', { [questionId]: answer });
        return response.data;
    },

    addCharacter: async (name: string, properties: Record<string, Answer>): Promise<GameResponse> => {
        const response = await api.post<GameResponse>('/game/add-character', { name, properties });
        return response.data;
    },

    addQuestion: async (text: string): Promise<GameResponse> => {
        const response = await api.post<GameResponse>('/game/add-question', { text });
        return response.data;
    },
};

export const analyticsApi = {
    getUserStats: async (): Promise<UserStats> => {
        const response = await api.get<UserStats>('/analytics/user-stats');
        return response.data;
    },

    getQuestionTree: async (): Promise<QuestionTree> => {
        const response = await api.get<QuestionTree>('/analytics/question-tree');
        return response.data;
    },
}; 
