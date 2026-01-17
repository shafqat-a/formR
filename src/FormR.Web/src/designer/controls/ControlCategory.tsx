import { useState } from 'react';
import type { ControlLibraryDto } from '../../types/template';
import { DraggableControl } from './DraggableControl';

interface ControlCategoryProps {
  category: string;
  controls: ControlLibraryDto[];
}

export function ControlCategory({ category, controls }: ControlCategoryProps) {
  const [isExpanded, setIsExpanded] = useState(true);

  const categoryDisplayNames: Record<string, string> = {
    '0': 'Basic Inputs',
    '1': 'Selection Controls',
    '2': 'Date/Time Controls',
    '3': 'Advanced Controls',
    '4': 'Layout Controls',
    BasicInputs: 'Basic Inputs',
    SelectionControls: 'Selection Controls',
    DateTimeControls: 'Date/Time Controls',
    AdvancedControls: 'Advanced Controls',
    LayoutControls: 'Layout Controls',
  };

  const displayName = categoryDisplayNames[category] || category;

  return (
    <div className="control-category">
      <div className="category-header" onClick={() => setIsExpanded(!isExpanded)}>
        <span className="category-icon">{isExpanded ? '▼' : '►'}</span>
        <span className="category-name">{displayName}</span>
        <span className="category-count">({controls.length})</span>
      </div>

      {isExpanded && (
        <div className="category-controls">
          {controls.map((control) => (
            <DraggableControl key={control.type} control={control} />
          ))}
        </div>
      )}

      <style>{`
        .control-category {
          margin-bottom: 8px;
        }

        .category-header {
          display: flex;
          align-items: center;
          padding: 8px 12px;
          background: #f3f4f6;
          border-radius: 4px;
          cursor: pointer;
          user-select: none;
          transition: background 0.2s;
        }

        .category-header:hover {
          background: #e5e7eb;
        }

        .category-icon {
          margin-right: 8px;
          font-size: 10px;
          color: #6b7280;
        }

        .category-name {
          flex: 1;
          font-weight: 500;
          font-size: 13px;
          color: #374151;
        }

        .category-count {
          font-size: 11px;
          color: #9ca3af;
        }

        .category-controls {
          padding: 8px 4px;
          display: grid;
          grid-template-columns: 1fr;
          gap: 4px;
        }
      `}</style>
    </div>
  );
}
