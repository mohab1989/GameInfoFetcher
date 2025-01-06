import { render, screen } from '@testing-library/react';
import App from './App';

test('renders slogan', () => {
  render(<App />);
  const linkElement = screen.getByText(/made easier/i);
  expect(linkElement).toBeInTheDocument();
});