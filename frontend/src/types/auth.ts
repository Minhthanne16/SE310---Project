export type UserRole = 'Student' | 'Lecturer' | 'AcademicAffairs';

export interface CurrentUser {
  id: string;
  userCode: string;
  fullName: string;
  email: string;
  role: UserRole;
  mustChangePassword: boolean;
}
