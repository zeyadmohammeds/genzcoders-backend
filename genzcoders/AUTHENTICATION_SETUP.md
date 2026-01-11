# Authentication System Documentation

This document describes the advanced authentication system implemented with Microsoft Identity, Google OAuth, OTP email verification, and password management.

## Features

### 1. Google OAuth Authentication
- Sign up and sign in with Google
- Automatic account linking if email already exists
- Secure token verification using Google APIs

### 2. Email Registration with OTP
- Two-step registration process:
  1. Send OTP to email
  2. Verify OTP and complete registration
- OTP expires in 10 minutes
- Rate limiting to prevent abuse

### 3. Password Management
- Forgot password (send reset link via email)
- Reset password with secure token
- Change password (authenticated users)
- Password strength validation:
  - Minimum 8 characters
  - At least one uppercase letter
  - At least one lowercase letter
  - At least one digit
  - At least one special character

### 4. Security Features
- Account lockout after 5 failed login attempts (30 minutes)
- Rate limiting on all authentication endpoints
- Secure password hashing using ASP.NET Identity
- JWT token authentication
- Email enumeration protection
- Secure token generation for password reset

## Configuration

### 1. Update appsettings.json

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID_HERE",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET_HERE"
    }
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "YOUR_EMAIL@gmail.com",
    "SmtpPassword": "YOUR_APP_PASSWORD",
    "FromEmail": "YOUR_EMAIL@gmail.com",
    "FromName": "GenzCoders"
  }
}
```

### 2. Google OAuth Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable Google+ API
4. Go to "Credentials" > "Create Credentials" > "OAuth 2.0 Client ID"
5. Configure OAuth consent screen
6. Add authorized redirect URIs:
   - `http://localhost:5166/api/auth/google-signin`
   - `https://yourdomain.com/api/auth/google-signin`
7. Copy Client ID and Client Secret to appsettings.json

### 3. Email Configuration (Gmail Example)

For Gmail, you need to:
1. Enable 2-Factor Authentication
2. Generate an App Password:
   - Go to Google Account Settings
   - Security > 2-Step Verification > App passwords
   - Generate password for "Mail"
   - Use this password in `Email:SmtpPassword`

## API Endpoints

### Authentication Endpoints

#### 1. Send OTP for Registration
```
POST /api/auth/send-otp
Body: { "email": "user@example.com" }
Response: { "message": "OTP sent successfully. Please check your email." }
```

#### 2. Verify OTP and Register
```
POST /api/auth/verify-otp-and-register
Body: {
  "email": "user@example.com",
  "otpCode": "123456",
  "password": "SecurePass123!",
  "nationalId": "123456789",
  "fullNameEn": "John Doe",
  "fullNameAr": "جون دو",
  "phone": "+1234567890"
}
Response: AuthResponseDto with access token
```

#### 3. Google Authentication
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

#### 7. Change Password (Authenticated)
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
Response: { "accountId": 1, "email": "user@example.com", "roleId": 1 }
```

## Database Migration

After implementing the changes, create and apply the database migration:

```bash
cd genzcoders
dotnet ef migrations add AddAuthenticationFeatures
dotnet ef database update
```

## Security Best Practices Implemented

1. **Rate Limiting**: All authentication endpoints have rate limiting to prevent brute force attacks
2. **Account Lockout**: Accounts are locked after 5 failed login attempts for 30 minutes
3. **Password Strength**: Enforced password complexity requirements
4. **Secure Token Generation**: Uses cryptographic random number generation for reset tokens
5. **Token Hashing**: Password reset tokens are hashed before storage
6. **Email Enumeration Protection**: Same response for existing/non-existing emails in password reset
7. **OTP Expiration**: OTPs expire after 10 minutes
8. **OTP Single Use**: OTPs can only be used once
9. **Password Reset Token Expiration**: Reset tokens expire after 1 hour

## Testing

1. Test email registration flow:
   - Send OTP
   - Verify with correct OTP
   - Verify with incorrect OTP (should fail)
   - Verify with expired OTP (should fail)

2. Test Google authentication:
   - Get Google ID token from frontend
   - Call google-auth endpoint
   - Verify token creation

3. Test password reset:
   - Request password reset
   - Check email for reset link
   - Reset password with token
   - Try with expired token (should fail)

4. Test security features:
   - Try multiple failed logins (should lock account)
   - Try rapid OTP requests (should hit rate limit)
   - Try weak passwords (should be rejected)

## Frontend Integration

### Google Sign-In Example (JavaScript)

```javascript
async function signInWithGoogle() {
  // Load Google Identity Services
  google.accounts.id.initialize({
    client_id: 'YOUR_GOOGLE_CLIENT_ID',
    callback: handleCredentialResponse
  });
  
  google.accounts.id.prompt();
}

async function handleCredentialResponse(response) {
  // Send ID token to backend
  const result = await fetch('/api/auth/google-auth', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ idToken: response.credential })
  });
  
  const data = await result.json();
  // Store token and redirect
  localStorage.setItem('accessToken', data.accessToken);
}
```

### Email Registration Flow

```javascript
// Step 1: Send OTP
const sendOtp = async (email) => {
  const response = await fetch('/api/auth/send-otp', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email })
  });
  return response.json();
};

// Step 2: Verify OTP and Register
const verifyOtpAndRegister = async (data) => {
  const response = await fetch('/api/auth/verify-otp-and-register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  return response.json();
};
```

## Notes

- In development mode, if email configuration is missing, OTP codes are logged to the console
- Password reset tokens are single-use and expire after 1 hour
- Google authentication automatically links accounts if email already exists
- All passwords are hashed using ASP.NET Identity PasswordHasher
