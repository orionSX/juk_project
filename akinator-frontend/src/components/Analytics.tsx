import React, { useState, useEffect } from 'react';
import {
    Box,
    Card,
    CardContent,
    Typography,
    CircularProgress,
    List,
    ListItem,
    ListItemText,
    Divider,
} from '@mui/material';
import { UserStats, QuestionTree } from '../types';
import { analyticsApi } from '../services/api';

export const Analytics: React.FC = () => {
    const [stats, setStats] = useState<UserStats | null>(null);
    const [questionTree, setQuestionTree] = useState<QuestionTree | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                setError(null);
                const [statsData, treeData] = await Promise.all([
                    analyticsApi.getUserStats(),
                    analyticsApi.getQuestionTree(),
                ]);
                setStats(statsData);
                setQuestionTree(treeData);
            } catch (err) {
                setError('Failed to load analytics data. Please try again.');
            } finally {
                setLoading(false);
            }
        };

        fetchData();
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
            <Box display="flex" justifyContent="center" alignItems="center">
                <Typography color="error">{error}</Typography>
            </Box>
        );
    }

    return (
        <Box display="flex" flexDirection="column" gap={3} p={3}>
            <Card>
                <CardContent>
                    <Typography variant="h5" gutterBottom>
                        User Statistics
                    </Typography>
                    {stats && (
                        <Box>
                            <Typography variant="body1">
                                Total Games: {stats.totalGames}
                            </Typography>
                            <Typography variant="body1">
                                Successful Guesses: {stats.successfulGuesses}
                            </Typography>
                            <Typography variant="body1">
                                Average Questions per Game: {stats.averageQuestionsPerGame.toFixed(2)}
                            </Typography>

                            <Typography variant="h6" sx={{ mt: 2 }}>
                                Most Guessed Characters
                            </Typography>
                            <List>
                                {stats.mostGuessedCharacters.map((char, index) => (
                                    <ListItem key={index}>
                                        <ListItemText
                                            primary={char.name}
                                            secondary={`Guessed ${char.count} times`}
                                        />
                                    </ListItem>
                                ))}
                            </List>

                            <Typography variant="h6" sx={{ mt: 2 }}>
                                Most Used Questions
                            </Typography>
                            <List>
                                {stats.mostUsedQuestions.map((q, index) => (
                                    <ListItem key={index}>
                                        <ListItemText
                                            primary={q.text}
                                            secondary={`Used ${q.count} times`}
                                        />
                                    </ListItem>
                                ))}
                            </List>
                        </Box>
                    )}
                </CardContent>
            </Card>

            {questionTree && (
                <Card>
                    <CardContent>
                        <Typography variant="h5" gutterBottom>
                            Question Tree
                        </Typography>
                        <QuestionTreeView tree={questionTree} />
                    </CardContent>
                </Card>
            )}
        </Box>
    );
};

const QuestionTreeView: React.FC<{ tree: QuestionTree; level?: number }> = ({
    tree,
    level = 0,
}) => {
    return (
        <Box sx={{ ml: level * 2 }}>
            <Typography variant="body1">{tree.question.text}</Typography>
            {tree.yesNode && (
                <Box sx={{ mt: 1 }}>
                    <Typography variant="body2" color="primary">
                        Yes:
                    </Typography>
                    <QuestionTreeView tree={tree.yesNode} level={level + 1} />
                </Box>
            )}
            {tree.noNode && (
                <Box sx={{ mt: 1 }}>
                    <Typography variant="body2" color="secondary">
                        No:
                    </Typography>
                    <QuestionTreeView tree={tree.noNode} level={level + 1} />
                </Box>
            )}
        </Box>
    );
}; 
