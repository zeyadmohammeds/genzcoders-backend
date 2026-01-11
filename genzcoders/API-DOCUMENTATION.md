# GenzCoders API Documentation

## Base URLs
- **HTTP**: `http://localhost:5166`
- **HTTPS**: `https://localhost:7237`

## Authentication

All authenticated endpoints require a JWT token in the Authorization header:
```
Authorization: Bearer <token>
```

## Endpoints

### Authentication (`/api/auth`)

#### Register
- **POST** `/api/auth/register`
- **Body**: 
  ```json
  {
    "email": "string",
    "password": "string",
    "nationalId": "string",
    "phone": "string",
    "fullNameEn": "string",
    "fullNameAr": "string"
  }
  ```
- **Response**: `AuthResponseDto` with `accessToken`, `expiresAtUtc`, `accountId`, `email`, `roleId`

#### Login
- **POST** `/api/auth/login`
- **Body**:
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- **Response**: `AuthResponseDto` with `accessToken`, `expiresAtUtc`, `accountId`, `email`, `roleId`

#### Get Current User
- **GET** `/api/auth/me`
- **Headers**: `Authorization: Bearer <token>`
- **Response**: User information

### Applications (`/api/Application`)

#### Ping
- **GET** `/api/Application/ping`
- **Response**: `"API is running"`

#### Get All Applications
- **GET** `/api/Application`
- **Response**: Array of applications

#### Get Application by ID
- **GET** `/api/Application/{id}`
- **Response**: Application object

#### Create Application
- **POST** `/api/Application`
- **Body**: `CreateApplicationDto`
- **Response**: Created application

#### Update Application
- **PUT** `/api/Application/{id}`
- **Body**: `UpdateApplicationDto`
- **Response**: Success

#### Update Application Status
- **PATCH** `/api/Application/{id}`
- **Body**: `PatchApplicationStatusDto`
- **Response**: Updated application

### Courses (`/api/Course`)

#### Get Available Courses
- **GET** `/api/Course/available`
- **Response**: Array of courses

#### Get Course by ID
- **GET** `/api/Course/{id}`
- **Response**: Course object

#### Search Courses
- **GET** `/api/Course/search?name={name}`
- **Response**: Array of matching courses

#### Create Course
- **POST** `/api/Course`
- **Body**: `CreateCourseDto`
- **Response**: Created course

#### Update Course
- **PUT** `/api/Course/{id}`
- **Body**: `UpdateCourseDto`
- **Response**: Updated course

#### Patch Course
- **PATCH** `/api/Course/{id}`
- **Body**: `PatchCourseDto`
- **Response**: No content (204)

#### Delete Course
- **DELETE** `/api/Course/{id}`
- **Response**: No content (204)

### Course Rounds (`/api/CourseRound`)

#### Get All Rounds
- **GET** `/api/CourseRound`
- **Response**: Array of course rounds

#### Get Round by ID
- **GET** `/api/CourseRound/{id}`
- **Response**: Course round object

#### Create Round
- **POST** `/api/CourseRound`
- **Body**: `CreateRoundDto`
- **Response**: `{ "id": number }`

#### Update Round
- **PUT** `/api/CourseRound/{id}`
- **Body**: `UpdateRoundDto`
- **Response**: No content (204)

#### Patch Round
- **PATCH** `/api/CourseRound/{id}`
- **Body**: `PatchRoundDto`
- **Response**: No content (204)

#### Delete Round
- **DELETE** `/api/CourseRound/{id}`
- **Response**: No content (204)

### Health (`/api/Health`)

#### Health Check
- **GET** `/api/Health`
- **Response**: 
  ```json
  {
    "status": "ok",
    "environment": "Development"
  }
  ```

#### Database Health Check
- **GET** `/api/Health/db`
- **Response**: Database connection status

## Frontend Integration Example

### JavaScript/TypeScript (Fetch API)
```javascript
const API_BASE_URL = 'http://localhost:5166';

// Example: Login
async function login(email, password) {
  const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });
  
  if (!response.ok) {
    throw new Error('Login failed');
  }
  
  const data = await response.json();
  // Store token: localStorage.setItem('token', data.accessToken);
  return data;
}

// Example: Authenticated Request
async function getApplications() {
  const token = localStorage.getItem('token');
  const response = await fetch(`${API_BASE_URL}/api/Application`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  });
  
  return response.json();
}
```

### Axios Example
```javascript
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5166',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Login
const login = async (email, password) => {
  const response = await api.post('/api/auth/login', { email, password });
  localStorage.setItem('token', response.data.accessToken);
  return response.data;
};

// Get applications
const getApplications = () => api.get('/api/Application');
```

## CORS Configuration

The backend is configured to accept requests from common frontend development ports:
- `http://localhost:3000` (React default)
- `http://localhost:5173` (Vite default)
- `http://localhost:4200` (Angular default)
- `http://localhost:8080` (Vue default)

To add more origins, update `appsettings.Development.json`:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://your-frontend-url:port"
    ]
  }
}
```

## Error Responses

All error responses follow this format:
```json
{
  "message": "Error description"
}
```

Common status codes:
- `200` - Success
- `201` - Created
- `204` - No Content
- `400` - Bad Request
- `401` - Unauthorized
- `404` - Not Found
- `409` - Conflict
- `500` - Internal Server Error
