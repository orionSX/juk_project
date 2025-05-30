import React, { useState } from 'react';
import { Box, Container, Paper, Tab, Tabs, Typography } from '@mui/material';
import LoginForm from './LoginForm';
import RegisterForm from './RegisterForm';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`auth-tabpanel-${index}`}
      aria-labelledby={`auth-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ p: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

const AuthPage: React.FC = () => {
  const [tabValue, setTabValue] = useState(0);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  return (
    <Container component="main" maxWidth="sm">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Paper elevation={3} sx={{ width: '100%', mt: 3 }}>
          <Typography component="h1" variant="h4" align="center" sx={{ pt: 3 }}>
            Welcome to Akinator
          </Typography>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs
              value={tabValue}
              onChange={handleTabChange}
              aria-label="auth tabs"
              centered
            >
              <Tab label="Login" />
              <Tab label="Register" />
            </Tabs>
          </Box>
          <TabPanel value={tabValue} index={0}>
            <LoginForm />
          </TabPanel>
          <TabPanel value={tabValue} index={1}>
            <RegisterForm />
          </TabPanel>
        </Paper>
      </Box>
    </Container>
  );
};

export default AuthPage; 
