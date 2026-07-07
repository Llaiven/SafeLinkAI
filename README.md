<<<<<<< HEAD
# SafeLink AI – MVC v2.0
**Cambios aplicados:** API de Geolocalización · Dashboard de Reportes · Interfaz rediseñada

---

## Nuevas características

### 🗺️ API de Geolocalización (Google Maps)
- Mapa interactivo en Create/Edit de reportes — haz clic para ubicar el incidente
- Botón "Usar mi ubicación" con GPS del navegador
- Geocodificación inversa automática (coordenadas → dirección)
- Vista de detalle con mapa embebido y círculo de impacto

### 📊 Dashboard de Incidentes en Reportes
- Tarjetas de estadísticas: Activos / En Atención / Resueltos
- Gráfico de dona por categoría (Chart.js)
- Panel de reportes recientes
- Tabla con filtros por estado, categoría y búsqueda

### 🎨 Interfaz rediseñada
- Sidebar oscuro con gradiente y navegación mejorada
- Sistema de diseño propio (variables CSS, componentes reutilizables)
- Tarjetas con bordes de color por severidad
- Badges de colores por estado y categoría
- Botones y formularios modernizados
- Responsive para móvil

---

## Configurar la API de Google Maps

### Paso 1 — Obtener API Key
1. Ve a https://console.cloud.google.com
2. Crea un proyecto o selecciona uno existente
3. Activa estas APIs:
   - **Maps JavaScript API**
   - **Geocoding API**
4. Crea una API Key en "Credentials"

### Paso 2 — Agregar la key al proyecto
Abre `appsettings.json` y reemplaza:
```json
"GoogleMaps": {
  "ApiKey": "TU_API_KEY_AQUI"
}
```

> ⚠️ Si no tienes API Key, el mapa no cargará pero el resto de la app funciona normalmente.

---

## Ejecución (después de clonar)

```powershell
# En Package Manager Console de Visual Studio
Add-Migration InitialCreate
Update-Database
```
Luego presiona **F5**.

---

## Credenciales demo
| Correo | Contraseña | Rol |
|--------|-----------|-----|
| admin@safelinkai.com | Admin@1234! | Administrador |
| ciudadano@safelinkai.com | Citizen@1234! | Ciudadano |
=======
# SafeLink AI – Aplicación Web MVC
**Plataforma de Gestión de Emergencias Comunitarias Inteligentes**

---

## Requisitos previos
- Visual Studio 2022 (con carga de trabajo ASP.NET)
- .NET 8 SDK
- SQL Server (LocalDB incluido con VS2022)

---

## Pasos para ejecutar

### 1. Abrir el proyecto
Abre `SafeLinkAI.csproj` en Visual Studio 2022.

### 2. Restaurar paquetes NuGet
```
dotnet restore
```

### 3. Configurar cadena de conexión
En `appsettings.json` ya está configurado para SQL Server LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SafeLinkAI_DB;Trusted_Connection=True"
```
Si usas SQL Server local cambia a:
```json
"DefaultConnection": "Server=localhost;Database=SafeLinkAI_DB;Trusted_Connection=True;TrustServerCertificate=True"
```

### 4. Aplicar migraciones
En la Consola del Administrador de paquetes (Tools → NuGet → Package Manager Console):
```powershell
Add-Migration InitialCreate
Update-Database
```
O desde terminal:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Ejecutar
Presiona `F5` o:
```bash
dotnet run
```

---

## Credenciales de prueba (auto-creadas al iniciar)

| Usuario | Correo | Contraseña | Rol |
|---------|--------|------------|-----|
| Administrador SafeLink | admin@safelinkai.com | Admin@1234! | Administrador |
| Juan Pérez | ciudadano@safelinkai.com | Citizen@1234! | Ciudadano |

---

## Estructura del proyecto

```
SafeLinkAI/
├── Controllers/
│   ├── AccountController.cs   ← Autenticación (Login, Registro, Logout)
│   ├── HomeController.cs      ← Dashboard y Notificaciones
│   ├── ReportsController.cs   ← CRUD de Reportes de Emergencia
│   └── UsersController.cs     ← Gestión de Usuarios (solo Admin)
├── Models/
│   ├── ApplicationUser.cs     ← Usuario con Identity
│   ├── EmergencyReport.cs     ← Reporte de emergencia
│   └── Notification.cs        ← Notificaciones del sistema
├── Data/
│   ├── ApplicationDbContext.cs ← EF Core DbContext
│   └── DbInitializer.cs       ← Seed de datos iniciales
├── Services/
│   └── NotificationService.cs ← Lógica de notificaciones
├── ViewModels/
│   └── ViewModels.cs          ← DTOs de vistas
├── Views/
│   ├── Account/               ← Login, Register, AccessDenied
│   ├── Home/                  ← Dashboard, Notificaciones
│   ├── Reports/               ← Index, Create, Edit, Details
│   └── Users/                 ← Index, Edit, Details
└── Program.cs                 ← Configuración de la aplicación
```

---

## Módulos implementados

| Módulo | Funcionalidad | Estado |
|--------|--------------|--------|
| Autenticación | Login, registro, logout, lockout | ✅ 90% |
| Gestión de Usuarios | CRUD, activar/desactivar, roles | ✅ 75% |
| Reportes de Emergencia | CRUD, filtros, cambio de estado | ✅ 65% |
| Base de Datos | EF Core + SQL Server + Identity | ✅ 85% |
| Seguridad | Roles, autorización, antiforgery | ✅ 60% |

---

## Roles del sistema
- **Administrador**: ve todos los reportes, gestiona usuarios, cambia estados.
- **Ciudadano**: crea y ve solo sus propios reportes, recibe notificaciones.
>>>>>>> 7ee0d1330da358cbb81d04df34c76b99ba1f1fc3
