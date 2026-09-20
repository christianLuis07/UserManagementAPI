# User Management API - TechHive Solutions

## 📖 Deskripsi
API Manajemen Pengguna RESTful yang sederhana untuk TechHive Solutions. Memungkinkan Departemen SDM dan TI untuk melakukan operasi CRUD (Create, Read, Update, Delete) pada data pengguna. 

**Implementasi**: Minimal API dengan .NET 10.0 menggunakan in-memory Dictionary storage.

---

## 🎯 Fitur Utama

### ✅ CRUD Operations
- **GET /api/users** - Retrieve semua users
- **GET /api/users/{id}** - Retrieve user berdasarkan ID  
- **POST /api/users** - Create user baru dengan validasi
- **PUT /api/users/{id}** - Update user dengan validasi
- **DELETE /api/users/{id}** - Delete user
- **GET /api/health** - Health check endpoint

### ✅ Validasi & Error Handling
- ✓ Required field validation (Name, Email)
- ✓ Email uniqueness check
- ✓ Auto-increment ID generation
- ✓ Proper HTTP status codes (200, 201, 204, 400, 404, 409)
- ✓ Descriptive error messages (Bahasa Indonesia)

### ✅ API Documentation
- ✓ OpenAPI/Swagger integration
- ✓ Endpoint descriptions
- ✓ Auto-generated API docs

---

## 🛠️ Tech Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| **.NET** | 10.0 | Framework |
| **C#** | Latest | Language |
| **Minimal APIs** | - | API pattern |
| **Dictionary** | - | In-memory storage |
| **OpenAPI** | 10.0.11 | API documentation |

---

## 📂 Struktur Proyek

```
UserManagementAPI/
├── Program.cs                          # Main entry point dengan endpoints
├── UserManagementAPI.csproj           # Project file
├── UserManagementAPI.http             # API testing file
├── appsettings.json                   # Configuration
├── appsettings.Development.json       # Development config
├── README.md                          # dokumentasi ini
├── COPILOT_DOCUMENTATION.md           # Dokumentasi Copilot assist
├── Properties/
│   └── launchSettings.json            # Launch settings
└── obj/                               # Build artifacts
```

---

## 🚀 Cara Menjalankan

### Prerequisites
- .NET 10.0 SDK atau lebih tinggi
- Windows / Mac / Linux
- (Optional) VS Code REST Client extension untuk testing

### 1. Clone atau buka project
```bash
cd UserManagementAPI
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Run aplikasi
```bash
dotnet run
```

**Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
      Now listening on: http://localhost:5000
```

### 4. Verify API running
```bash
curl https://localhost:5001/api/health
```

---

## 📡 API Endpoints

### 1. Health Check
```http
GET /api/health

Response (200 OK):
{
  "status": "API is healthy",
  "timestamp": "2024-09-20T10:30:00Z"
}
```

### 2. Get All Users
```http
GET /api/users

Response (200 OK):
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@techhive.com",
    "department": "Engineering",
    "position": "Senior Developer",
    "createdAt": "2024-09-20T10:00:00Z",
    "updatedAt": "2024-09-20T10:00:00Z"
  },
  {
    "id": 2,
    "name": "Jane Smith",
    "email": "jane.smith@techhive.com",
    "department": "HR",
    "position": "HR Manager",
    "createdAt": "2024-09-20T10:05:00Z",
    "updatedAt": "2024-09-20T10:05:00Z"
  }
]
```

### 3. Get User by ID
```http
GET /api/users/1

Response (200 OK):
{
  "id": 1,
  "name": "John Doe",
  "email": "john.doe@techhive.com",
  "department": "Engineering",
  "position": "Senior Developer",
  "createdAt": "2024-09-20T10:00:00Z",
  "updatedAt": "2024-09-20T10:00:00Z"
}

# User tidak ditemukan
Response (404 Not Found):
{
  "message": "User dengan ID 999 tidak ditemukan"
}
```

### 4. Create User
```http
POST /api/users
Content-Type: application/json

Request Body:
{
  "name": "Ahmad Nurhadi",
  "email": "ahmad.nurhadi@techhive.com",
  "department": "IT",
  "position": "DevOps Engineer"
}

Response (201 Created):
{
  "id": 3,
  "name": "Ahmad Nurhadi",
  "email": "ahmad.nurhadi@techhive.com",
  "department": "IT",
  "position": "DevOps Engineer",
  "createdAt": "2024-09-20T10:30:00Z",
  "updatedAt": "2024-09-20T10:30:00Z"
}

# Validasi Error
Response (400 Bad Request):
{
  "message": "Field 'Name' dan 'Email' wajib diisi"
}

# Email sudah ada
Response (409 Conflict):
{
  "message": "Email 'john.doe@techhive.com' sudah terdaftar"
}
```

### 5. Update User
```http
PUT /api/users/1
Content-Type: application/json

Request Body:
{
  "name": "John Doe Updated",
  "email": "john.updated@techhive.com",
  "department": "Engineering",
  "position": "Tech Lead"
}

Response (200 OK):
{
  "id": 1,
  "name": "John Doe Updated",
  "email": "john.updated@techhive.com",
  "department": "Engineering",
  "position": "Tech Lead",
  "createdAt": "2024-09-20T10:00:00Z",
  "updatedAt": "2024-09-20T10:30:00Z"
}

# User tidak ditemukan
Response (404 Not Found):
{
  "message": "User dengan ID 999 tidak ditemukan"
}
```

### 6. Delete User
```http
DELETE /api/users/1

Response (204 No Content):
[Empty body]

# User tidak ditemukan
Response (404 Not Found):
{
  "message": "User dengan ID 1 tidak ditemukan"
}
```

---

## 🧪 Testing API

### Option 1: VS Code REST Client
1. Install extension: **REST Client** (Huachao Mao)
2. Open file: `UserManagementAPI.http`
3. Click **Send Request** pada setiap endpoint
4. Lihat response di panel sebelah kanan

### Option 2: Postman
1. [Download Postman](https://www.postman.com/downloads/)
2. Create new collection
3. Add requests untuk setiap endpoint:
   - Base URL: `https://localhost:5001`
   - Headers: `Content-Type: application/json`
4. Test semua operations
5. Verify status codes dan responses

### Option 3: cURL
```bash
# Get all users
curl -X GET https://localhost:5001/api/users \
  -H "Content-Type: application/json"

# Create user
curl -X POST https://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test User",
    "email": "test@techhive.com",
    "department": "IT"
  }'

# Get user by ID
curl -X GET https://localhost:5001/api/users/1 \
  -H "Content-Type: application/json"

# Update user
curl -X PUT https://localhost:5001/api/users/1 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Updated Name",
    "email": "updated@techhive.com"
  }'

# Delete user
curl -X DELETE https://localhost:5001/api/users/1 \
  -H "Content-Type: application/json"
```

---

## 📊 Data Model

### User Properties

| Property | Type | Required | Max Length | Notes |
|----------|------|----------|------------|-------|
| **id** | int | Yes | - | Auto-generated, primary key |
| **name** | string | Yes | - | Full name of the user |
| **email** | string | Yes | - | **Unique**, Email address |
| **department** | string? | No | - | Department name |
| **position** | string? | No | - | Job position/title |
| **createdAt** | DateTime | Yes | - | Auto-set at creation |
| **updatedAt** | DateTime | Yes | - | Auto-updated on changes |

### Sample Data Structure
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john.doe@techhive.com",
  "department": "Engineering",
  "position": "Senior Developer",
  "createdAt": "2024-09-20T10:00:00Z",
  "updatedAt": "2024-09-20T10:00:00Z"
}
```

---

## ✅ HTTP Status Codes

| Code | Meaning | Use Case |
|------|---------|----------|
| **200** | OK | GET successful, PUT successful |
| **201** | Created | POST successful, resource created |
| **204** | No Content | DELETE successful |
| **400** | Bad Request | Invalid input, missing fields |
| **404** | Not Found | Resource doesn't exist |
| **409** | Conflict | Duplicate email |

---

## 🔍 In-Memory Storage Details

### Cara Kerja
```csharp
// Dictionary untuk penyimpanan:
var users = new Dictionary<int, User>
{
    { 1, new User { ... } },
    { 2, new User { ... } }
};

// Auto-increment ID:
var nextUserId = 3;

// Operasi CRUD:
- GET: users.Values.ToList() atau users.TryGetValue()
- POST: users[nextUserId++] = newUser
- PUT: existingUser properties update
- DELETE: users.Remove(id)
```

### Keuntungan
✓ **Simple** - tidak perlu database setup  
✓ **Fast** - in-memory operations  
✓ **Debug-friendly** - mudah di-trace  
✓ **Perfect untuk learning** - fokus pada API design  

### Keterbatasan
⚠️ Data hilang saat aplikasi di-restart  
⚠️ Tidak scalable untuk production  
⚠️ Hanya single-instance  
⚠️ Tidak ada persistence  

---

## 🔄 Data Flow

```
Client Request
    ↓
HTTP Endpoint (MapGet/MapPost/MapPut/MapDelete)
    ↓
Validation (Required fields, Email uniqueness)
    ↓
Business Logic (Dictionary operations)
    ↓
HTTP Response (200/201/204/400/404/409)
    ↓
Client
```

---

## 📝 Contoh Test Scenarios

### Scenario 1: Successful Create User
```
1. POST /api/users dengan valid data
2. Response: 201 Created dengan new user data
3. Status: ✅ PASS
```

### Scenario 2: Duplicate Email Error
```
1. POST /api/users dengan email yang sudah ada
2. Response: 409 Conflict
3. Message: "Email sudah terdaftar"
4. Status: ✅ PASS
```

### Scenario 3: Missing Required Field
```
1. POST /api/users tanpa field "email"
2. Response: 400 Bad Request
3. Message: "Field 'Name' dan 'Email' wajib diisi"
4. Status: ✅ PASS
```

### Scenario 4: Update Non-Existent User
```
1. PUT /api/users/999 dengan valid data
2. Response: 404 Not Found
3. Message: "User dengan ID 999 tidak ditemukan"
4. Status: ✅ PASS
```

---

## 🎓 Learning Notes

### Minimal APIs in .NET 10
- `MapGet()` untuk GET requests
- `MapPost()` untuk POST requests
- `MapPut()` untuk PUT requests
- `MapDelete()` untuk DELETE requests
- `.WithName()` untuk endpoint naming
- `.WithOpenApi()` untuk Swagger documentation
- `.WithDescription()` untuk endpoint description

### LINQ Operations Used
- `.TryGetValue()` - Null-safe dictionary lookup
- `.Any()` - Check if item exists
- `.Values.ToList()` - Get all values as list
- `.Where()` - Filter collection
- `.FirstOrDefault()` - Get first or default item

---

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Change port in launchSettings.json
# atau gunakan environment variable:
dotnet run --urls="https://localhost:5002"
```

### Certificate Error (HTTPS)
```bash
# Trust the development certificate:
dotnet dev-certs https --trust
```

### API Not Responding
```bash
# 1. Check if app is running:
dotnet run

# 2. Test health endpoint:
curl https://localhost:5001/api/health

# 3. Check firewall if accessing from another machine
```

---

## 📚 Documentation Files

- **README.md** - Project overview & API documentation (file ini)
- **COPILOT_DOCUMENTATION.md** - Detailed documentation about Copilot's assistance
- **UserManagementAPI.http** - HTTP requests for testing

---

## 🚀 Next Steps

### Untuk Pengembangan Lebih Lanjut:
1. Implement database (SQL Server, EF Core)
2. Add authentication (JWT, OAuth)
3. Add authorization (Role-based)
4. Add logging (Serilog)
5. Add unit tests (xUnit, NUnit)
6. Add API versioning
7. Add rate limiting
8. Deploy to cloud (Azure, AWS)

### Resources:
- [Microsoft Docs - Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api)
- [ASP.NET Core Best Practices](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
- [REST API Design Guidelines](https://restfulapi.net/)

---

## 📄 License
Internal Project - TechHive Solutions

---

## 👤 Project Info
- **Company**: TechHive Solutions
- **Project**: User Management API
- **Version**: 1.0
- **Status**: ✅ Complete
- **Date**: September 20, 2026


