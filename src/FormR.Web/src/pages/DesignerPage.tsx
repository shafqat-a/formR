import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useDesignerStore } from '../stores/designer-store';
import { templateService } from '../services/template-service';
import { ControlPalette } from '../designer/controls/ControlPalette';
import { DesignerCanvas } from '../designer/canvas/DesignerCanvas';
import { PropertiesPanel } from '../designer/properties-panel/PropertiesPanel';
import type { CreateTemplateDto, UpdateTemplateDto } from '../types/template';

export function DesignerPage() {
  const { id } = useParams<{ id?: string }>();
  const navigate = useNavigate();
  const [isSaving, setIsSaving] = useState(false);

  const {
    template,
    templateName,
    templateDescription,
    controls,
    isDirty,
    setTemplate,
    setTemplateName,
    setTemplateDescription,
    resetTemplate,
    markClean,
  } = useDesignerStore();

  useEffect(() => {
    if (id) {
      loadTemplate(id);
    } else {
      resetTemplate();
    }
  }, [id]);

  const loadTemplate = async (templateId: string) => {
    try {
      const loadedTemplate = await templateService.getById(templateId);
      setTemplate(loadedTemplate);
    } catch (error) {
      console.error('Failed to load template:', error);
      alert('Failed to load template');
    }
  };

  const handleSave = async () => {
    if (!templateName.trim()) {
      alert('Please enter a template name');
      return;
    }

    try {
      setIsSaving(true);

      if (template && id) {
        // Update existing template
        const updateDto: UpdateTemplateDto = {
          name: templateName,
          description: templateDescription || undefined,
          version: template.version,
          controls: controls.map((c, index) => ({
            id: c.id,
            type: c.type,
            label: c.label,
            placeholder: c.placeholder,
            defaultValue: c.defaultValue,
            isRequired: c.isRequired,
            validationRules: c.validationRules,
            position: c.position,
            properties: c.properties,
            parentControlId: c.parentControlId,
            order: index,
          })),
        };

        const updated = await templateService.update(id, updateDto);
        setTemplate(updated);
        markClean();
        alert('Template updated successfully!');
      } else {
        // Create new template
        const createDto: CreateTemplateDto = {
          name: templateName,
          description: templateDescription || undefined,
          controls: controls.map((c, index) => ({
            type: c.type,
            label: c.label,
            placeholder: c.placeholder,
            defaultValue: c.defaultValue,
            isRequired: c.isRequired,
            validationRules: c.validationRules,
            position: c.position,
            properties: c.properties,
            parentControlId: c.parentControlId,
            order: index,
          })),
        };

        const created = await templateService.create(createDto);
        setTemplate(created);
        markClean();
        alert('Template created successfully!');
        navigate(`/designer/${created.id}`, { replace: true });
      }
    } catch (error) {
      console.error('Failed to save template:', error);
      alert('Failed to save template');
    } finally {
      setIsSaving(false);
    }
  };

  const handleBack = () => {
    if (isDirty) {
      if (confirm('You have unsaved changes. Are you sure you want to leave?')) {
        navigate('/templates');
      }
    } else {
      navigate('/templates');
    }
  };

  return (
    <div className="designer-page">
      <div className="designer-header">
        <div className="header-left">
          <button onClick={handleBack} className="back-button">
            ← Back
          </button>
          <div className="template-info">
            <input
              type="text"
              value={templateName}
              onChange={(e) => setTemplateName(e.target.value)}
              className="template-name-input"
              placeholder="Untitled Template"
            />
            <input
              type="text"
              value={templateDescription}
              onChange={(e) => setTemplateDescription(e.target.value)}
              className="template-description-input"
              placeholder="Add description..."
            />
          </div>
        </div>

        <div className="header-right">
          {isDirty && <span className="unsaved-indicator">● Unsaved changes</span>}
          <button onClick={handleSave} className="save-button" disabled={isSaving}>
            {isSaving ? 'Saving...' : 'Save Template'}
          </button>
        </div>
      </div>

      <div className="designer-content">
        <ControlPalette />
        <DesignerCanvas />
        <PropertiesPanel />
      </div>

      <style>{`
        .designer-page {
          display: flex;
          flex-direction: column;
          height: 100vh;
          overflow: hidden;
        }

        .designer-header {
          display: flex;
          align-items: center;
          justify-content: space-between;
          padding: 12px 20px;
          background: white;
          border-bottom: 2px solid #e5e7eb;
        }

        .header-left {
          display: flex;
          align-items: center;
          gap: 16px;
          flex: 1;
        }

        .back-button {
          padding: 8px 16px;
          background: white;
          border: 1px solid #d1d5db;
          border-radius: 4px;
          cursor: pointer;
          font-size: 14px;
          color: #374151;
          transition: all 0.2s;
        }

        .back-button:hover {
          background: #f3f4f6;
          border-color: #9ca3af;
        }

        .template-info {
          display: flex;
          flex-direction: column;
          gap: 4px;
          flex: 1;
          max-width: 600px;
        }

        .template-name-input {
          padding: 6px 12px;
          border: 1px solid transparent;
          border-radius: 4px;
          font-size: 18px;
          font-weight: 600;
          color: #111827;
          outline: none;
          transition: all 0.2s;
        }

        .template-name-input:hover,
        .template-name-input:focus {
          background: #f9fafb;
          border-color: #d1d5db;
        }

        .template-description-input {
          padding: 4px 12px;
          border: 1px solid transparent;
          border-radius: 4px;
          font-size: 13px;
          color: #6b7280;
          outline: none;
          transition: all 0.2s;
        }

        .template-description-input:hover,
        .template-description-input:focus {
          background: #f9fafb;
          border-color: #d1d5db;
        }

        .header-right {
          display: flex;
          align-items: center;
          gap: 12px;
        }

        .unsaved-indicator {
          font-size: 13px;
          color: #f59e0b;
        }

        .save-button {
          padding: 10px 24px;
          background: #3b82f6;
          color: white;
          border: none;
          border-radius: 4px;
          cursor: pointer;
          font-size: 14px;
          font-weight: 500;
          transition: background 0.2s;
        }

        .save-button:hover:not(:disabled) {
          background: #2563eb;
        }

        .save-button:disabled {
          opacity: 0.5;
          cursor: not-allowed;
        }

        .designer-content {
          display: flex;
          flex: 1;
          overflow: hidden;
        }
      `}</style>
    </div>
  );
}
