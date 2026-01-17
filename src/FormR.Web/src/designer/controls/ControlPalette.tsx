import { useEffect, useState } from 'react';
import { useDesignerStore } from '../../stores/designer-store';
import { controlLibraryService } from '../../services/control-library-service';
import { ControlCategory } from './ControlCategory';
import type { ControlLibraryDto } from '../../types/template';

export function ControlPalette() {
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');

  const { controlLibrary, setControlLibrary } = useDesignerStore();

  useEffect(() => {
    loadControlLibrary();
  }, []);

  const loadControlLibrary = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const library = await controlLibraryService.getLibrary();
      setControlLibrary(library);
    } catch (err) {
      console.error('Failed to load control library:', err);
      setError('Failed to load control library');
    } finally {
      setIsLoading(false);
    }
  };

  const getGroupedControls = () => {
    let filtered = controlLibrary;

    // Apply search filter
    if (searchQuery) {
      filtered = filtered.filter((control) =>
        control.type.toLowerCase().includes(searchQuery.toLowerCase())
      );
    }

    // Group by category
    const grouped = filtered.reduce((acc, control) => {
      const category = control.category;
      if (!acc[category]) {
        acc[category] = [];
      }
      acc[category].push(control);
      return acc;
    }, {} as Record<string, ControlLibraryDto[]>);

    return grouped;
  };

  if (isLoading) {
    return (
      <div className="control-palette loading">
        <div className="loading-spinner">Loading controls...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="control-palette error">
        <div className="error-message">{error}</div>
        <button onClick={loadControlLibrary} className="retry-button">
          Retry
        </button>
      </div>
    );
  }

  const groupedControls = getGroupedControls();
  const categoryOrder = ['0', '1', '2', '3', '4']; // BasicInputs, Selection, DateTime, Advanced, Layout

  return (
    <div className="control-palette">
      <div className="palette-header">
        <h3 className="palette-title">Controls</h3>
        <p className="palette-subtitle">Drag controls onto the canvas</p>
      </div>

      <div className="search-box">
        <input
          type="text"
          placeholder="Search controls..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="search-input"
        />
      </div>

      <div className="palette-content">
        {categoryOrder.map((category) => {
          const controls = groupedControls[category];
          if (!controls || controls.length === 0) return null;

          return <ControlCategory key={category} category={category} controls={controls} />;
        })}

        {Object.keys(groupedControls).length === 0 && (
          <div className="no-results">No controls found</div>
        )}
      </div>

      <style>{`
        .control-palette {
          width: 280px;
          height: 100%;
          background: white;
          border-right: 1px solid #e5e7eb;
          display: flex;
          flex-direction: column;
          overflow: hidden;
        }

        .control-palette.loading,
        .control-palette.error {
          justify-content: center;
          align-items: center;
        }

        .loading-spinner {
          padding: 20px;
          color: #6b7280;
          text-align: center;
        }

        .error-message {
          padding: 20px;
          color: #dc2626;
          text-align: center;
        }

        .retry-button {
          margin-top: 12px;
          padding: 8px 16px;
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

        .palette-header {
          padding: 20px 16px 16px;
          border-bottom: 1px solid #e5e7eb;
        }

        .palette-title {
          margin: 0 0 4px 0;
          font-size: 18px;
          font-weight: 600;
          color: #111827;
        }

        .palette-subtitle {
          margin: 0;
          font-size: 12px;
          color: #6b7280;
        }

        .search-box {
          padding: 12px 16px;
          border-bottom: 1px solid #e5e7eb;
        }

        .search-input {
          width: 100%;
          padding: 8px 12px;
          border: 1px solid #d1d5db;
          border-radius: 4px;
          font-size: 13px;
          outline: none;
          transition: border-color 0.2s;
        }

        .search-input:focus {
          border-color: #3b82f6;
        }

        .palette-content {
          flex: 1;
          overflow-y: auto;
          padding: 12px 16px;
        }

        .no-results {
          padding: 40px 20px;
          text-align: center;
          color: #9ca3af;
          font-size: 13px;
        }
      `}</style>
    </div>
  );
}
