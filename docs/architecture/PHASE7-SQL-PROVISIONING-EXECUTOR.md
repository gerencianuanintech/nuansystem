# Ejecutor controlado del laboratorio de fase 7

Decisión y plan de implementación, 2026-10-25. Alcance autorizado: código y pruebas offline; ninguna apertura de conexión, ejecución SQL o SAP durante el desarrollo.

Descubrimiento: el generador en `tools/phase7-sql` ya produce los scripts protegidos y un manifiesto con hashes; el monitor y los lectores de la suite reciben conexiones previamente abiertas. No existe un ejecutor de aprovisionamiento. Se reutilizan esos scripts y su protocolo; no se agrega lógica al API, Domain, Application, Worker, SAP ni migraciones de producción.

El ejecutor será una herramienta PowerShell separada: revisión por defecto, ejecución futura solo con activación explícita, aprobación vinculada al hash externo del manifiesto, RunId, ventana, identidad administrativa y límites de tiempo. Recibe una conexión Microsoft.Data.SqlClient ya abierta en master; nunca resuelve credenciales ni abre conexiones. La ejecución acepta la propiedad y cierra esa sesión, incluso al fallar. No conecta durante descubrimiento ni carga de módulos.

Se verifica todo el paquete antes de contactar al transporte: rutas, enlaces, duplicados JSON, inventario cerrado, tamaños y hashes. Se conservan en memoria los bytes verificados para evitar ejecutar una relectura alterada. Separación GO con análisis de cadenas y comentarios; rechazo de GO repetido y directivas de cliente. Secuencia fija 00/10/20A/20B/30A/30B/40A/40B/50, cambio explícito de base, sin reintentos, sin rollback global ni DROP. Los inventarios y el monitor mantienen sus procedimientos independientes.

Los cinco secretos se reciben como SecureString, nunca por JSON o archivos. Se inyectan mediante parámetros solo antes de la etapa 50 y se limpian en finally junto con contextos de autorización. Un diario local nuevo registra intención y resultado por lote, sin SQL ni errores del proveedor; cualquier fallo de auditoría detiene la ejecución. Una cancelación, expiración o timeout detiene los lotes siguientes. La sesión se cierra aun cuando no sea posible limpiar contextos. La limpieza no borra bases u objetos parcialmente creados.

Plan:
1. Escribir pruebas offline con transporte simulado para aprobación, integridad, orden, fallos, límites, secretos y auditoría; observar rojo.
2. Implementar lector/plan inmutable, núcleo con transporte inyectado y adaptador SqlClient de sesión abierta.
3. Añadir entrada de revisión/ejecución explícita y guía operativa; no ejecutar la rama SQL.
4. Validar las pruebas offline y regresión del generador; revisión independiente final y corrección de hallazgos importantes.

Criterio: el transporte simulado nunca acredita aprovisionamiento real ni cierre de fase 7. La aprobación de archivos no equivale a autorización para ejecutar. Sin datos reales de servidor, permisos, esquema e inventarios no se afirma que SQL funciona.

Resultado: implementados lector/verificador, núcleo privado con transporte simulado y adaptador SqlClient sin Open, entrada de revisión por defecto y guía operativa. Rojo observado: módulo inicialmente ausente; prueba de secretos duplicados falló antes de añadir su comparación previa; las regresiones de política de errores y cambio de base fallaron al faltar sus funciones. Verde: 68 controles del ejecutor y 99 del generador. Una revisión independiente final; dos hallazgos importantes corregidos en una pasada. El lector de identidad usa ReadAsync con el mismo token. No se modificaron C#, migraciones ni integración SAP y no se ejecutaron conexiones, SQL o SAP. El cierre de fase 7 sigue pendiente de validación real autorizada.
