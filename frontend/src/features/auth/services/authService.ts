import { isValidPassword } from '@/features/auth/passwordRules';

export type LoginResult = 'success' | 'mustChangePassword';

export interface AuthService {
  login(email: string, password: string): Promise<LoginResult>;
  changeInitialPassword(email: string, newPassword: string): Promise<void>;
}

// Backend chưa có endpoint xác thực (xem README). Bản demo này mô phỏng luồng
// trên giao diện mẫu để dựng màn hình; thay bằng lời gọi apiRequest khi có API.
// - Mật khẩu toàn số (mã người dùng) => lần đăng nhập đầu, bắt đổi mật khẩu.
// - Mật khẩu hợp lệ (>= 8 ký tự, có chữ và số) => đăng nhập thành công.
export const demoAuthService: AuthService = {
  async login(email, password) {
    if (/^[0-9]{6,}$/.test(password)) return 'mustChangePassword';
    if (!email.includes('@') || !isValidPassword(password)) {
      throw new Error(
        'Email hoặc mật khẩu không đúng. Kiểm tra lại hoặc dùng Quên mật khẩu.',
      );
    }
    return 'success';
  },
  async changeInitialPassword() {},
};
