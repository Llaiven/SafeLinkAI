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
