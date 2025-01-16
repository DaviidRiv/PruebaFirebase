# Proyecto Web en .Net Core 8 MVC

## Descripción General
Este proyecto es una aplicación web desarrollada en **.Net Core 8 MVC**, que utiliza varias herramientas y servicios modernos para ofrecer funcionalidades de autenticación, gestión de datos, y almacenamiento de archivos.

### Características Principales:
- **Autenticación de Usuarios**:
  - Uso de **Firebase** para autenticación por correo electrónico.
  - Integración con **Google API** para autenticación mediante correo.
  - Recuperación de contraseña a través de correo electrónico.
  - Registro de nuevos usuarios mediante formularios web.

- **Gestor de Teléfonos (CRUD)**:
  - Creación, lectura, actualización y eliminación de teléfonos.
  - Uso de **Firebase Realtime Database** como base de datos NoSQL.

- **Almacenamiento de Archivos**:
  - Integración con **Dropbox API** para almacenar imágenes de los teléfonos.
  - Guardado de la ruta de las imágenes y su URL en Firebase Realtime Database para su visualización en la aplicación web.

## Tecnologías Utilizadas

### Frameworks y Lenguajes:
- **.Net Core 8 MVC**
- **C#**

### Servicios y APIs:
- **Firebase**:
  - Firebase Authentication
  - Firebase Realtime Database
- **Google API**:
  - Google Authentication
- **Dropbox API**:
  - Almacenamiento de imágenes

### Paquetes NuGet:
- [FirebaseAdmin](https://www.nuget.org/packages/FirebaseAdmin)
- [Google.Apis.Auth](https://www.nuget.org/packages/Google.Apis.Auth)
- [FirebaseDatabase.net](https://www.nuget.org/packages/FirebaseDatabase.net)
- [Dropbox.Api](https://www.nuget.org/packages/Dropbox.Api)
