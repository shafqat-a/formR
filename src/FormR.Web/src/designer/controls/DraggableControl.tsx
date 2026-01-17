import { useDraggable } from '@dnd-kit/core';
import type { ControlLibraryDto } from '../../types/template';

interface DraggableControlProps {
  control: ControlLibraryDto;
}

export function DraggableControl({ control }: DraggableControlProps) {
  const { attributes, listeners, setNodeRef, isDragging } = useDraggable({
    id: `palette-${control.type}`,
    data: {
      type: 'control-palette',
      control,
    },
  });

  const getControlIcon = (iconName: string) => {
    // Map icon names to emoji icons (can be replaced with actual icon library)
    const iconMap: Record<string, string> = {
      'text-input': '📝',
      'textarea': '📄',
      'number': '🔢',
      'email': '📧',
      'phone': '📱',
      'dropdown': '▼',
      'checkbox': '☑️',
      'radio': '🔘',
      'multi-select': '✅',
      'calendar': '📅',
      'clock': '🕐',
      'date-range': '📆',
      'upload': '📎',
      'rich-text': '✏️',
      'star': '⭐',
      'slider': '🎚️',
      'signature': '✍️',
      'section': '📋',
      'columns': '▦',
      'tabs': '🗂️',
      'accordion': '▤',
    };

    return iconMap[iconName] || '📌';
  };

  const formatControlName = (type: string) => {
    // Convert PascalCase to Title Case with spaces
    return type.replace(/([A-Z])/g, ' $1').trim();
  };

  return (
    <div
      ref={setNodeRef}
      {...listeners}
      {...attributes}
      className={`draggable-control ${isDragging ? 'dragging' : ''}`}
    >
      <span className="control-icon">{getControlIcon(control.icon)}</span>
      <span className="control-name">{formatControlName(control.type)}</span>

      <style>{`
        .draggable-control {
          display: flex;
          align-items: center;
          padding: 10px 12px;
          background: white;
          border: 1px solid #e5e7eb;
          border-radius: 4px;
          cursor: grab;
          user-select: none;
          transition: all 0.2s;
        }

        .draggable-control:hover {
          background: #f9fafb;
          border-color: #3b82f6;
          box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        }

        .draggable-control.dragging {
          opacity: 0.5;
          cursor: grabbing;
        }

        .control-icon {
          margin-right: 8px;
          font-size: 16px;
          flex-shrink: 0;
        }

        .control-name {
          font-size: 13px;
          color: #374151;
          flex: 1;
        }
      `}</style>
    </div>
  );
}
