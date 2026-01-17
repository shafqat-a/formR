import { AxiosError } from 'axios';

export interface ApiError {
  statusCode: number;
  message: string;
  details?: string;
}

export interface ValidationError {
  field: string;
  message: string;
}

/**
 * Handle API errors and transform them into a consistent format
 */
export function handleApiError(error: AxiosError): ApiError {
  if (error.response) {
    // Server responded with error status
    const data = error.response.data as any;

    return {
      statusCode: error.response.status,
      message: data?.message || error.message,
      details: data?.details,
    };
  } else if (error.request) {
    // Request was made but no response received
    return {
      statusCode: 0,
      message: 'Network error - unable to reach server',
    };
  } else {
    // Error in request setup
    return {
      statusCode: 0,
      message: error.message,
    };
  }
}

/**
 * Display error message to user
 */
export function displayError(error: ApiError | string): void {
  const message = typeof error === 'string' ? error : error.message;

  // This can be replaced with your toast/notification library
  console.error('API Error:', message);

  // Example: toast.error(message);
}

/**
 * Handle validation errors from API
 */
export function handleValidationErrors(errors: ValidationError[]): Record<string, string> {
  const fieldErrors: Record<string, string> = {};

  errors.forEach((error) => {
    fieldErrors[error.field] = error.message;
  });

  return fieldErrors;
}

/**
 * Check if error is a specific HTTP status code
 */
export function isErrorStatus(error: ApiError, statusCode: number): boolean {
  return error.statusCode === statusCode;
}

/**
 * Extract error message for display
 */
export function getErrorMessage(error: unknown): string {
  if (typeof error === 'string') {
    return error;
  }

  if (error && typeof error === 'object') {
    if ('message' in error) {
      return (error as ApiError).message;
    }
  }

  return 'An unexpected error occurred';
}
