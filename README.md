# ExtraHours.API 🕑

## Descripción

ExtraHours.API es una aplicación completa para la gestión de horas extra en empresas. El sistema incluye un backend desarrollado en .NET 9 con API REST y un frontend en React. Permite a los empleados solicitar horas extra y a los administradores gestionar estas solicitudes de manera eficiente.

La aplicación cuenta con un sistema de autenticación JWT robusto y manejo de roles (Admin, Employee, Manager), proporcionando diferentes niveles de acceso según el tipo de usuario.

#### Desarrollado por Grupo 4

---

# Proyecto Principal

Este repositorio es una unión de otros proyectos. A continuación, se enumeran los repositorios base que contribuyeron a este trabajo:

## Repositorios Base

### Frontend del Proyecto Base
[Frontend del Proyecto Base](https://stackblitz.com/edit/vitejs-vite-pmnhntdf)

Este repositorio contiene la implementación del frontend utilizando Vite.js. Incluye componentes clave que son reutilizables y modularizados, facilitando el mantenimiento y la escalabilidad del código. La aplicación está diseñada con un enfoque en la usabilidad, asegurando que los usuarios puedan navegar de manera intuitiva y con una interfaz atractiva. 

Además, cuenta con un diseño interactivo y fácil de usar, cumpliendo con las necesidades del proyecto y facilitando una experiencia de usuario dinámica y responsiva. Se han implementado funcionalidades como:

- **Navegación fluida**: Utilizando rutas que permiten una transición rápida entre diferentes vistas.
- **Accesibilidad**: Permite que la aplicación sea accesible para todos los usuarios.

### Backend del Proyecto
[Backend del Proyecto](https://github.com/Vero-nica22/backendgrupo4/tree/main?tab=readme-ov-file)

Este repositorio contiene la implementación del backend, que incluye:

- **APIs**: Definición de las rutas y lógica de negocio.
- **Conexión a la Base de Datos**: Configuración y manejo de la base de datos.
- **Tokens**: Implementación de la autenticación y gestión de tokens.
- **Peticiones HTTP**: Manejo de solicitudes y respuestas entre el cliente y el servidor.
- **Pruebas Unitarias**: Conjunto de pruebas para asegurar la funcionalidad del código.
- **GitHub Actions**: Configuración para integración continua y despliegue.
- **Docker**: Archivos necesarios para la contenedorización de la aplicación.

---

## Tecnologías

### Backend

- **.NET 9 SDK** - Framework principal para la API
- **Entity Framework Core** - ORM para manejo de base de datos
- **JWT (JSON Web Tokens)** - Sistema de autenticación
- **BCrypt** - Encriptación de contraseñas
- **MySQL** - Base de datos relacional
- **Docker** - Contenedorización de la aplicación

### Frontend

- **React** - Framework de JavaScript
- **Node.js** - Entorno de ejecución para JavaScript
- **npm** - Gestor de paquetes

### DevOps

- **GitHub Actions** - CI/CD para automatización de despliegues
- **Docker** - Contenedorización y despliegue

---

## Instalación

### Requisitos previos

Asegúrate de tener instalado:

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js y npm](https://nodejs.org/)
- [MySQL](https://www.mysql.com/)
- [Docker](https://www.docker.com/) (opcional)

### Instalación manual

#### 1. Clona el repositorio

```bash
git clone https://github.com/Vero-nica22/backendgrupo4.git
cd ExtraHours.API
```

#### 2. Configura la base de datos

Asegúrate de que MySQL esté ejecutándose y actualiza la cadena de conexión en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=extrahours;uid=tu_usuario;pwd=tu_contraseña;"
  }
}
```

#### 3. Restaura las dependencias del backend

```bash
dotnet restore
```

#### 4. Instala Entity Framework CLI (si no lo tienes)

```bash
dotnet tool install --global dotnet-ef
```

#### 5. Aplica las migraciones y crea la base de datos

```bash
dotnet ef database update
```

#### 6. Ejecuta el backend

```bash
dotnet run
```

El backend estará disponible en `https://localhost:5023`.

#### 7. Instala y ejecuta el frontend

```bash
cd frontend
npm install
npm run dev
```

El frontend estará disponible en `http://localhost:5173`.

### Instalación con Docker

#### 1. Clona el repositorio

```bash
git clone https://github.com/Vero-nica22/backendgrupo4.git
cd ExtraHours.API
```

#### 2. Construye y ejecuta con Docker

```bash
docker build -t extrahours-api .
docker run -p 5023:5023 extrahours-api
```

---

## Instrucciones de uso

### Usuarios base del sistema

Al iniciar el proyecto, se crean automáticamente estos usuarios para pruebas:

| Rol      | Usuario               | Contraseña  | Permisos                     |
| -------- | --------------------- | ----------- | ---------------------------- |
| Admin    | admin@ejemplo.com     | admin123    | Gestión completa del sistema |
| Employee | empleado1@ejemplo.com | empleado123 | Solicitar horas extra        |

### Acceso al sistema

#### 1. Login

- Accede a `http://localhost:5173/login`
- Ingresa las credenciales según tu rol
- El sistema te redirigirá al dashboard correspondiente

#### 2. Funcionalidades por rol

**Administrador:**

- Ver todas las solicitudes de horas extra
- Aprobar o rechazar solicitudes
- Gestionar usuarios del sistema
- Generar reportes

**Empleado:**

- Crear nuevas solicitudes de horas extra
- Ver el estado de sus solicitudes
- Editar solicitudes pendientes

### Endpoints principales de la API

#### Autenticación

```bash
# Login
POST /api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "admin@ejemplo.com",
  "password": "admin123"
}
```

#### Gestión de usuarios

```bash
# Obtener todos los usuarios (requiere autenticación)
GET /api/users
Authorization: Bearer {token}

# Crear nuevo usuario
POST /api/users
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "nuevo_usuario",
  "email": "usuario@ejemplo.com",
  "password": "contraseña123",
  "role": "Employee"
}
```

#### Solicitudes de horas extra

```bash
# Obtener todas las solicitudes
GET /api/extrahours
Authorization: Bearer {token}

# Crear nueva solicitud
POST /api/extrahours
Authorization: Bearer {token}
Content-Type: application/json

{
  "date": "2024-12-01",
  "hours": 4,
  "description": "Proyecto urgente"
}
```

---

## Posibles errores y soluciones

### Error de conexión a la base de datos

**Error:** `Unable to connect to any of the specified MySQL hosts`

**Solución:**

1. Verifica que MySQL esté ejecutándose
2. Comprueba la cadena de conexión en `appsettings.json`
3. Asegúrate de que la base de datos exista

### Error de migraciones

**Error:** `Unable to create an object of type 'ApplicationDbContext'`

**Solución:**

```bash
dotnet ef database drop
dotnet ef database update
```

### Error de puertos ocupados

**Error:** `Port 5023 is already in use`

**Solución:**

1. Cambia el puerto en `appsettings.json`
2. O termina el proceso que está usando el puerto:

```bash
# En Windows
netstat -ano | findstr :5023
taskkill /PID {PID} /F

# En macOS/Linux
lsof -ti:5023 | xargs kill
```

### Error de dependencias de npm

**Error:** `npm ERR! peer dep missing`

**Solución:**

```bash
cd frontend
rm -rf node_modules
rm package-lock.json
npm install
```

### Error de autenticación JWT

**Error:** `401 Unauthorized`

**Solución:**

1. Verifica que el token JWT sea válido
2. Comprueba que el token se esté enviando en el header Authorization
3. Asegúrate de que la clave JWT en `appsettings.json` sea la correcta

### Error de CORS

**Error:** `Access to XMLHttpRequest blocked by CORS policy`

**Solución:**
Verifica la configuración de CORS en `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
```

---

## CI/CD con GitHub Actions

El proyecto incluye un pipeline de CI/CD automatizado que:

- Ejecuta pruebas unitarias
- Construye la aplicación
- Crea imágenes Docker
- Despliega automáticamente en el entorno de producción

El archivo de configuración se encuentra en `.github/workflows/ci-cd.yml`.

---

## Docker

El proyecto incluye un `Dockerfile` optimizado para:

- Compilación en múltiples etapas
- Imagen final ligera basada en Alpine
- Configuración de variables de entorno
- Exposición del puerto 5023

Para construir la imagen:

```bash
docker build -t extrahours-api .
```

---

## Uso de Docker en desarrollo y producción

### Producción

- Usa el archivo `Dockerfile` para construir una imagen optimizada y lista para desplegar en servidores.
- Comando:

```bash
docker build -t extrahours-api .
docker run -p 5023:5023 extrahours-api
```

### Desarrollo

- Usa `Dockerfile.dev` (en backend y frontend) junto con `docker-compose.yml` para desarrollo local con hot reload.
- Esto te permite ver los cambios en tiempo real sin reconstruir la imagen.
- Comando:

```bash
docker-compose up --build
```

Esto levantará dos servicios:

- Backend (.NET) en http://localhost:5023
- Frontend (React/Vite) en http://localhost:5173

## Recomendación sobre el uso de `docker-compose up` y `--build`

- Usa `docker-compose up` normalmente para levantar los servicios en desarrollo. Esto es suficiente para aprovechar el hot reload y ver cambios en el código fuente.
- Solo necesitas usar `docker-compose build` o `docker-compose up --build` si:
  - Cambiaste el `Dockerfile` o `Dockerfile.dev`.
  - Cambiaste el `package.json` o agregaste/eliminaste dependencias en el frontend.
  - Quieres forzar una reinstalación de dependencias dentro del contenedor.
- Si tienes errores de dependencias nativas en el frontend, elimina `node_modules` y `package-lock.json` en la carpeta `fronted` y luego ejecuta `docker-compose build frontend` antes de volver a levantar los servicios.

Así evitas reconstrucciones innecesarias y aprovechas al máximo el entorno de desarrollo con Docker.

### Resumen de archivos

- `Dockerfile` → Producción (build optimizado, un solo contenedor)
- `Dockerfile.dev` → Desarrollo (hot reload, volúmenes)
- `docker-compose.yml` → Orquestación de backend y frontend en desarrollo

> **Recomendación:** No elimines ninguno de estos archivos. Usa el adecuado según el entorno.

---

## Notas importantes

- Cambia las cadenas de conexión y claves JWT en `appsettings.json` según tu entorno de producción
- Los usuarios base se crean automáticamente solo si no existen en la base de datos
- Las contraseñas se almacenan con hash seguro usando BCrypt
- El sistema está preparado para escalabilidad horizontal
- Se recomienda usar HTTPS en producción

---

## Muchas gracias

¡Gracias por usar ExtraHours.API! Si tienes alguna pregunta o sugerencia, no dudes en contactar al equipo de desarrollo!
