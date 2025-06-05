# UsersService – Real Estate Microservice

Este microservicio forma parte del sistema distribuido RealEstate, encargado de gestionar el registro, autenticación y perfil de usuarios. Diseñado con una arquitectura limpia y tecnologías modernas, permite escalar horizontalmente y acoplarse fácilmente a otros servicios mediante JWT.

## Características principales

- Registro de usuarios con validaciones exhaustivas (nombre, correo, contraseña, etc.)
- Login con generación segura de tokens JWT
- Recuperación del perfil autenticado vía token
- Validación de datos usando FluentValidation
- Manejo global de errores con middleware personalizado
- Conexión a base de datos Cassandra optimizada
- Protección de rutas con [Authorize]
- Documentación automática de endpoints vía Swagger

## Tecnologías utilizadas

| Tecnología           | Descripción                                  |
|----------------------|----------------------------------------------|
| .NET 8               | Framework principal del microservicio        |
| ASP.NET Core         | Backend RESTful con controladores            |
| Cassandra (NoSQL)    | Base de datos distribuida para persistencia  |
| FluentValidation     | Validación fuerte y reutilizable de inputs   |
| JWT Authentication   | Seguridad y autorización basada en tokens    |
| Swagger / Swashbuckle| Documentación interactiva de la API          |

## Estructura del proyecto

```
UsersService/
│
├── Controllers/               # Controladores HTTP
│   └── AuthController.cs
│
├── Dtos/                      # Modelos de datos para entrada/salida
│   ├── RegisterRequest.cs
│   ├── LoginRequest.cs
│   └── AuthResponse.cs
│
├── Middlewares/              # Middleware para manejo global de errores
│   └── ExceptionMiddleware.cs
│
├── Models/                   # Entidades de dominio
│   └── User.cs
│
├── Repositories/             # Acceso a Cassandra
│   └── UserRepository.cs
│
├── Services/                 # Lógica de negocio y JWT
│   ├── JwtService.cs
│   └── HashingService.cs
│
├── Validators/               # Validadores con FluentValidation
│   ├── RegisterRequestValidator.cs
│   └── LoginRequestValidator.cs
│
├── Config/                   # Configuraciones externas
│   ├── CassandraOptions.cs
│   └── JwtOptions.cs
│
├── appsettings.Development.json  # Configuración de JWT y Cassandra
├── Program.cs                    # Entry point con inyección de dependencias
└── UsersService.csproj
```

## Configuración del entorno

### Cassandra

En `appsettings.Development.json`:

```json
"Cassandra": {
  "ContactPoints": [ "127.0.0.1" ],
  "Keyspace": "realestate",
  "Username": "cassandra",
  "Password": "cassandra"
}
```

### JWT

```json
"Jwt": {
  "Key": "clave_secreta_segura",
  "Issuer": "RealEstateAPI",
  "Audience": "RealEstateFrontend"
}
```

## Cómo ejecutar

1. Clonar el repositorio:

   git clone https://github.com/PabloToledoBarahona/realestate-users-microservice.git

2. Restaurar dependencias:

   dotnet restore

3. Ejecutar la aplicación:

   dotnet run

4. Acceder a Swagger UI:

   http://localhost:5186/swagger

## Endpoints disponibles

| Método | Ruta               | Autenticación | Descripción                          |
|--------|--------------------|----------------|--------------------------------------|
| POST   | /auth/register     | No             | Registra un nuevo usuario            |
| POST   | /auth/login        | No             | Inicia sesión y genera token JWT     |
| GET    | /auth/profile      | Sí             | Retorna los datos del usuario logueado
