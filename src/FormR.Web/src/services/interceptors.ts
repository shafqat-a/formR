import type { AxiosError, AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { handleApiError } from '../utils/error-handler';

/**
 * Setup authentication interceptors for Axios instance
 */
export function setupAuthInterceptors(axiosInstance: AxiosInstance): void {
  // Request interceptor - add JWT token to headers
  axiosInstance.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      const token = getAuthToken();

      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
      }

      return config;
    },
    (error: AxiosError) => {
      return Promise.reject(error);
    }
  );

  // Response interceptor - handle authentication errors
  axiosInstance.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
      if (error.response?.status === 401) {
        // Token expired or invalid - redirect to login
        clearAuthToken();
        window.location.href = '/login';
      }

      return Promise.reject(handleApiError(error));
    }
  );
}

/**
 * Get authentication token from storage
 */
function getAuthToken(): string | null {
  return localStorage.getItem('auth_token');
}

/**
 * Clear authentication token from storage
 */
function clearAuthToken(): void {
  localStorage.removeItem('auth_token');
}

/**
 * Set authentication token in storage
 */
export function setAuthToken(token: string): void {
  localStorage.setItem('auth_token', token);
}

/**
 * Check if user is authenticated
 */
export function isAuthenticated(): boolean {
  return !!getAuthToken();
}
