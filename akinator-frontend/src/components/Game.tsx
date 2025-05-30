import React, { useState, useEffect } from 'react';
import { Box, Button, Card, CardContent, Typography, CircularProgress } from '@mui/material';
import type { GameResponse, Answer } from '../types';
import { gameApi } from '../services/api';

export const Game: React.FC = () => {
    const [gameState, setGameState] = useState<GameResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [answers, setAnswers] = useState<Map<number, Answer>>(new Map());

    const startNewGame = async () => {
        try {
            setLoading(true);
            setError(null);
            setAnswers(new Map());
            const response = await gameApi.startGame();
            setGameState(response);
        } catch (err) {
            setError('Failed to start game. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    const handleAnswer = async (answer: Answer) => {
        if (!gameState?.question) return;

        try {
            setLoading(true);
            setError(null);
            
            // Update answers map
            const newAnswers = new Map(answers);
            newAnswers.set(gameState.question.id, answer);
            setAnswers(newAnswers);

            // Submit answer to API
            const response = await gameApi.submitAnswer(gameState.question.id, answer);
            setGameState(response);
        } catch (err) {
            setError('Failed to submit answer. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    const addCharacter = async (name: string, properties: Record<string, Answer>) => {
        try {
            setLoading(true);
            setError(null);
            const response = await gameApi.addCharacter(name, properties);
            setGameState(response);
        } catch (err) {
            setError('Failed to add character. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        startNewGame();
    }, []);

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Box display="flex" flexDirection="column" alignItems="center" gap={2}>
                <Typography color="error">{error}</Typography>
                <Button variant="contained" onClick={startNewGame}>
                    Try Again
                </Button>
            </Box>
        );
    }

    if (!gameState) {
        return null;
    }

    return (
        <Box display="flex" flexDirection="column" alignItems="center" gap={3} p={3}>
            <Card sx={{ maxWidth: 600, width: '100%' }}>
                <CardContent>
                    {gameState.isComplete ? (
                        <>
                            <Typography variant="h5" gutterBottom>
                                Game Complete!
                            </Typography>
                            {gameState.character && (
                                <Typography variant="body1" gutterBottom>
                                    I guessed: {gameState.character.name}
                                </Typography>
                            )}
                            <Typography variant="body2" color="text.secondary">
                                {gameState.message}
                            </Typography>
                            <Box mt={2}>
                                <Button variant="contained" onClick={startNewGame}>
                                    Play Again
                                </Button>
                            </Box>
                        </>
                    ) : (
                        <>
                            <Typography variant="h5" gutterBottom>
                                Question {gameState.question?.id}:
                            </Typography>
                            <Typography variant="body1" gutterBottom>
                                {gameState.question?.text}
                            </Typography>
                            <Box display="flex" gap={2} mt={2}>
                                <Button
                                    variant="contained"
                                    color="primary"
                                    onClick={() => handleAnswer('Yes')}
                                >
                                    Yes
                                </Button>
                                <Button
                                    variant="contained"
                                    color="secondary"
                                    onClick={() => handleAnswer('No')}
                                >
                                    No
                                </Button>
                                <Button
                                    variant="outlined"
                                    onClick={() => handleAnswer('Unknown')}
                                >
                                    Don't Know
                                </Button>
                            </Box>
                        </>
                    )}
                </CardContent>
            </Card>
        </Box>
    );
}; 
 