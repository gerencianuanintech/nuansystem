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
| `100_tenant_purchase_reference_catalog_sync.sql` | Tenant | Normaliza impuestos, unidades de medida y listas de precios para sincronizacion previa a ordenes. |
| `101_tenant_sap_purchase_order_import.sql` | Tenant | Agrega identidad, version SAP y estado de enrutamiento a ordenes de compra. |
| `112_tenant_sap_payment_terms_sync.sql` | Tenant | Importacion idempotente SAP B1 y aplicacion por GlobalId de condiciones de pago. |
| `113_master_payment_terms_sync_registration.sql` | Master | Registra PaymentTerms SAPToErp y activa el contrato Matriz-Sucursal. |
| `114_master_payment_terms_sync_configuration.sql` | Master | Completa SyncEntityConfigurations sin activar perfiles ni workers. |
| `102_purchase_order_warehouse_routing.sql` | Master | Configura rutas de ordenes por bodega para el piloto DEMO. |
| `103_tenant_purchase_order_sync.sql` | Tenant | Agrega auditoria de decisiones de enrutamiento. |
| `104_master_demo_purchase_order_pilot_profile.sql` | Master | Completa dependencias y PurchaseOrder en el perfil piloto DEMO. |
| `105_master_activate_reference_and_purchase_order_sync.sql` | Master | Activa las capacidades declaradas de impuestos, unidades, listas de precios y ordenes de compra. |
