import { apiRequest } from '@/api/request';
import type { ActivationCodeSent, CurrentUser } from '@/types/auth';

export type LoginResult = 'success' | 'mustChangePassword';

export interface AuthService {
  login(email: string, password: string): Promise<LoginResult>;
  sendActivationCode(): Promise<ActivationCodeSent>;
  changeInitialPassword(
    currentPassword: string,
    newPassword: string,
    activationCode: string,
  ): Promise<void>;
  logout(): Promise<void>;
}

// Phiên đăng nhập là JWT trong cookie HttpOnly do backend đặt; frontend không
// đọc hay lưu token, trình duyệt tự gửi cookie qua proxy /api cùng origin.
export const authService: AuthService = {
  async login(email, password) {
    const user = await apiRequest<CurrentUser>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
    return user.mustChangePassword ? 'mustChangePassword' : 'success';
  },

  // Gửi OTP kích hoạt tới email của tài khoản đang đăng nhập (lần đầu).
  async sendActivationCode() {
    return apiRequest<ActivationCodeSent>('/auth/activation-code', {
      method: 'POST',
    });
  },

  async changeInitialPassword(currentPassword, newPassword, activationCode) {
    await apiRequest<CurrentUser>('/auth/change-initial-password', {
      method: 'POST',
      body: JSON.stringify({ currentPassword, newPassword, activationCode }),
    });
  },

  async logout() {
    await apiRequest<boolean>('/auth/logout', { method: 'POST' });
  },
};

export function getCurrentUser(): Promise<CurrentUser> {
  return apiRequest<CurrentUser>('/auth/me');
}
