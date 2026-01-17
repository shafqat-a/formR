import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { TemplateListPage } from './pages/TemplateListPage';
import { DesignerPage } from './pages/DesignerPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/templates" replace />} />
        <Route path="/templates" element={<TemplateListPage />} />
        <Route path="/designer" element={<DesignerPage />} />
        <Route path="/designer/:id" element={<DesignerPage />} />
        <Route path="*" element={<Navigate to="/templates" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
