# Configuración de Producción - Sistema de Ferretería

## Variables de Entorno Requeridas

### Backend (.NET API)

Las siguientes variables de entorno deben configurarse en tu plataforma de hosting (Railway, Render, Azure, etc.):

```bash
# Base de Datos MySQL
MYSQL_CONNECTION_STRING=Server=your-mysql-server;Database=sistemapuntoventa;Uid=your-user;Pwd=your-password;Port=3306;

# JWT Configuration
JWT_SECRET_KEY=your-super-secret-jwt-key-at-least-32-characters-long

# Frontend URL (para CORS)
FRONTEND_URL=https://your-frontend.vercel.app
```

### Frontend (React/Vercel)

Las siguientes variables de entorno deben configurarse en Vercel:

```bash
# API URL (se detecta automáticamente en api.ts)
# No requiere configuración manual si usas la lógica actual
```

## Plataformas de Hosting Recomendadas

### Backend Options:

1. **Railway** (Recomendado para empezar)
   - Soporta .NET 8 y MySQL
   - Deployment automático desde GitHub
   - Variables de entorno fáciles de configurar
   - Plan gratuito disponible

2. **Render** (Excelente plan gratuito)
   - Soporta Docker y .NET
   - PostgreSQL incluido (puedes migrar de MySQL)
   - SSL automático
   - Deployment desde GitHub

3. **Azure** (Mejor para .NET)
   - Azure App Service + Azure SQL Database
   - Integración perfecta con .NET
   - Escalabilidad excelente
   - Más costoso pero muy robusto

### Frontend:

- **Vercel** (Ya estás usándolo)
  - Excelente para React
  - Deployment automático
  - Dominios personalizados gratuitos
  - SSL automático

## Pasos para Deployment

### 1. Preparar el Repositorio

```bash
# Asegúrate de que todo esté commiteado
git add .
git commit -m "Ready for production deployment"
git push origin main
```

### 2. Configurar Backend (Ejemplo con Railway)

1. Crea cuenta en [railway.app](https://railway.app)
2. Crea nuevo proyecto → "Deploy from GitHub repo"
3. Conecta tu repositorio
4. Railway detectará automáticamente tu proyecto .NET
5. Configura las variables de entorno
6. Agrega servicio MySQL
7. Deploy!

### 3. Configurar Frontend (Vercel)

1. Ya debería estar configurado
2. Actualiza la URL del backend en producción si es necesario
3. Deploy automático con git push

### 4. Actualizar CORS en Producción

En `appsettings.Production.json`, asegúrate de que `FrontendUrl` apunte a tu URL de Vercel.

## Security Notes para Producción

1. **JWT Secret Key**: Usa una clave fuerte de al menos 32 caracteres
2. **Database Password**: Usa contraseñas fuertes y diferentes de desarrollo
3. **CORS**: Solo permite tu dominio de frontend específico
4. **HTTPS**: Asegúrate de que tu backend use HTTPS en producción
5. **Environment Variables**: Nunca commits secretos en el repositorio

## Monitoreo y Logs

- Railway: Logs integrados en el dashboard
- Render: Logs disponibles en el dashboard
- Azure: Application Insights para monitoreo avanzado

## Backup Strategy

- **Database**: Configura backups automáticos en tu proveedor de base de datos
- **Logs**: Configura retención de logs para auditoría
- **Code**: Tu código está seguro en GitHub

## Troubleshooting Común

### Error 401 Unauthorized
- Verifica que el JWT secret key sea el mismo en backend y frontend
- Revisa la expiración del token

### Error CORS
- Verifica que FrontendUrl sea correcto en appsettings
- Asegúrate de que tu dominio de frontend esté permitido

### Database Connection Issues
- Verifica las credenciales de MySQL
- Asegúrate de que la base de datos sea accesible desde el backend
- Revisa los firewall rules