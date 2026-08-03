# Evidencia de validación de consulta SAP de Bodegas

## Alcance

- Fecha: 2026-08-03.
- Rama: `refactor/codex-skills-v10-sap-query-separation`.
- Commit validado: `95cf7ec2676112f2e9a654355067f0834934d6fe`.
- Empresa piloto: `NuanSystem_DEMO`.
- Modalidad: consulta directa y exclusivamente de lectura contra SAP Business One Service Layer.

No se inició ningún worker, API o cliente WinForms. No se modificó ninguna base de datos, no se enviaron documentos ni datos de negocio hacia SAP y no se invocaron SAP DI API, HANA o SRI.

## Resultado runtime saneado

| Gate | Estado | Evidencia saneada |
|---|---:|---|
| Configuración local disponible | Aprobado | Conexión administrativa y clave de cifrado presentes; valores no mostrados |
| TLS SQL | Aprobado | `Encrypt=true`, `TrustServerCertificate=false` |
| TLS Service Layer | Aprobado | Validación de certificado activa; `IgnoreSslErrors=false` |
| Login Service Layer | Aprobado | Una solicitud de control y respuesta exitosa |
| Consulta de Bodegas | Aprobado | Dos solicitudes GET paginadas y exitosas |
| Paginación real | Aprobado | Se recorrieron dos páginas enlazadas por Service Layer |
| Mapeo | Aprobado | 24 registros mapeados; cero códigos o nombres vacíos |
| Estado activo | Aprobado | 24 activas y 0 inactivas en la lectura observada |
| Duplicados de código | Aprobado | Cero códigos duplicados en la lectura observada |
| Logout Service Layer | Aprobado | Una solicitud de control y respuesta exitosa |
| Escrituras de negocio hacia SAP | Aprobado | Cero solicitudes de escritura |
| Procesos NuanSystem | Aprobado | Cero antes y después de la validación |

Las solicitudes POST de `Login` y `Logout` son operaciones de control de sesión. La única operación de negocio fue `GET Warehouses`; no se ejecutaron POST, PUT, PATCH o DELETE sobre entidades SAP.

## Validación automatizada complementaria

Se ejecutaron cinco pruebas focalizadas de `SapServiceLayerQueryClientTests` y `SapServiceLayerWarehouseReaderTests`:

- paginación mediante `odata.nextLink`;
- rechazo de paginación hacia otro servidor;
- límite máximo de 100 páginas;
- mapeo de Bodegas y banderas de actividad;
- cierre de sesión aun cuando la consulta termina o falla.

Resultado: 5 superadas, 0 fallidas y 0 omitidas.

## Protección de información

La evidencia no contiene credenciales, cookies, URL del servidor, nombre de base SAP, códigos o nombres de bodegas, cadenas de conexión ni payloads completos. El arnés temporal solo conservó métricas agregadas y estados de éxito; fue retirado al finalizar.

## Conclusión

El flujo separado de consulta de Bodegas quedó validado para `NuanSystem_DEMO`: abre una sesión, consulta y pagina exclusivamente en lectura, mapea la respuesta y cierra la sesión sin efectuar escrituras. Esta aprobación no activa perfiles ni workers y no autoriza importación, actualización local ni envío de información a SAP.
