import apiClient from './api-client';
import type { ControlLibraryDto } from '../types/template';

class ControlLibraryService {
  private readonly baseUrl = '/v1/controls';

  /**
   * Get all available control types from the control library
   */
  async getLibrary(): Promise<ControlLibraryDto[]> {
    const response = await apiClient.get<ControlLibraryDto[]>(`${this.baseUrl}/library`);
    return response.data;
  }

  /**
   * Get controls grouped by category
   */
  async getLibraryGrouped(): Promise<Record<string, ControlLibraryDto[]>> {
    const library = await this.getLibrary();

    return library.reduce((acc, control) => {
      const category = control.category;
      if (!acc[category]) {
        acc[category] = [];
      }
      acc[category].push(control);
      return acc;
    }, {} as Record<string, ControlLibraryDto[]>);
  }
}

export const controlLibraryService = new ControlLibraryService();
export default controlLibraryService;
