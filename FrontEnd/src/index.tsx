import React from 'react';
import ReactDOM from 'react-dom/client';
import './styles/landing-page.css';
import App from './App.tsx';
import reportWebVitals from './reportWebVitals.tsx';

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
reportWebVitals();
