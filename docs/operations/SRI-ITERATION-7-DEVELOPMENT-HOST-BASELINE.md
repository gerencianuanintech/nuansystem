# Baseline del host de desarrollo — Iteración 7

## Propósito

Registrar el computador autorizado por el propietario para desarrollo y ensayos
locales de NuanSystem. Este baseline no lo convierte en host productivo y no
autoriza instalación SCM, inicio del worker, procesamiento documental ni
llamadas al SRI.

## Alcance autorizado

- compilación, pruebas y publicación local;
- validación de artefactos inmutables;
- preflight de solo lectura;
- ensayos futuros deshabilitados cuando exista autorización separada;
- evidencia saneada sin conexiones, tokens, claves, XML ni secretos.

## Inventario observado

Snapshot de solo lectura: 2026-07-25.

| Componente | Evidencia |
|---|---|
| Equipo | `PROYECTOS` |
| Sistema operativo | Microsoft Windows 11 Pro, versión `10.0.26200`, x64 |
| Zona horaria | `SA Pacific Standard Time` |
| CPU | Intel Core i9-14900HX |
| Procesadores lógicos | 32 |
| Memoria física | 31.7 GiB |
| Disco `C:` | 951.6 GiB; 502.5 GiB libres |
| Disco `E:` | 931.5 GiB; 859.1 GiB libres |
| Dominio | `WORKGROUP`; no unido a Active Directory |
| SDK .NET | `10.0.302` |
| Certificado público local | Una copia `CN=localhost` en `LocalMachine\Root`, sin clave privada |

No se versionaron huellas de certificados, identidad de usuario, rutas de
secrets, conexiones ni valores protegidos.

## Artefactos disponibles

| Release | Proyectos | Runtime | Modalidad |
|---|---:|---|---|
| `7.1.0-dotnet10-pilot1+9275f7c2` | 5 | `win-x64` | framework-dependent |
| `7.1.0-dotnet10-pilot2+664c48a4` | 5 | `win-x64` | framework-dependent |

Ambas releases conservan sus manifests y hashes validados en Fase 7.3.

## Estado de procesos

Durante el snapshot:

- `NuanSystem.SyncWorker`: 0;
- `NuanSystem.MasterBranchSyncWorker`: 0;
- `NuanSystem.SriWorker`: 0;
- `NuanSystem.WinForms`: 0.

No se instaló ni inició ningún proceso para obtener esta evidencia.

## Evaluación de aptitud

### Apto para desarrollo

- supera el baseline local de CPU, RAM y disco;
- dispone de .NET 10 y DevExpress previamente validados;
- puede compilar, probar y publicar artefactos `win-x64`;
- conserva certificados y configuración local exclusivamente para ensayos
  controlados;
- permite repetir preflights sin tocar infraestructura productiva.

### No apto como evidencia productiva

- Windows 11 Pro es una estación interactiva, no el Windows Server dedicado
  propuesto;
- pertenece a `WORKGROUP`, no a dominio/Active Directory;
- no puede acreditar gMSA productiva;
- usa identidad, trust y configuración de desarrollo;
- no demuestra patch ownership, EDR, egress, ACL, vault, alertas ni soporte
  productivos;
- no demuestra restore coordinado ni RPO/RTO;
- no autoriza canario DEMO.

## Regla de uso

```text
Host PROYECTOS
  -> desarrollo / build / tests / publish / preflight
  -X-> aprobación de D7-02 productivo
  -X-> aprobación de D7-03 gMSA
  -X-> instalación permanente
  -X-> habilitación de SriWorker
  -X-> llamada SRI
```

La instalación temporal deshabilitada autorizada posteriormente quedó
registrada en
[SRI-ITERATION-7-DEVELOPMENT-WORKER-REHEARSAL.md](SRI-ITERATION-7-DEVELOPMENT-WORKER-REHEARSAL.md).
El ensayo no reutilizó recursos residuales de Iteración 6 y restauró el
baseline material.

## Resultado

**Validado como host de desarrollo.**

**D7-02 y D7-03 productivas permanecen bloqueadas.**
