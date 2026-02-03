# Sistema de Gestión de Inventarios

Sistema full-stack para gestión de inventarios de una tienda en línea, desarrollado con .NET 6 y Angular 16.

## Características

- **Autenticación**: Registro e inicio de sesión con JWT
- **Roles**: Administrador (acceso completo) y Empleado (solo lectura)
- **Gestión de Productos**: CRUD completo con validaciones
- **Notificaciones**: Alertas automáticas cuando el stock es menor a 5 unidades
- **Reportes**: Generación de PDFs con productos de bajo stock
- **Frontend Responsivo**: Interfaz moderna con búsqueda y filtros

## Tecnologías

### Backend
- .NET 6.0
- ASP.NET Core Web API
- Entity Framework Core 6.0
- PostgreSQL
- JWT Authentication
- iTextSharp (generación de PDFs)
- xUnit + Moq (pruebas unitarias)

### Frontend
- Angular 16
- TypeScript
- RxJS
- Standalone Components

### Infraestructura
- Docker & Docker Compose
- Nginx (servidor web para frontend)

## Requisitos Previos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL 15](https://www.postgresql.org/) (o Docker)
- [Docker](https://www.docker.com/) (opcional, para despliegue containerizado)

## Instalación y Ejecución

### Opción 1: Con Docker (Recomendado)

```bash
# Clonar el repositorio
git clone <repository-url>
cd evaluationTouch

# Iniciar todos los servicios
docker-compose up -d

# Ver logs
docker-compose logs -f
```

Acceder a:
- Frontend: http://localhost:4200
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### Opción 2: Ejecución Local

#### 1. Base de Datos

Crear base de datos PostgreSQL:
```sql
CREATE DATABASE inventario_db;
```

O iniciar solo PostgreSQL con Docker:
```bash
docker run -d --name postgres_inventario \
  -e POSTGRES_DB=inventario_db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:15-alpine
```

#### 2. Backend

```bash
cd backend/InventarioSystem.API

# Restaurar dependencias
dotnet restore

# Aplicar migraciones (primera vez)
dotnet ef database update

# Ejecutar API
dotnet run
```

La API estará disponible en: http://localhost:5000

#### 3. Frontend

```bash
cd frontend/inventario-frontend

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm start
```

El frontend estará disponible en: http://localhost:4200

## Configuración

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=inventario_db;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "SecretKey": "SuperSecretKey123456789012345678901234567890",
    "Issuer": "InventarioSystemAPI",
    "Audience": "InventarioSystemClient",
    "ExpirationMinutes": 60
  }
}
```

### Frontend (environment.ts)

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

## Estructura del Proyecto

```
evaluationTouch/
├── backend/
│   ├── InventarioSystem.API/        # Controladores y configuración
│   ├── InventarioSystem.Core/       # Entidades, DTOs, Servicios
│   ├── InventarioSystem.Infrastructure/  # Repositorios, DbContext
│   └── InventarioSystem.Tests/      # Pruebas unitarias
├── frontend/
│   └── inventario-frontend/         # Aplicación Angular
├── docker-compose.yml
├── ARCHITECTURE.md                  # Documentación de arquitectura
└── README.md
```

## API Endpoints

### Autenticación
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | /api/auth/register | Registrar usuario |
| POST | /api/auth/login | Iniciar sesión |

### Productos
| Método | Endpoint | Descripción | Rol |
|--------|----------|-------------|-----|
| GET | /api/products | Listar productos (con filtros) | Autenticado |
| GET | /api/products/{id} | Obtener producto por ID | Autenticado |
| GET | /api/products/low-stock | Productos con stock bajo | Autenticado |
| GET | /api/products/categories | Listar categorías | Autenticado |
| POST | /api/products | Crear producto | Administrador |
| PUT | /api/products/{id} | Actualizar producto | Administrador |
| DELETE | /api/products/{id} | Eliminar producto | Administrador |

### Reportes
| Método | Endpoint | Descripción | Rol |
|--------|----------|-------------|-----|
| GET | /api/reports/low-stock-pdf | Reporte PDF stock bajo | Administrador |
| GET | /api/reports/inventory-pdf | Reporte PDF general | Administrador |

### Notificaciones
| Método | Endpoint | Descripción | Rol |
|--------|----------|-------------|-----|
| GET | /api/notifications | Listar notificaciones | Administrador |
| GET | /api/notifications/unread-count | Contar no leídas | Administrador |
| PUT | /api/notifications/{id}/mark-read | Marcar como leída | Administrador |
| PUT | /api/notifications/mark-all-read | Marcar todas como leídas | Administrador |

## Usuario de Prueba

Al iniciar la aplicación, se crea un usuario administrador:

- **Email**: admin@sistema.com
- **Contraseña**: Admin123!
- **Rol**: Administrador

## Pruebas

### Ejecutar pruebas unitarias

```bash
cd backend
dotnet test
```

### Cobertura de pruebas

Las pruebas cubren:
- AuthService: Registro, login, generación de tokens
- ProductService: CRUD, notificaciones de stock bajo
- NotificationService: Gestión de notificaciones
- ProductRepository: Operaciones de base de datos

## Funcionalidades Principales

### 1. Autenticación
- Registro de usuarios con validación de datos
- Login con generación de token JWT
- Protección de rutas según rol

### 2. Gestión de Productos
- Listado con búsqueda por nombre
- Filtrado por categoría
- Indicador visual de stock bajo
- CRUD completo (solo administradores)
- Validación de cantidad no negativa

### 3. Notificaciones Automáticas
- Cuando un producto tiene menos de 5 unidades
- Se notifica a todos los administradores
- Marcado como leído individual o masivo

### 4. Reportes PDF
- Reporte de productos con stock bajo
- Reporte general de inventario
- Incluye estadísticas y totales

## Arquitectura

Ver [ARCHITECTURE.md](./ARCHITECTURE.md) para documentación detallada de la arquitectura del sistema.

## Troubleshooting

### Error de conexión a PostgreSQL
```bash
# Verificar que PostgreSQL está corriendo
docker ps | grep postgres

# Ver logs del contenedor
docker logs inventario_db
```

### Error de CORS
Verificar que el frontend está corriendo en http://localhost:4200

### Error de JWT
Verificar que la clave secreta en appsettings.json tiene al menos 32 caracteres

## Licencia

Este proyecto fue desarrollado como caso práctico para Touch Consulting.
