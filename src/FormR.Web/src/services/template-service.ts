import apiClient from './api-client';
import type {
  TemplateListDto,
  TemplateDetailDto,
  CreateTemplateDto,
  UpdateTemplateDto,
} from '../types/template';

class TemplateService {
  private readonly baseUrl = '/v1/templates';

  /**
   * Get all templates for the current tenant
   */
  async list(): Promise<TemplateListDto[]> {
    const response = await apiClient.get<TemplateListDto[]>(this.baseUrl);
    return response.data;
  }

  /**
   * Get a template by ID
   */
  async getById(id: string): Promise<TemplateDetailDto> {
    const response = await apiClient.get<TemplateDetailDto>(`${this.baseUrl}/${id}`);
    return response.data;
  }

  /**
   * Create a new template
   */
  async create(template: CreateTemplateDto): Promise<TemplateDetailDto> {
    const response = await apiClient.post<TemplateDetailDto>(this.baseUrl, template);
    return response.data;
  }

  /**
   * Update an existing template
   */
  async update(id: string, template: UpdateTemplateDto): Promise<TemplateDetailDto> {
    const response = await apiClient.put<TemplateDetailDto>(`${this.baseUrl}/${id}`, template);
    return response.data;
  }

  /**
   * Delete a template
   */
  async delete(id: string): Promise<void> {
    await apiClient.delete(`${this.baseUrl}/${id}`);
  }
}

export const templateService = new TemplateService();
export default templateService;
