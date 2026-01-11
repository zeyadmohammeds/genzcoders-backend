# API Endpoints Documentation

## Authentication Endpoints

### Public Endpoints (No Authentication Required)

#### 1. Send OTP for Registration (Student Only)
```
POST /api/auth/send-otp
Body: { "email": "student@example.com" }
Response: { "message": "OTP sent successfully. Please check your email." }
```

#### 2. Verify OTP and Register (Student Only)
```
POST /api/auth/verify-otp-and-register
Body: {
  "email": "student@example.com",
  "otpCode": "123456",
  "password": "SecurePass123!",
  "nationalId": "123456789",
  "fullNameEn": "John Doe",
  "fullNameAr": "جون دو",
  "phone": "+1234567890"
}
Response: AuthResponseDto with access token
```

#### 3. Google Authentication (Student Only)
```
POST /api/auth/google-auth
Body: { "idToken": "GOOGLE_ID_TOKEN" }
Response: AuthResponseDto with access token
```

#### 4. Login
```
POST /api/auth/login
Body: {
  "email": "user@example.com",
  "password": "SecurePass123!"
}
Response: AuthResponseDto with access token
```

#### 5. Forgot Password
```
POST /api/auth/forgot-password
Body: { "email": "user@example.com" }
Response: { "message": "If the email exists, a password reset link has been sent." }
```

#### 6. Reset Password
```
POST /api/auth/reset-password
Body: {
  "email": "user@example.com",
  "token": "RESET_TOKEN_FROM_EMAIL",
  "newPassword": "NewSecurePass123!"
}
Response: { "message": "Password has been reset successfully." }
```

### Authenticated Endpoints (Require Bearer Token)

#### 7. Change Password
```
POST /api/auth/change-password
Headers: Authorization: Bearer {token}
Body: {
  "currentPassword": "OldPass123!",
  "newPassword": "NewSecurePass123!"
}
Response: { "message": "Password has been changed successfully." }
```

#### 8. Get Current User
```
GET /api/auth/me
Headers: Authorization: Bearer {token}
Response: { "accountId": 1, "email": "user@example.com", "roleId": 3 }
```

---

## Admin Endpoints (Require Admin Role)

All admin endpoints require `Authorization: Bearer {token}` header with an admin account token.

#### 1. Create Instructor Account
```
POST /api/admin/create-instructor
Headers: Authorization: Bearer {admin_token}
Body: {
  "email": "instructor@example.com",
  "password": "SecurePass123!",
  "nationalId": "123456789",
  "fullNameEn": "Instructor Name",
  "fullNameAr": "اسم المدرب",
  "phone": "+1234567890"
}
Response: {
  "message": "Instructor account created successfully.",
  "id": 5,
  "email": "instructor@example.com"
}
```

#### 2. Create Admin Account
```
POST /api/admin/create-admin
Headers: Authorization: Bearer {admin_token}
Body: {
  "email": "admin2@example.com",
  "password": "SecurePass123!",
  "nationalId": "123456789",
  "fullNameEn": "Admin Name",
  "fullNameAr": "اسم المسؤول",
  "phone": "+1234567890"
}
Response: {
  "message": "Admin account created successfully.",
  "id": 6,
  "email": "admin2@example.com"
}
```

#### 3. Get All Users
```
GET /api/admin/users
Headers: Authorization: Bearer {admin_token}
Response: Array of user objects with simple information:
[
  {
    "id": 1,
    "email": "user@example.com",
    "fullNameEn": "John Doe",
    "fullNameAr": "جون دو",
    "phone": "+1234567890",
    "roleId": 3,
    "roleName": "Student",
    "statusId": 1,
    "statusName": "Active",
    "isActive": true,
    "isEmailVerified": true,
    "createdAt": "2024-01-01",
    "lastLoginAt": "2024-01-15T10:30:00Z",
    "hasPassword": true,
    "hasGoogleAuth": false
  }
]
```

#### 4. Admin Dashboard
```
GET /api/admin/dashboard
Headers: Authorization: Bearer {admin_token}
Response: {
  "summary": {
    "totalUsers": 150,
    "totalStudents": 120,
    "totalInstructors": 20,
    "totalAdmins": 10,
    "activeUsers": 145,
    "totalCourses": 25,
    "totalCourseRounds": 50,
    "totalApplications": 200,
    "pendingApplications": 15,
    "totalEnrollments": 180,
    "recentRegistrations": 30
  },
  "usersByRole": [
    { "role": "Student", "count": 120 },
    { "role": "Instructor", "count": 20 },
    { "role": "Admin", "count": 10 }
  ],
  "recentApplications": [...]
}
```

---

## Instructor Endpoints (Require Instructor Role)

All instructor endpoints require `Authorization: Bearer {token}` header with an instructor account token.

#### 1. Instructor Dashboard
```
GET /api/instructor/dashboard
Headers: Authorization: Bearer {instructor_token}
Response: {
  "summary": {
    "myCourseRounds": 5,
    "totalStudents": 50,
    "activeCourseRounds": 3,
    "totalMaterials": 20,
    "totalZoomMeetings": 15
  },
  "upcomingMeetings": [...],
  "recentCourseRounds": [...],
  "courseRoundsWithStudents": [...]
}
```

---

## Profile Endpoints (Require Authentication)

All profile endpoints require `Authorization: Bearer {token}` header.

#### 1. Get My Profile
```
GET /api/profile
Headers: Authorization: Bearer {token}
Response: UserProfileDto {
  "id": 1,
  "email": "user@example.com",
  "phone": "+1234567890",
  "nationalId": "123456789",
  "fullNameEn": "John Doe",
  "fullNameAr": "جون دو",
  "roleId": 3,
  "roleName": "Student",
  "createdAt": "2024-01-01",
  "isActive": true,
  "isEmailVerified": true,
  "emailVerifiedAt": "2024-01-01T10:00:00Z",
  "lastLoginAt": "2024-01-15T10:30:00Z",
  "hasPassword": true,
  "hasGoogleAuth": false
}
```

#### 2. Update My Profile
```
PUT /api/profile
Headers: Authorization: Bearer {token}
Body: {
  "email": "newemail@example.com",  // Optional
  "fullNameEn": "New Name",          // Optional
  "fullNameAr": "اسم جديد",           // Optional
  "phone": "+9876543210",            // Optional
  "nationalId": "987654321"          // Optional
}
Response: Updated UserProfileDto
```

---

## Response Format

### AuthResponseDto
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAtUtc": "2024-01-15T11:30:00Z",
  "accountId": 1,
  "email": "user@example.com",
  "roleId": 3,
  "roleName": "Student"
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "message": "Invalid email format."
}
```

### 401 Unauthorized
```json
{
  "message": "Invalid credentials."
}
```

### 403 Forbidden
```json
{
  "message": "Only administrators can create instructor accounts."
}
```

### 404 Not Found
```json
{
  "message": "Account not found."
}
```

### 409 Conflict
```json
{
  "message": "Email already exists."
}
```

### 423 Locked
```json
{
  "message": "Account is locked. Try again in 30 minutes."
}
```

### 429 Too Many Requests
```json
{
  "message": "Too many login attempts. Please try again later."
}
```

---

## Security Features

1. **Role-Based Access Control**: Only admins can create instructor/admin accounts
2. **Rate Limiting**: All authentication endpoints have rate limiting
3. **Account Lockout**: Accounts lock after 5 failed login attempts
4. **Password Strength**: Enforced password requirements
5. **Email Verification**: Required for student registration
6. **Security Headers**: X-Content-Type-Options, X-Frame-Options, etc.
7. **JWT Tokens**: Secure token-based authentication
8. **Password Hashing**: ASP.NET Identity PasswordHasher

---

## Notes

- Frontend sign-up is **ONLY** for students
- Admins and Instructors are created by admins via API endpoints
- All passwords must meet strength requirements:
  - Minimum 8 characters
  - At least one uppercase letter
  - At least one lowercase letter
  - At least one digit
  - At least one special character
- OTP codes expire after 10 minutes
- Password reset tokens expire after 1 hour
- Account lockout duration: 30 minutes after 5 failed attempts
