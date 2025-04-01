# Analizador de Gmail

## Descripción
GmailAnalyzer es una herramienta que ayuda a procesar y analizar automáticamente los correos electrónicos no leídos de Gmail utilizando inteligencia artificial. El programa utiliza la API de Gmail para acceder a los correos y OpenAI para analizar su contenido, proporcionando resúmenes y puntuaciones de importancia.

## Objetivo
El objetivo principal es ayudar a los usuarios a gestionar de manera más eficiente su bandeja de entrada, identificando rápidamente los correos más importantes y proporcionando resúmenes concisos de su contenido.

## Características
- Lectura automática de correos no leídos de Gmail
- Análisis de contenido utilizando OpenAI
- Puntuación de importancia para cada correo
- Generación de resúmenes concisos
- Contextualización basada en el equipo y proyecto del usuario

## Requisitos
- Credenciales de Gmail (archivo de credenciales)
- Clave API de OpenAI
- Archivo appsettings.json configurado

## Configuración
1. Crear un archivo `appsettings.json` con la siguiente estructura:
```json
{
  "OpenAI": {
    "ApiKey": "tu-clave-api"
  },
  "Gmail": {
    "CredentialsPath": "ruta-a-tus-credenciales"
  }
}
```

2. Configurar las credenciales de Gmail
3. Configurar la clave API de OpenAI

## Uso
Ejecutar el programa y automáticamente:
1. Leerá los correos no leídos
2. Analizará cada correo
3. Mostrará los resultados ordenados por importancia

## Salida
Por cada correo analizado, se mostrará:
- Asunto del correo
- Puntuación de importancia (1-10)
- Resumen del contenido
