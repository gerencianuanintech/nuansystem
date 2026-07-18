# Motor de dependencias de sincronizacion Master/Sucursal

## Decision

Las dependencias entre entidades se administran en Master mediante
`SyncEntityDefinitions` y `SyncEntityDefinitionDependencies`. El catalogo es la
fuente de verdad; WinForms solo lo presenta y la API/Application aplica las
reglas.

El motor cubre dos niveles distintos:

1. **Dependencia de ejecucion**: una entidad requerida debe estar activa en el
   mismo perfil, habilitada para la misma sucursal y ejecutarse antes que la
   entidad dependiente.
2. **Dependencia de registro**: cuando un payload referencia un maestro concreto,
   el aplicador verifica que ese registro exista en la sucursal. Si aun no existe,
   el target queda en error reprocesable y conserva auditoria para reintento.

## Reglas

- No se hardcodean dependencias en WinForms.
- Application obtiene el grafo desde el catalogo Master.
- La validacion rechaza dependencias ausentes, inactivas o deshabilitadas en la
  misma sucursal.
- Un orden manual incompatible produce una advertencia; la ejecucion siempre
  usa orden topologico y emplea `ExecutionOrder` solo como desempate estable.
- Si una ejecucion manual solicita una entidad, se incorpora automaticamente el
  cierre transitivo de sus dependencias configuradas.
- Los ciclos se siguen rechazando al mantener el catalogo. El ordenamiento
  tambien falla de forma segura si recibe un grafo ciclico o incompleto.
- Una dependencia de registro pendiente no se marca `Ignored` ni `Applied`; se
  programa como error reprocesable y puede pasar a `DeadLetter` al agotar la
  politica de intentos existente.
- La primera comprobacion de registro es `Item -> ItemGroups`, usando el codigo
  o identificador funcional recibido en el payload y la conexion tenant resuelta
  por backend.

## Limites

- El motor no crea productores, full sources ni aplicadores para entidades nuevas.
- Las definiciones futuras pueden existir como borradores no operativos hasta su
  implementacion en una fase posterior.
- SAP no participa en el ordenamiento Master/Sucursal.
- No se sincronizan stock, kardex, costos, lotes, series ni documentos como parte
  de esta decision.

## Orden inicial

El catalogo conserva ordenes predeterminados coherentes con la ruta prevista:

1. `Currencies`
2. `Tax`
3. `UnitOfMeasure`
4. `ItemGroups`
5. `Warehouse`
6. `BusinessPartner`
7. `Item`
8. `PriceList`
9. `PurchaseOrder`

Los catalogos geograficos y auxiliares existentes mantienen su propio orden y
dependencias. La operatividad real de cada definicion depende de que existan
productor y aplicador registrados.
