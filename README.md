# 🛡️ Policy Management API

<<<<<<< HEAD
This is a simple ASP.NET Core Web API project for managing **users, policies, and documents** using JSON-based data storage and **JWT authentication**.
=======
A simple ASP.NET Core Web API project by **Thara Charly T** to manage policies, documents, and users with secure JWT authentication and JSON-based storage.
>>>>>>> 42d7708 (📝 Add detailed README for Policy Management API)

## 🚀 Features

- 🔐 User Registration and Login (with **BCrypt hashed passwords**)
- 🔑 JWT Token generation with **permissions and roles**
- 📄 CRUD for Policies and Documents
- ✅ FluentValidation for DTOs
<<<<<<< HEAD
- 📂 JSON-based user storage (`Data/users.json`)
=======
- 📂 JSON-based user storage ()
>>>>>>> 42d7708 (📝 Add detailed README for Policy Management API)
- 🔁 Refresh token support
- 🔐 Role/Permission-based Authorization
- 🧪 Swagger UI for testing

<<<<<<< HEAD
---

## 📦 Project Structure


=======
## 📦 Project Structure

```
>>>>>>> 42d7708 (📝 Add detailed README for Policy Management API)
PolicyManagement/
├── Controllers/
│   ├── AuthController.cs
│   ├── DocumentController.cs
│   └── PolicyController.cs
├── Data/
│   └── Users.json
├── Models/
│   └── User.cs
├── Services/
│   ├── IUserService.cs
│   └── JsonUserService.cs
├── Tools/
│   └── PasswordHasher.cs
<<<<<<< HEAD
├── Validators/
│   └── CreatePolicyDtoValidator.cs
├── Program.cs
└── README.md         


---

## 🧪 API Endpoints

### ✅ Auth
| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/auth/register` | Register new user |
| `POST` | `/api/auth/login` | Login and get JWT token |

### 📄 Policy
| Method | Route | Description |
|--------|-------|-------------|
| `GET`  | `/api/policy` | Get all policies |
| `POST` | `/api/policy` | Add new policy |
| `PUT`  | `/api/policy/{id}` | Update policy |
| `DELETE` | `/api/policy/{id}` | Delete policy |

---

## 🔐 Example JWT Payload

```json
{
  "userId": "xxx",
  "username": "admin",
  "role": "Admin",
  "permission": [
    "Policy.Add",
    "Policy.List",
    "Policy.Edit",
    "Policy.Delete",
    "Document.Add",
    "Document.List"
  ],
  "exp": 1234567890
}
🛠️ Run the Project
dotnet build
dotnet run
Or use:
dotnet watch run

🧾 License

MIT License — free to use for personal and educational purposes.
=======
├── Program.cs
├── appsettings.json
```

## ⚙️ How to Run

```bash
dotnet build
dotnet run
```

Then open: [http://localhost:5138/swagger](http://localhost:5138/swagger)

>>>>>>> 42d7708 (📝 Add detailed README for Policy Management API)
