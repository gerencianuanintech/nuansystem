# Orden de Compra - Documentos Relacionados

Esta guia explica la primera version visual de la pestana **Documentos Relacionados** dentro del formulario de Orden de Compra.

El objetivo de esta pantalla es que el usuario pueda ver que documentos estan vinculados a una orden de compra, por ejemplo una solicitud de compra, una cotizacion, una entrada de mercancia o una factura del proveedor.

## Alcance desarrollado

En este paso se trabajo solo el diseno del formulario WinForms. No se agrego logica de base de datos, no se creo integracion con SAP y no se conectaron acciones reales a los botones.

Se modifico el formulario:

`src/Frontend/NuanSystem.WinForms.Forms/Purchasing/PurchaseOrders/FrmPurchaseOrderEdit.Designer.cs`

Se agrego apoyo visual y datos de ejemplo en:

`src/Frontend/NuanSystem.WinForms.Forms/Purchasing/PurchaseOrders/FrmPurchaseOrderEdit.cs`

## Vista para el usuario

Al abrir la orden de compra, la pestana activa queda en **Documentos Relacionados**.

La pestana muestra estos botones internos:

- **Agregar Relacion**: preparado para vincular un documento nuevo a la orden.
- **Ver Documento**: preparado para abrir o consultar el documento seleccionado.
- **Desvincular**: preparado para quitar la relacion entre la orden y el documento seleccionado.
- **Actualizar**: preparado para refrescar la lista de documentos relacionados.

Debajo de los botones se muestra la tabla **Documentos Relacionados** con columnas para:

- Tipo de documento.
- Serie.
- Numero.
- Fecha.
- Estado.
- Referencia.
- Comentario.
- Total.
- Accion.

En la parte inferior se agrego la seccion **Observaciones**, donde el usuario podra escribir comentarios generales sobre los documentos relacionados.

## Datos de ejemplo

Mientras no exista conexion real con backend para esta pestana, el formulario carga documentos de ejemplo cuando se abre en modo de referencia visual:

- Solicitud de Compra `SC-2026 / SC-000245`.
- Cotizacion Proveedor `COT-2026 / COT-00321`.
- Orden de Venta Interna `OVI-2026 / OVI-000112`.
- Entrada de Mercancia `EM-2026 / EM-000178`.
- Factura Proveedor `FAC-2026 / FAC-000321`.

Estos datos sirven solo para validar el diseno, el ancho de columnas y la lectura visual de la pantalla.

## Detalle tecnico

La grilla usa `GridControl` y `GridView` de DevExpress para mantener consistencia con el resto del ERP.

Las columnas principales del diseno son:

- `colRelatedDocumentType`: muestra el tipo de documento.
- `colRelatedDocumentSeries`: muestra la serie.
- `colRelatedDocumentNumber`: muestra el numero.
- `colRelatedDocumentDate`: muestra la fecha en formato `dd/MM/yyyy`.
- `colRelatedDocumentStatus`: muestra el estado.
- `colRelatedDocumentReference`: muestra la referencia.
- `colRelatedDocumentComment`: muestra el comentario.
- `colRelatedDocumentTotal`: muestra el total con formato numerico `N2`.
- `colRelatedDocumentAction`: deja preparada una accion por fila.

El metodo `ApplyRelatedDocumentStatusStyle` aplica colores suaves al estado:

- Verde para `Aprobada`, `Aceptada` o `Recibida`.
- Azul para `Confirmada`.
- Naranja para `Pendiente`.

El metodo `ApplyRelatedDocumentButtonIcons` asigna iconos a los botones internos cuando el recurso SVG existe en el proyecto.

## Pendiente para el siguiente paso

La pantalla ya queda lista a nivel visual. El siguiente trabajo funcional seria definir y desarrollar:

- Modelo de datos real para relaciones entre documentos.
- Endpoints backend para listar, agregar, ver y desvincular relaciones.
- Repositorio y procedimiento almacenado para persistir las relaciones.
- Eventos de los botones del formulario.
- Pruebas de carga de una orden con documentos relacionados reales.
