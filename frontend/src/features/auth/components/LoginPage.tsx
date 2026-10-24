'use client';

import React, { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { AlertCircle, Check, Circle } from 'lucide-react';
import { getNewPasswordRules } from '@/features/auth/passwordRules';
import {
  authService as defaultAuthService,
  type AuthService,
} from '@/features/auth/services/authService';

interface LoginPageProps {
  authService?: AuthService;
  onAuthenticated?: () => void;
}

type Step = 'login' | 'changePassword';

export const LoginPage: React.FC<LoginPageProps> = ({
  authService = defaultAuthService,
  onAuthenticated,
}) => {
  const router = useRouter();
  const [step, setStep] = useState<Step>('login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const finish = () => (onAuthenticated ? onAuthenticated() : router.push('/'));

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email.trim() || !password) {
      setError('Nhập email và mật khẩu để đăng nhập.');
      return;
    }
    try {
      setLoading(true);
      setError(null);
      const result = await authService.login(email.trim(), password);
      if (result === 'mustChangePassword') {
        setNewPassword('');
        setConfirmPassword('');
        setStep('changePassword');
      } else {
        finish();
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Đăng nhập thất bại');
    } finally {
      setLoading(false);
    }
  };

  const rules = getNewPasswordRules(newPassword, confirmPassword, password);
  const rulesPassed = rules.every((r) => r.passed);

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!rulesPassed) return;
    try {
      setLoading(true);
      setError(null);
      await authService.changeInitialPassword(password, newPassword);
      finish();
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Đổi mật khẩu thất bại');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = async () => {
    try {
      await authService.logout();
    } catch {
      // Cookie hết hạn theo thời gian sống của token nếu không gọi được API.
    }
    setStep('login');
    setPassword('');
    setError(null);
  };

  return (
    <div className="login-page">
      <section className="login-brand">
        <div className="login-brand-logo">
          <span>Q</span>
          QLKLTN
        </div>
        <div className="login-brand-copy">
          <h1>
            Khóa luận tốt nghiệp, <em>gọn trong một nơi.</em>
          </h1>
          <p>
            Đăng ký đề tài, nộp đề cương và khóa luận, chấm điểm và nhận kết
            quả.
          </p>
        </div>
        <div className="login-brand-footer">
          <span>Khoa Công nghệ Phần mềm</span>
        </div>
      </section>

      <main className="login-main">
        {step === 'login' ? (
          <form className="login-form" onSubmit={handleLogin} noValidate>
            <div className="login-heading">
              <h2>Đăng nhập</h2>
              <p>Dùng email do trường cấp.</p>
            </div>

            {error && (
              <div id="login-error" role="alert" className="login-error">
                <AlertCircle size={18} />
                <span>{error}</span>
              </div>
            )}

            <div className="login-field">
              <label htmlFor="email">Email</label>
              <input
                id="email"
                type="email"
                autoComplete="username"
                placeholder="ten.ho@truong.edu.vn"
                value={email}
                aria-invalid={!!error}
                aria-describedby={error ? 'login-error' : undefined}
                onChange={(e) => {
                  setEmail(e.target.value);
                  setError(null);
                }}
              />
            </div>

            <div className="login-field">
              <div className="login-field-row">
                <label htmlFor="password">Mật khẩu</label>
                <Link href="/forgot-password">Quên mật khẩu?</Link>
              </div>
              <input
                id="password"
                type="password"
                autoComplete="current-password"
                placeholder="Tối thiểu 8 ký tự, gồm chữ và số"
                value={password}
                aria-invalid={!!error}
                aria-describedby={error ? 'login-error' : undefined}
                onChange={(e) => {
                  setPassword(e.target.value);
                  setError(null);
                }}
              />
            </div>

            <button type="submit" className="login-submit" disabled={loading}>
              {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
            </button>

            <p className="login-note">
              Lần đầu đăng nhập? Mật khẩu ban đầu là mã người dùng của bạn (mã
              sinh viên hoặc mã giảng viên). Hệ thống sẽ yêu cầu bạn đổi mật
              khẩu trước khi vào.
            </p>
          </form>
        ) : (
          <form className="login-form" onSubmit={handleChangePassword}>
            <div className="login-heading">
              <span className="login-badge">Lần đăng nhập đầu tiên</span>
              <h2>Đổi mật khẩu để tiếp tục</h2>
              <p>
                Tài khoản {email} đang dùng mật khẩu ban đầu. Đặt mật khẩu mới
                để vào hệ thống. Bước này không bỏ qua được.
              </p>
            </div>

            {error && (
              <div role="alert" className="login-error">
                <AlertCircle size={18} />
                <span>{error}</span>
              </div>
            )}

            <div className="login-field">
              <label htmlFor="new-password">Mật khẩu mới</label>
              <input
                id="new-password"
                type="password"
                autoComplete="new-password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
              />
            </div>
            <div className="login-field">
              <label htmlFor="confirm-password">Nhập lại mật khẩu mới</label>
              <input
                id="confirm-password"
                type="password"
                autoComplete="new-password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
              />
            </div>

            <ul className="login-rules" aria-label="Quy tắc mật khẩu">
              {rules.map((r) => (
                <li key={r.label} className={r.passed ? 'passed' : undefined}>
                  {r.passed ? <Check size={15} /> : <Circle size={15} />}
                  {r.label}
                </li>
              ))}
            </ul>

            <button
              type="submit"
              className="login-submit"
              disabled={!rulesPassed || loading}
            >
              Đổi mật khẩu và tiếp tục
            </button>

            <span className="login-switch">
              Không phải tài khoản của bạn?{' '}
              <button type="button" onClick={handleLogout}>
                Đăng xuất
              </button>
            </span>
          </form>
        )}
      </main>
    </div>
  );
};
