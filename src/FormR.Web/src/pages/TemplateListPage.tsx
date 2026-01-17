import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { templateService } from '../services/template-service';
import type { TemplateListDto } from '../types/template';

export function TemplateListPage() {
  const navigate = useNavigate();
  const [templates, setTemplates] = useState<TemplateListDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadTemplates();
  }, []);

  const loadTemplates = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await templateService.list();
      setTemplates(data);
    } catch (err) {
      console.error('Failed to load templates:', err);
      setError('Failed to load templates');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateNew = () => {
    navigate('/designer');
  };

  const handleEditTemplate = (id: string) => {
    navigate(`/designer/${id}`);
  };

  const handleDeleteTemplate = async (id: string, name: string) => {
    if (!confirm(`Are you sure you want to delete "${name}"?`)) {
      return;
    }

    try {
      await templateService.delete(id);
      setTemplates(templates.filter((t) => t.id !== id));
    } catch (err) {
      console.error('Failed to delete template:', err);
      alert('Failed to delete template');
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  if (isLoading) {
    return (
      <div className="template-list-page loading">
        <div className="loading-spinner">Loading templates...</div>

        <style>{`
          .template-list-page.loading {
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
          }

          .loading-spinner {
            font-size: 16px;
            color: #6b7280;
          }
        `}</style>
      </div>
    );
  }

  if (error) {
    return (
      <div className="template-list-page error">
        <div className="error-message">{error}</div>
        <button onClick={loadTemplates} className="retry-button">
          Retry
        </button>

        <style>{`
          .template-list-page.error {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
            gap: 16px;
          }

          .error-message {
            font-size: 16px;
            color: #dc2626;
          }

          .retry-button {
            padding: 10px 24px;
            background: #3b82f6;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
          }

          .retry-button:hover {
            background: #2563eb;
          }
        `}</style>
      </div>
    );
  }

  return (
    <div className="template-list-page">
      <div className="page-header">
        <div>
          <h1 className="page-title">Form Templates</h1>
          <p className="page-subtitle">Create and manage form templates</p>
        </div>
        <button onClick={handleCreateNew} className="create-button">
          + New Template
        </button>
      </div>

      <div className="templates-grid">
        {templates.length === 0 ? (
          <div className="empty-state">
            <div className="empty-icon">📋</div>
            <div className="empty-title">No Templates Yet</div>
            <div className="empty-message">
              Get started by creating your first form template
            </div>
            <button onClick={handleCreateNew} className="empty-create-button">
              Create Template
            </button>
          </div>
        ) : (
          templates.map((template) => (
            <div key={template.id} className="template-card">
              <div className="card-header">
                <h3 className="template-name">{template.name}</h3>
                <span className="template-version">v{template.version}</span>
              </div>

              {template.description && (
                <p className="template-description">{template.description}</p>
              )}

              <div className="template-stats">
                <div className="stat">
                  <span className="stat-label">Controls:</span>
                  <span className="stat-value">{template.controlCount}</span>
                </div>
                <div className="stat">
                  <span className="stat-label">Modified:</span>
                  <span className="stat-value">{formatDate(template.modifiedDate)}</span>
                </div>
              </div>

              <div className="card-actions">
                <button
                  onClick={() => handleEditTemplate(template.id)}
                  className="edit-button"
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDeleteTemplate(template.id, template.name)}
                  className="delete-button"
                >
                  Delete
                </button>
              </div>
            </div>
          ))
        )}
      </div>

      <style>{`
        .template-list-page {
          min-height: 100vh;
          background: #f9fafb;
          padding: 40px 20px;
        }

        .page-header {
          max-width: 1200px;
          margin: 0 auto 32px;
          display: flex;
          align-items: flex-start;
          justify-content: space-between;
        }

        .page-title {
          margin: 0 0 8px 0;
          font-size: 32px;
          font-weight: 700;
          color: #111827;
        }

        .page-subtitle {
          margin: 0;
          font-size: 16px;
          color: #6b7280;
        }

        .create-button {
          padding: 12px 24px;
          background: #3b82f6;
          color: white;
          border: none;
          border-radius: 6px;
          cursor: pointer;
          font-size: 15px;
          font-weight: 500;
          transition: background 0.2s;
        }

        .create-button:hover {
          background: #2563eb;
        }

        .templates-grid {
          max-width: 1200px;
          margin: 0 auto;
          display: grid;
          grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
          gap: 24px;
        }

        .empty-state {
          grid-column: 1 / -1;
          padding: 80px 20px;
          text-align: center;
        }

        .empty-icon {
          font-size: 64px;
          margin-bottom: 20px;
          opacity: 0.3;
        }

        .empty-title {
          font-size: 24px;
          font-weight: 600;
          color: #111827;
          margin-bottom: 12px;
        }

        .empty-message {
          font-size: 16px;
          color: #6b7280;
          margin-bottom: 32px;
        }

        .empty-create-button {
          padding: 14px 32px;
          background: #3b82f6;
          color: white;
          border: none;
          border-radius: 6px;
          cursor: pointer;
          font-size: 16px;
          font-weight: 500;
          transition: background 0.2s;
        }

        .empty-create-button:hover {
          background: #2563eb;
        }

        .template-card {
          background: white;
          border: 1px solid #e5e7eb;
          border-radius: 8px;
          padding: 24px;
          transition: all 0.2s;
        }

        .template-card:hover {
          box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
          border-color: #d1d5db;
        }

        .card-header {
          display: flex;
          align-items: flex-start;
          justify-content: space-between;
          margin-bottom: 12px;
        }

        .template-name {
          margin: 0;
          font-size: 18px;
          font-weight: 600;
          color: #111827;
          flex: 1;
        }

        .template-version {
          display: inline-block;
          padding: 3px 8px;
          background: #e0e7ff;
          color: #4338ca;
          border-radius: 4px;
          font-size: 11px;
          font-weight: 600;
          margin-left: 8px;
        }

        .template-description {
          margin: 0 0 16px 0;
          font-size: 14px;
          color: #6b7280;
          line-height: 1.5;
          display: -webkit-box;
          -webkit-line-clamp: 2;
          -webkit-box-orient: vertical;
          overflow: hidden;
        }

        .template-stats {
          display: flex;
          gap: 20px;
          margin-bottom: 20px;
          padding-top: 16px;
          border-top: 1px solid #f3f4f6;
        }

        .stat {
          display: flex;
          align-items: center;
          gap: 6px;
        }

        .stat-label {
          font-size: 12px;
          color: #9ca3af;
        }

        .stat-value {
          font-size: 13px;
          font-weight: 500;
          color: #374151;
        }

        .card-actions {
          display: flex;
          gap: 8px;
        }

        .edit-button,
        .delete-button {
          flex: 1;
          padding: 10px;
          border: 1px solid #d1d5db;
          border-radius: 4px;
          cursor: pointer;
          font-size: 14px;
          font-weight: 500;
          transition: all 0.2s;
        }

        .edit-button {
          background: white;
          color: #3b82f6;
          border-color: #3b82f6;
        }

        .edit-button:hover {
          background: #eff6ff;
        }

        .delete-button {
          background: white;
          color: #dc2626;
          border-color: #fca5a5;
        }

        .delete-button:hover {
          background: #fef2f2;
          border-color: #f87171;
        }
      `}</style>
    </div>
  );
}
