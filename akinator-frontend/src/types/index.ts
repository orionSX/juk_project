export type Answer = 'Yes' | 'No' | 'Unknown';

export interface Question {
    id: number;
    text: string;
}

export interface Character {
    id: number;
    name: string;
    properties: Record<string, Answer>;
}

export interface GameResponse {
    question: Question | null;
    character: Character | null;
    isComplete: boolean;
    message: string;
}

export interface UserStats {
    totalGames: number;
    successfulGuesses: number;
    averageQuestionsPerGame: number;
    mostGuessedCharacters: Array<{ name: string; count: number }>;
    mostUsedQuestions: Array<{ text: string; count: number }>;
}

export interface QuestionTree {
    question: Question;
    yesNode: QuestionTree | null;
    noNode: QuestionTree | null;
} 
