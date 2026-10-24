import { cleanup, fireEvent, render, screen } from '@testing-library/react';
import { afterEach, expect, it, vi } from 'vitest';
import type { AuthService } from '@/features/auth/services/authService';
import { LoginPage } from './LoginPage';

vi.mock('next/navigation', () => ({ useRouter: () => ({ push: vi.fn() }) }));

afterEach(cleanup);

const fakeAuth = (result: 'success' | 'mustChangePassword'): AuthService => ({
  login: vi.fn().mockResolvedValue(result),
  sendActivationCode: vi.fn().mockResolvedValue({
    expiresAt: '2026-10-24T08:10:00Z',
    resendAvailableAt: '2000-01-01T00:00:00Z',
  }),
  changeInitialPassword: vi.fn().mockResolvedValue(undefined),
  logout: vi.fn().mockResolvedValue(undefined),
});

it('shows an error when email or password is empty', () => {
  render(<LoginPage authService={fakeAuth('success')} />);
  fireEvent.click(screen.getByRole('button', { name: 'Đăng nhập' }));
  expect(screen.getByRole('alert')).toHaveTextContent(
    'Nhập email và mật khẩu để đăng nhập.',
  );
});

it('asks for a new password on first login and enables submit once rules pass', async () => {
  const auth = fakeAuth('mustChangePassword');
  const onAuthenticated = vi.fn();
  render(<LoginPage authService={auth} onAuthenticated={onAuthenticated} />);

  fireEvent.change(screen.getByLabelText('Email'), {
    target: { value: 'an.nv@truong.edu.vn' },
  });
  fireEvent.change(screen.getByLabelText('Mật khẩu'), {
    target: { value: '22520001' },
  });
  fireEvent.click(screen.getByRole('button', { name: 'Đăng nhập' }));

  const submit = await screen.findByRole('button', {
    name: 'Đổi mật khẩu và tiếp tục',
  });
  expect(submit).toBeDisabled();

  fireEvent.change(screen.getByLabelText('Mật khẩu mới'), {
    target: { value: 'matkhau123' },
  });
  fireEvent.change(screen.getByLabelText('Nhập lại mật khẩu mới'), {
    target: { value: 'matkhau123' },
  });
  // Mật khẩu đã hợp lệ nhưng chưa có mã xác thực 6 số.
  expect(submit).toBeDisabled();
  expect(auth.sendActivationCode).toHaveBeenCalledTimes(1);

  fireEvent.change(screen.getByLabelText('Mã xác thực'), {
    target: { value: '123456' },
  });
  expect(submit).toBeEnabled();

  fireEvent.click(submit);
  await vi.waitFor(() => expect(onAuthenticated).toHaveBeenCalled());
  expect(auth.changeInitialPassword).toHaveBeenCalledWith(
    '22520001',
    'matkhau123',
    '123456',
  );
});

it('resends the activation code on request', async () => {
  const auth = fakeAuth('mustChangePassword');
  render(<LoginPage authService={auth} />);

  fireEvent.change(screen.getByLabelText('Email'), {
    target: { value: 'an.nv@truong.edu.vn' },
  });
  fireEvent.change(screen.getByLabelText('Mật khẩu'), {
    target: { value: '22520001' },
  });
  fireEvent.click(screen.getByRole('button', { name: 'Đăng nhập' }));

  const resend = await screen.findByRole('button', { name: 'Gửi lại mã' });
  await vi.waitFor(() => expect(resend).toBeEnabled());
  fireEvent.click(resend);

  await vi.waitFor(() =>
    expect(auth.sendActivationCode).toHaveBeenCalledTimes(2),
  );
});

it('clears the session when the user logs out from the change-password step', async () => {
  const auth = fakeAuth('mustChangePassword');
  render(<LoginPage authService={auth} />);

  fireEvent.change(screen.getByLabelText('Email'), {
    target: { value: 'an.nv@truong.edu.vn' },
  });
  fireEvent.change(screen.getByLabelText('Mật khẩu'), {
    target: { value: '22520001' },
  });
  fireEvent.click(screen.getByRole('button', { name: 'Đăng nhập' }));
  fireEvent.click(await screen.findByRole('button', { name: 'Đăng xuất' }));

  await screen.findByRole('button', { name: 'Đăng nhập' });
  expect(auth.logout).toHaveBeenCalled();
});
