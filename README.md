# Módulo de Facturación para Farmacia

## Descripción
Este proyecto es un módulo de facturación diseñado para una farmacia. Permite gestionar clientes, productos y la generación de facturas de manera sencilla y eficiente. Está desarrollado en **C#** utilizando **Windows Forms** como interfaz gráfica, y **SQL Server** para la gestión de la base de datos. La aplicación sigue una **arquitectura de n capas**, lo que facilita el mantenimiento, escalabilidad y separación de responsabilidades.

## Características principales

- Gestión de clientes: Crear, editar y consultar clientes.
- Gestión de productos: Crear, editar y consultar productos.
- Generación de facturas: Crear facturas seleccionando cliente y productos, con cálculo automático del total.
- Base de datos SQL Server para almacenamiento persistente.
- Arquitectura en n capas para una mejor organización y mantenimiento.

## Arquitectura

El sistema está organizado en las siguientes capas:

- **Capa de Presentación:** Windows Forms para la interfaz con el usuario.
- **Capa de Negocio:** Lógica de negocio y validaciones.
- **Capa de Acceso a Datos:** Comunicación con SQL Server.
- **Capa de Entidades:** Modelos que representan los datos del sistema.

## Tecnologías utilizadas

- C#
- .NET Framework (Windows Forms)
- SQL Server
- Arquitectura en n capas

## Requisitos previos

- Visual Studio 2017 o superior
- SQL Server 2012 o superior
- .NET Framework instalado (según versión del proyecto)
- Configuración de cadena de conexión a SQL Server

## Instalación y configuración

1. Clonar o descargar este repositorio.
2. Abrir la solución `.sln` con Visual Studio.
3. Configurar la cadena de conexión en `app.config` o archivo correspondiente.
4. Compilar y ejecutar la aplicación.

## Uso

1. Crear clientes con sus datos básicos.
2. Registrar productos con nombre, precio y stock.
3. Generar facturas seleccionando cliente y productos; el sistema calcula el total automáticamente.
4. Consultar y administrar clientes, productos y facturas desde la aplicación.

## Estructura del proyecto

/Presentacion -> Windows Forms (Interfaz)

/Negocio -> Lógica de negocio

/AccesoDatos -> Acceso a la base de datos

/Entidades -> Modelos de datos

/ScriptsSQL -> Scripts para base de datos

## Autor

- [Daniel Moyolema]
- [GitHub: https://github.com/daniel-devlp]

## Licencia

Este proyecto está bajo la licencia MIT. Ver archivo LICENSE para más detalles.
