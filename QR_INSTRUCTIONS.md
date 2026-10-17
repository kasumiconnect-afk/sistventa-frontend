# Instrucciones para Probar la Funcionalidad QR

## Configuración Actual

**Frontend:** http://localhost:5173 (o http://192.168.0.95:5173 desde celular)
**Backend:** http://0.0.0.0:5164 (accesible como http://192.168.0.95:5164 desde celular)
**IP Local:** 192.168.0.95

## ✅ PROBLEMAS SOLUCIONADOS

### Problema 1: Redirección al Login
**Problema:** Al escanear el QR, el sistema redirigía al login en lugar de ir a la página de pagos.

**Solución:** 
- La página de simulación de pagos ahora es completamente pública (no requiere autenticación)
- Los endpoints de pagos en el backend ahora permiten acceso sin autenticación `[AllowAnonymous]`
- Se creó una instancia separada de axios para llamadas públicas sin token JWT
- Se mejoró la lógica de verificación de autenticación en App.tsx

### Problema 2: Error CORS desde Celular
**Problema:** Al acceder desde el celular (192.168.0.95:5173), fallaban las peticiones al backend (localhost:5164).

**Solución:**
- Backend ahora escucha en todas las interfaces de red (0.0.0.0:5164)
- Agregadas IPs locales a la configuración CORS (192.168.0.95:5173, 5174, 5175)
- Frontend detecta automáticamente si accede desde IP local y ajusta la URL del backend
- Health check y endpoints de pagos ahora funcionan desde celular

## Pasos para Probar el Sistema de Pagos QR

### 1. Iniciar los Servicios

El backend y frontend ya están corriendo:
- Backend .NET API: `http://localhost:5164`
- Frontend React: `http://localhost:5173`

### 2. Configurar IP Local (si es necesario)

1. Accede al sistema: http://localhost:5174
2. Inicia sesión con tus credenciales
3. Si necesitas cambiar la IP local para escanear el QR desde tu celular:
   - Ve a la página de simulación de pagos
   - Haz clic en el icono de configuración ⚙️
   - Ingresa tu IP local (ej: 192.168.1.9)
   - Ingresa el puerto del frontend (5174)

### 3. Crear una Venta con Pago QR

1. Navega a la página de **Ventas**
2. Agrega productos al carrito
3. Selecciona **QR** como método de pago
4. Completa la venta

### 4. Escanear el QR

1. Se mostrará un modal con el código QR
2. Escanea el QR con tu celular
3. **Ahora te redirigirá directamente a la página de simulación de pagos** (sin login)
4. El sistema verificará automáticamente el estado del backend

### 5. Simular el Pago

En la página de simulación de pagos:
- **Botón Verde**: Simular pago exitoso
- **Botón Rojo**: Simular pago rechazado
- **Botón Amarillo**: Simular error del sistema

### 6. Verificar el Proceso

1. El sistema mostrará el estado del pago
2. Si el pago es exitoso, la venta se marcará como "PAGADA"
3. El modal de QR se cerrará automáticamente
4. La venta aparecerá en el listado con estado pagado

## Funcionalidades Implementadas

### Frontend (React)
- ✅ Generación de QR con `qrcode.react`
- ✅ Modal de visualización de QR
- ✅ Sistema de polling para verificar estado del pago
- ✅ Página de simulación de pagos **COMPLETAMENTE PÚBLICA**
- ✅ Configuración de IP local y puerto
- ✅ Verificación de estado del backend
- ✅ Axios público para endpoints sin autenticación

### Backend (.NET API)
- ✅ Endpoint para recibir webhooks de pago `[AllowAnonymous]`
- ✅ Endpoint para verificar estado de pago `[AllowAnonymous]`
- ✅ Endpoint de health check `[AllowAnonymous]`
- ✅ Actualización de estado de ventas
- ✅ Manejo de diferentes estados (PAID, REJECTED, ERROR)

## Cambios Recientes (Corrección del problema de login)

### App.tsx
- Cambiado estado inicial de `isAuthenticated` a `null` (en lugar de `false`)
- Agregado verificación de ruta `/payment-simulation` antes de checkAuthentication
- Agregado estado de loading mientras se verifica autenticación
- Mejorada lógica para no redirigir al login en páginas públicas

### paymentService.ts
- Creada instancia separada de axios (`publicApi`) sin autenticación
- Los endpoints de pago ahora usan `publicApi` en lugar de `api`
- Esto permite que la página de simulación funcione sin token JWT

### PaymentsController.cs
- Agregado atributo `[AllowAnonymous]` a todos los endpoints de pagos
- `/api/payments/health` - público
- `/api/payments/webhook` - público  
- `/api/payments/venta/{ventaId}` - público

## Troubleshooting

### El QR ahora funciona pero el pago no se procesa
- Verifica que el backend esté corriendo en http://localhost:5164
- Revisa la consola del navegador para errores
- Verifica que el endpoint `/api/payments/health` responda
- Revisa los logs del backend para ver si el webhook se recibe

### El estado del pago no se actualiza
- El sistema hace polling cada 3 segundos
- El timeout es de 5 minutos
- Verifica que no haya errores de red
- Revisa que la venta exista en la base de datos

### Problemas de red desde el celular
- Asegúrate de que tu celular esté en la misma red WiFi
- Verifica que tu IP local esté configurada correctamente
- Verifica que el firewall no bloquee el puerto 5174

## URL de Simulación Directa

Para probar directamente la página de simulación (sin login):
```
http://192.168.0.95:5173/payment-simulation?payment_id=TEST-123&venta_id=1&monto=100
```

Esta URL ahora funciona directamente sin requerir autenticación.

## Para Producción

En producción, el sistema debe:
- Usar HTTPS
- Configurar dominios reales en lugar de IPs locales
- Implementar webhooks reales de proveedores de pago (CUCU, Circle, etc.)
- Usar variables de entorno para configuración
- Considerar seguridad adicional (aunque los endpoints de pago deben seguir siendo públicos para webhooks externos)