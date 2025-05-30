import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import {
    AppBar,
    Toolbar,
    Typography,
    Container,
    Button,
    CssBaseline,
    ThemeProvider,
    createTheme,
} from '@mui/material';
import { Game } from './components/Game';
import { Analytics } from './components/Analytics';

const theme = createTheme({
    palette: {
        primary: {
            main: '#1976d2',
        },
        secondary: {
            main: '#dc004e',
        },
    },
});

export const App: React.FC = () => {
  return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <Router>
                <AppBar position="static">
                    <Toolbar>
                        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                            Akinator Game
                        </Typography>
                        <Button color="inherit" component={Link} to="/">
                            Play
                        </Button>
                        <Button color="inherit" component={Link} to="/analytics">
                            Analytics
                        </Button>
                    </Toolbar>
                </AppBar>

                <Container maxWidth="lg" sx={{ mt: 4 }}>
                    <Routes>
                        <Route path="/" element={<Game />} />
                        <Route path="/analytics" element={<Analytics />} />
                    </Routes>
                </Container>
            </Router>
        </ThemeProvider>
    );
};
