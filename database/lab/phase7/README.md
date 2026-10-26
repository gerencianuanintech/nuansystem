# Laboratorio SQL de fase 7: preparación offline

Estos archivos no son migraciones de producción. El generador prepara un paquete nuevo en disco; no abre conexiones ni ejecuta SQL o SAP. Requiere PowerShell 7.5 o posterior para las pruebas offline.

```powershell
& tools/phase7-sql/New-Phase7SqlProvisioningBundle.ps1 `
  -SpecPath database/lab/phase7/provisioning-spec.example.json `
  -OutputDirectory artifacts/phase7-lab-review
& tools/phase7-sql/Test-Phase7SqlProvisioningBundle.ps1
```

La carpeta de salida debe ser nueva. El ejemplo usa una ventana caducada y una autorización ficticia: sirve para revisión, nunca para ejecutar. No agregar credenciales al JSON.

El paquete contiene dos bases por RunId, cinco identidades independientes, scripts de esquema, semilla sintética, fallos controlados, monitor e inventarios; conserva los 16 originales y registra sus hashes. Excluye las tres bajas legacy iniciales de 024 y el script mixto Master/tenant 063. No modifica esos originales.

Los fixtures de `review-example` permiten comprobar offline que los triggers generados son equivalentes al renderizador C# de la suite. No son evidencia de una instalación real.

Plan, permisos, restricciones y criterios de cierre: [aprovisionamiento](../../../docs/operations/BACKEND-SOLID-PHASE-7-SQL-PROVISIONING.md).

## Ficha de configuración y checklist

Completar una copia de [laboratory-preparation.template.json](laboratory-preparation.template.json) para planificar las dos corridas, y una copia de [provisioning-spec.template.json](provisioning-spec.template.json) por RunId. La ficha general no es configuración ejecutable; la especificación de provisión conserva los 14 campos del generador pero comienza incompleta y se rechaza hasta completar datos reales aprobados. No agregar secretos. El ejemplo anterior permanece ficticio y caducado.

Datos, permisos, responsables, autorizaciones por etapa y criterios de cierre: [checklist del laboratorio](../../../docs/operations/BACKEND-SOLID-PHASE-7-SQL-LAB-CHECKLIST.md). Preparar o marcar una casilla no autoriza ejecución SQL/SAP ni acredita autenticidad.
