# Validación controlada de Condiciones de Pago

## Precondiciones

- Rama y binarios de Iteración 4 actualizados.
- Scripts `112` ejecutados en Matriz y todas las sucursales participantes.
- Scripts `113` y `114` ejecutados en `NuanSystem_Master`.
- Credenciales SAP cifradas mediante la configuración tenant; nunca en Git o línea de comandos persistida.
- Perfil Matriz-Sucursal aprobado y activo para `BusinessPartnerPaymentTerms`.

## Diagnóstico TLS en desarrollo

Si Postman conecta pero el worker rechaza el certificado interno, configurar localmente y solo durante la prueba:

```json
{
  "ServiceLayer": {
    "IgnoreSslErrors": true
  }
}
```

Ejecutar el worker con `DOTNET_ENVIRONMENT=Development`. El proceso debe rechazar esta opción en cualquier otro ambiente. Restaurar `IgnoreSslErrors=false` al finalizar. La corrección definitiva es instalar una cadena de certificados confiable para SAP Service Layer.

## Perfil y worker Matriz-Sucursal

En la administración de perfiles:

- dirección: `MasterToBranch`;
- modo del perfil y de la entidad: `Incremental` para recibir los eventos publicados por la importación SAP;
- entidad: `BusinessPartnerPaymentTerms`;
- operaciones: insertar, actualizar y desactivar;
- distribución por sucursal: `All`;
- sucursales: únicamente las aprobadas.

En configuración local del worker:

```json
{
  "MasterBranchSyncWorker": {
    "Enabled": true,
    "SkeletonMode": false,
    "EnabledEntityAppliers": [
      "BusinessPartnerPaymentTerms"
    ]
  }
}
```

No sustituir permanentemente otros aplicadores existentes al editar el arreglo; para una prueba aislada puede limitarse temporalmente a esta entidad y después restaurarse.

## Evidencia mínima

1. Login y lectura real de `PaymentTermsTypes`.
2. Filas SAP creadas/actualizadas en la Matriz con `SAP_B1`, `GroupNumber`, `Days`, `IsCredit` y `GlobalId`.
3. Segunda importación `Unchanged` conservando `GlobalId` y generando reconciliación.
4. `SyncOutbox` y targets para cada sucursal aprobada.
5. `SyncInbox` y auditoría con estado aplicado.
6. Filas de sucursal con los mismos `GlobalId`, referencia SAP, `Days` e `IsCredit`.
7. Colisión controlada que no sobrescriba un registro local.

Finalizada la prueba, restaurar flags temporales, detener procesos manuales y confirmar que no quedaron locks vencidos.
