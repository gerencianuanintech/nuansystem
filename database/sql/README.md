# Scripts SQL

Aqui se ubicaran los scripts de `NuanSystem_Master`, tablas por empresa, seed inicial, migraciones manuales y objetos auxiliares para SQL Server.

La compatibilidad futura con MySQL se trabajara desde abstracciones de persistencia y scripts separados por motor cuando sea necesario.

## Sincronizacion Maestro-Sucursal

- `074_apply_master_branch_sync_master.sql`: instalador SQLCMD de objetos Master.
- `075_apply_master_branch_sync_tenant.sql`: instalador SQLCMD de objetos por sucursal.
- `076_check_master_branch_sync_installation.sql`: diagnostico de instalacion sin cambios de datos.
- `080_sync_entity_definitions.sql`: catalogo Master de definiciones de entidades, dependencias, auditoria y procedimientos CRUD de persistencia.
- `083_tenant_country_master_branch_sync.sql`: identidad global y contratos CRUD tenant para sincronizar paises.
- `084_master_country_sync_registration.sql`: registra Countries como capacidad operativa sin activar perfiles.
- `085_tenant_province_master_branch_sync.sql`: identidad global y contratos CRUD tenant para sincronizar provincias.
- `086_master_province_sync_registration.sql`: registra Provinces como capacidad operativa dependiente de Countries.
- `087_tenant_city_master_branch_sync.sql`: identidad global y contratos CRUD tenant para sincronizar ciudades.
- `088_master_city_sync_registration.sql`: registra Cities como capacidad operativa dependiente de Countries y Provinces.
- `089_sap_service_layer_company_settings.sql`: agrega procedimientos y auditoria segura para configurar Service Layer por empresa sin exponer credenciales.
- `090_tenant_currency_master_branch_sync.sql`: crea o normaliza Currencies, agrega GlobalId y contratos CRUD tenant para sincronizar monedas.
- `091_master_currency_sync_registration.sql`: registra Currencies como capacidad operativa sin activar perfiles ni distribuciones.
- `092_sync_routing_by_target_branch.sql`: permite enrutar maestros dependientes de ubicacion, como bodegas, solamente a la sucursal tenant indicada.
- `093_sync_distribution_policies.sql`: agrega modos None/All/Selected/Rule, selecciones por GlobalId, reglas seguras y auditoria de decisiones por sucursal.
- `097_tenant_item_group_master_branch_sync.sql`: normaliza ItemGroups, agrega contratos de aplicacion idempotente y conserva GlobalId entre tenants.
- `098_master_item_group_sync_registration.sql`: registra ItemGroups como entidad operativa y declara la dependencia Item -> ItemGroups.
- `099_master_sync_dependency_engine.sql`: registra definiciones futuras y el grafo inicial de dependencias para listas de precios y ordenes de compra, sin activar productores ni aplicadores.
- `127_tenant_item_family_master_branch_sync.sql`: normaliza ItemFamilies, agrega GlobalId, repara las proyecciones CRUD y crea el aplicador idempotente sin adopcion automatica por codigo.
- `128_master_item_family_sync_registration.sql`: registra ItemFamilies y sus dependencias ItemGroups -> ItemFamilies -> Item, manteniendo perfiles y workers deshabilitados.
- `135_tenant_warehouse_tombstone_code_reservation.sql`: reserva los codigos de Warehouse eliminados logicamente mediante validacion CRUD e indice unico no filtrado.
- `136_tenant_currency_transactional_outbox.sql`: endurece Currency con LocalOutbox transaccional, reserva de tombstones y aplicacion por GlobalId sin adopcion por codigo.
- `137_master_currency_transactional_registration.sql`: registra el contrato Currency transaccional, deshabilitado por defecto, y conserva la dependencia PriceList -> Currencies.
- `138_tenant_sri_txt_import.sql`: agrega cargas TXT SRI tenant, detalle normalizado, estado `Staged` y enqueue explícito idempotente.
- `139_master_sri_txt_import_security.sql`: registra permisos API de cargas TXT SRI sin concederlos automáticamente a roles.
- `198_tenant_product_types_master.sql`: evoluciona ProductTypes con naturaleza ERP cerrada, auditoria, LocalOutbox transaccional y aplicacion por GlobalId sin adopcion por codigo.
- `199_master_definitions_inventory_product_types_navigation.sql`: conserva identidades y accesos legacy al mover Tipos de producto a Configuracion > Definiciones > Inventario.
- `200_master_product_types_sync_registration.sql`: registra ProductType antes de Item con configuracion y ownership deshabilitados por defecto.
- `201_tenant_item_lines_master.sql`: evoluciona ItemLines con GlobalId, orden, auditoria, LocalOutbox transaccional y aplicacion por GlobalId sin adopcion por codigo; no agrega referencias SAP/externas.
- `202_master_definitions_inventory_item_lines_navigation.sql`: conserva identidades y accesos legacy al mover Lineas de articulos a Configuracion > Definiciones > Inventario con FormKey `item-lines`.
- `203_master_item_lines_sync_registration.sql`: registra ItemLine sin dependencias funcionales, con configuracion y ownership deshabilitados por defecto.
- `204_master_product_types_dependency_repair.sql`: retira de forma forward-only la dependencia prematura Item -> ProductType hasta que el payload de articulos publique ProductTypeGlobalId.
- `205_tenant_item_subgroups_master.sql`: convierte ItemSubgroups en maestro dependiente de ItemFamilies, preserva relaciones historicas verificables, agrega auditoria y Outbox transaccional, y aplica por GlobalId sin adopcion por codigo.
- `206_master_definitions_inventory_item_subgroups_navigation.sql`: conserva identidades, accesos y denegaciones al mover Subgrupos de articulos a Configuracion > Definiciones > Inventario con FormKey `item-subgroups`.
- `207_master_item_subgroups_sync_registration.sql`: registra ItemSubgroups despues de ItemFamilies, con configuracion y ownership deshabilitados por defecto.
- `208_tenant_item_origins_master.sql`: crea ItemOrigins con GlobalId, seeds deterministas `Local`/`Imported`/`Mixed`, preservacion exacta de valores JSON historicos, auditoria, LocalOutbox y aplicacion por GlobalId sin adopcion por codigo.
- `209_master_definitions_inventory_item_origins_navigation.sql`: registra Origenes de articulos en Configuracion > Definiciones > Inventario con permisos API y operaciones de formulario.
- `210_master_item_origins_sync_registration.sql`: registra ItemOrigin independiente, sin dependencia Item, con configuracion y ownership deshabilitados por defecto.
- `211_tenant_replenishment_methods_master.sql`: evoluciona ReplenishmentMethods preservando Id, codigos y JSON historico; agrega GlobalId, auditoria, LocalOutbox y sync sin adopcion por codigo.
- `212_master_definitions_inventory_replenishment_methods_navigation.sql`: migra formulario/menu legacy a FormKey `replenishment-methods` preservando identidades y accesos.
- `213_master_replenishment_methods_sync_registration.sql`: registra ReplenishmentMethod independiente y deshabilitado por defecto, sin dependencia Item.
- `214_tenant_storage_conditions_master.sql`: evoluciona StorageConditions preservando Id, códigos, casing y JSON histórico exacto; agrega GlobalId, auditoría, tombstones y sync sin adopción por código.
- `215_master_definitions_inventory_storage_conditions_navigation.sql`: migra formulario/menu legacy a FormKey `storage-conditions` preservando identidades y accesos.
- `216_master_storage_conditions_sync_registration.sql`: registra StorageCondition independiente, sin dependencia Item y deshabilitado por defecto.
- `217_tenant_item_commercial_segments_master.sql`: crea el maestro tenant de segmentos comerciales de artículos con auditoría, lookup y eliminación lógica.
- `218_master_definitions_inventory_item_commercial_segments_navigation.sql`: registra formulario, menú, permisos y operaciones aplicables para `item-commercial-segments`.
- `219_master_item_commercial_segments_form_operations_repair.sql`: reparación forward-only que completa las doce operaciones canónicas visibles en Accesos y su concesión inicial a `ADMIN`.
- `220_master_item_origins_form_operations_repair.sql`: reparación forward-only que completa las doce operaciones canónicas de Orígenes de artículos visibles en Accesos y su concesión inicial a `ADMIN`.
- `221_tenant_item_alert_types_master.sql`: crea el maestro tenant de Tipos de alerta de artículos, sus procedimientos CRUD, consulta para selector y auditoría.
- `222_master_definitions_inventory_item_alert_types_navigation.sql`: registra formulario, menú, permisos y las doce operaciones canónicas de Tipos de alerta de artículos.
- `223_tenant_item_auxiliary_delete_hardening.sql`: reparación forward-only que hace atómica la eliminación lógica y su auditoría, preservando el resultado real de filas afectadas, para Orígenes, Segmentos comerciales y Tipos de alerta de artículos.
- `224_master_item_auxiliary_navigation_hardening.sql`: reparación forward-only que reactiva sin duplicar formularios, menús, accesos de menú y operaciones de Orígenes y Segmentos comerciales.
- `225_master_item_alert_types_unicode_repair.sql`: reparación forward-only que corrige y protege la etiqueta Unicode de Tipos de alerta de artículos en Master.
- `226_tenant_sales_channels_master.sql`: evoluciona Canales de venta preservando Id, códigos y el consumo actual por código de ItemEdit; agrega GlobalId, orden, auditoría y CRUD independiente sin activar sincronización.
- `227_master_definitions_inventory_sales_channels_navigation.sql`: migra formulario y menú legacy a `sales-channels`, registra permisos API y las doce operaciones canónicas preservando identidades y accesos.
| `100_tenant_purchase_reference_catalog_sync.sql` | Tenant | Normaliza impuestos, unidades de medida y listas de precios para sincronizacion previa a ordenes. |
| `101_tenant_sap_purchase_order_import.sql` | Tenant | Agrega identidad, version SAP y estado de enrutamiento a ordenes de compra. |
| `112_tenant_sap_payment_terms_sync.sql` | Tenant | Importacion idempotente SAP B1 y aplicacion por GlobalId de condiciones de pago. |
| `113_master_payment_terms_sync_registration.sql` | Master | Registra PaymentTerms SAPToErp y activa el contrato Matriz-Sucursal. |
| `114_master_payment_terms_sync_configuration.sql` | Master | Completa SyncEntityConfigurations sin activar perfiles ni workers. |
| `102_purchase_order_warehouse_routing.sql` | Master | Configura rutas de ordenes por bodega para el piloto DEMO. |
| `103_tenant_purchase_order_sync.sql` | Tenant | Agrega auditoria de decisiones de enrutamiento. |
| `104_master_demo_purchase_order_pilot_profile.sql` | Master | Completa dependencias y PurchaseOrder en el perfil piloto DEMO. |
| `105_master_activate_reference_and_purchase_order_sync.sql` | Master | Activa las capacidades declaradas de impuestos, unidades, listas de precios y ordenes de compra. |
