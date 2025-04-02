# Analizador de Gmail

Una herramienta para analizar mensajes de Gmail utilizando las APIs de OpenAI.

## Objetivo del Proyecto

Este proyecto tiene como objetivo ayudar a los usuarios a analizar y procesar automáticamente sus correos electrónicos de Gmail mediante inteligencia artificial. Permite:

- Analizar el contenido de los correos electrónicos
- Categorizar mensajes según su importancia
- Generar resúmenes automáticos de correos largos
- Identificar acciones requeridas en los mensajes
- Ayudar a priorizar la bandeja de entrada

## Cómo Usar

1. **Configuración Inicial**:
   - Clona este repositorio
   - Instala las dependencias necesarias
   - Configura tu archivo `appsettings.json` con tus credenciales

2. **Ejecución**:
   - Ejecuta la aplicación
   - Autoriza el acceso a tu cuenta de Gmail cuando se solicite
   - Selecciona las opciones de análisis según tus necesidades

3. **Interpretación de Resultados**:
   - Revisa los análisis generados
   - Utiliza los resúmenes y categorizaciones para gestionar tu bandeja de entrada

## Configuración

La aplicación utiliza un archivo `appsettings.json` para la configuración:

```json
{
  "OpenAI": {
    "ApiKey": "tu-clave-api-de-openai"
  },
  "Gmail": {
    "Email": "tu-correo@gmail.com",
    "Password": "tu-contraseña"
  },
  "Context": {
    "Prompt": "Tu prompt de contexto personalizado aquí..."
  }
}
```

### Prompt de Contexto

El prompt de contexto se utiliza para proporcionar información personalizada a la IA al analizar los correos. Contiene información sobre:

- Tu rol y responsabilidades
- Tu estructura organizacional
- Proyectos actuales y prioridades
- Miembros del equipo
- Preferencias de filtrado de correos

Personaliza el prompt de contexto en el archivo `appsettings.json` para adaptar el análisis a tus necesidades específicas.
