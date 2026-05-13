# Fase 9: Modulo de Documentos Comerciales

## Objetivo

Implementar la primera version del modulo de documentos comerciales con cabecera, detalle, calculo de totales y persistencia transaccional contra la base de datos de la empresa activa.

## Alcance Implementado

### 1. Contrato de repositorio

Se agrego:

- `IDocumentRepository`

Ubicacion:

- `src/Backend/NuanSystem.Application/Abstractions/Data/IDocumentRepository.cs`

Operaciones:

- `GetAllAsync`
- `GetByIdAsync`
- `CreateAsync`
- `CustomerExistsAsync`
- `GetMissingItemIdsAsync`

### 2. DTOs

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Documents/Dtos`

Archivos:

- `DocumentDto`
- `DocumentSummaryDto`
- `DocumentLineDto`
- `CreateDocumentData`
- `CreateDocumentLineData`

### 3. Queries

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Documents/Queries`

Archivos:

- `GetDocumentsQuery`
- `GetDocumentsQueryHandler`
- `GetDocumentByIdQuery`
- `GetDocumentByIdQueryHandler`

### 4. Command de creacion

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Documents/Commands`

Archivos:

- `CreateDocumentCommand`
- `CreateDocumentLineCommand`
- `CreateDocumentCommandValidator`
- `CreateDocumentCommandHandler`

Tipos de documento permitidos:

- `SalesOrder`
- `Delivery`
- `Invoice`

### 5. Validaciones

Validaciones principales:

- Tipo de documento requerido y permitido.
- Cliente requerido.
- Fecha requerida.
- Moneda requerida de 3 caracteres.
- Al menos una linea.
- Cada linea requiere articulo.
- Cantidad mayor a 0.
- Precio mayor o igual a 0.
- `TaxRate` entre 0 y 1.
- Cliente activo existente.
- Articulos activos existentes.

`TaxRate` se maneja como proporcion decimal:

- `0.12` representa 12%.
- `0` representa exento.

### 6. Calculo de totales

El handler calcula:

- `LineTotal = Quantity * UnitPrice`
- `Subtotal = suma de LineTotal`
- `TaxTotal = suma de LineTotal * TaxRate`
- `Total = Subtotal + TaxTotal`

Los calculos se redondean a 6 decimales usando `MidpointRounding.AwayFromZero`.

### 7. Persistencia transaccional

Se implemento:

- `DocumentRepository`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Repositories/DocumentRepository.cs`

Responsabilidades:

- Insertar cabecera.
- Generar `DocumentNumber`.
- Insertar lineas.
- Confirmar transaccion.
- Revertir transaccion si ocurre error.

Formato inicial de numero:

```text
DOCUMENTTYPE-000001
```

Ejemplo:

```text
SALESORDER-000001
```

### 8. Tablas utilizadas

El modulo usa tablas tenant creadas desde Fase 4:

- `Documents`
- `DocumentLines`
- `Customers`
- `Items`

Tambien deja el camino preparado para SAP mediante:

- `SapSyncLog`

### 9. Endpoints REST

Listar documentos:

```http
GET /api/documents
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

Consultar documento por id:

```http
GET /api/documents/{id}
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

Crear documento:

```http
POST /api/documents
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
Content-Type: application/json
```

Body ejemplo:

```json
{
  "documentType": "SalesOrder",
  "customerId": 1,
  "documentDate": "2026-04-28",
  "currency": "USD",
  "lines": [
    {
      "itemId": 1,
      "quantity": 2,
      "unitPrice": 10,
      "taxRate": 0.12
    }
  ]
}
```

## Flujo de Creacion

```text
POST /api/documents
  -> CreateDocumentCommand
    -> FluentValidation
    -> Validar cliente activo
    -> Validar articulos activos
    -> Calcular totales
    -> DocumentRepository.CreateAsync
      -> Insertar cabecera
      -> Generar DocumentNumber
      -> Insertar lineas
      -> Commit
    -> Consultar documento completo
```

## Verificacion Realizada

Se ejecuto:

```powershell
dotnet build NuanSystem.sln --no-restore
```

Resultado:

```text
Compilacion correcta.
0 advertencias
0 errores
```

Tambien se levanto temporalmente la API y se valido:

```http
GET http://localhost:5081/health
```

Resultado:

```text
200 Healthy
```

## Pendientes

- Probar contra SQL Server real cuando exista tenant.
- Agregar update/cancelacion de documentos.
- Agregar estados formales y transiciones.
- Validar stock para articulos inventariables.
- Agregar numeracion configurable por empresa y tipo de documento.
- Agregar impuestos configurables.
- Integrar envio a SAP Business One.
- Agregar permisos finos por tipo de documento.

## Estado de la Fase

La Fase 9 queda implementada a nivel de API, Application, Persistence y documentacion.

La siguiente fase natural es Fase 10: integracion SAP Business One.
