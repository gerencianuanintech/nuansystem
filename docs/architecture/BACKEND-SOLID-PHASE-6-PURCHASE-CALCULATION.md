# Fase 6: calculo puro de ordenes de compra

Fecha: 2026-10-23. Proceso operativo; extraccion de reglas existentes, sin cambios monetarios.

## Descubrimiento y decisiones

PurchaseOrderCalculator combina importes con DTO de persistencia, metadata y valores SAP. Solo lo llaman CreatePurchaseOrderCommandHandler, UpdatePurchaseOrderCommandHandler y PurchaseOrderCollections.UpdateAsync. Domain no tiene calculador equivalente. Se reutiliza el patron de tipos puros Domain y el adaptador Application existente, sin agregado, repositorio ni servicio nuevo.

14 casos de caracterizacion sobre la implementacion anterior ya aprobaron: descuentos de linea/cabecera, proporcion del impuesto, impuesto cero/mixto, lineas vacias/filtradas, limites y clamp, precision de cantidad/precio, overflow, moneda, metadata y auditoria. Se conservan como regresion despues de extraer.

Domain/Purchasing recibe solo valores numericos de lineas y porcentaje de cabecera. Devuelve resultados numericos de lineas y documento. Redondea a seis decimales con MidpointRounding.ToEven en bruto, descuento de linea, impuesto de linea, descuento de cabecera e impuesto de documento. Conserva clamp de descuentos 0..100, precio/impuesto no negativos, impuesto sin limite superior y overflow decimal explicito. El descuento de cabecera reduce proporcionalmente el impuesto del documento; no se redistribuye en las lineas persistidas. No aplica conversion de moneda: el tipo de cambio sigue siendo metadata. Se mantienen defaults USD/1 en el adaptador actual.

Application filtra ItemId/cantidad invalidos, conserva orden/numeracion de lineas y transforma el resultado puro a PurchaseOrderPersistData, con los mismos DTO, metadata, truncamientos, fechas, estados y auditoria. Permisos, workflow, repositorios, transacciones y publicacion permanecen en Application/Persistence. Se permite acceso de la asamblea de pruebas a los internos de Application, siguiendo el patron ya usado por Persistence/SapIntegration, para caracterizar sin hacer publico el adaptador.

## SAP y compatibilidad historica

El codigo de objeto 22 se centraliza en Features/SapSync/Mapping, siguiendo los mapeos de integracion Application existentes. Lo referencian el adaptador, el default del DTO y el mensaje de solicitud SAP. No se agrega dependencia de Domain a SAP ni se requiere un cliente SAP para calcular. SQL 046 conserva DEFAULT/CHECK SapObjectType=22 y los parametros actuales; los registros historicos y el contrato HTTP siguen iguales. No se modifica ni ejecuta SQL.

## Decision independiente: numeracion

La creacion utiliza la serie autorizada cargada del servidor y el numero reservado. Edicion y cambios de adjuntos/documentos relacionados usan los identificadores del documento existente. Los defaults OC-2026/OC-000001 del adaptador podian inventar numeracion si faltaba metadata. Se retiran por separado de la extraccion aritmetica: se rechaza una serie cargada sin codigo antes de reservar; se rechaza un documento existente sin serie/numero antes de escribir, sin renumerarlo. El adaptador exige identificadores en vez de inventarlos. Se agregan pruebas dedicadas; no se alteran los importes ni truncamientos existentes.

## Registro operativo y limites

Actor: usuario autorizado por empresa y serie. Entradas: lineas/descuento y metadata; los importes se recalculan en backend. Estado: Draft/Rejected/SapError editables segun politica vigente; no hay cambio de workflow. Stock/caja: sin movimientos nuevos. Persistencia/transaccion: mismas llamadas/procedimientos y guards de concurrencia. Auditoria: mismos usuario, fechas y resultados. Integracion: solo constante de mapeo, sin llamadas remotas. Reintento/rollback: sin cambios. Pruebas: caracterizacion antes/despues, calculador Domain, adaptacion, numeracion y workflow/persistencia estatica existentes.

Trabajo en checkout actual con snapshot/copias en artifacts/backend-solid-phase6. Se conservan cambios previos, sin stash/reset/commit/cambio de rama. No se inician API/workers ni se ejecutan SAP o DB. Las fases 7 y siguientes quedan fuera. Las aristas nuevas son PurchaseOrderCalculator(Application) -> PurchaseOrderAmountCalculator(Domain) y Application adapter/DTO -> SapPurchaseOrderMapping; el patron de repositorio de los catalogos internos no cambia.
