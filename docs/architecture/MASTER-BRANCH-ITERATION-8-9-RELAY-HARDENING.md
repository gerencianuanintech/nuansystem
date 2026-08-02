# Iteración 8.9 — Aislamiento del relay por entidad

## Resultado esperado

El relay tenant a Master solo puede reclamar eventos y liberar leases de las
entidades incluidas explícitamente en `EnabledEntityAppliers`. Deshabilitar una
entidad deja de ser únicamente una defensa en el aplicador: también impide que
el relay altere sus filas de `LocalOutbox`.

## Evidencia y problema

El claim de `SyncOutbox` en Master ya estaba limitado por las entidades
habilitadas. En cambio, `LocalSyncOutboxRelay` descubría tenants y ejecutaba los
procedimientos de claim y liberación de leases sin un filtro equivalente. Una
prueba o activación acotada podía cambiar el estado técnico de eventos ajenos,
aunque después no existiera un aplicador habilitado para procesarlos.

## Decisión

- `EnabledEntityAppliers` es la allowlist única para el relay local y el claim
  central.
- El relay normaliza, elimina duplicados y transmite la allowlist a ambos
  procedimientos tenant.
- El repositorio repite la validación y no abre una conexión tenant cuando la
  lista está vacía.
- La migración tenant `164` filtra tanto el claim como la liberación de leases.
- Una lista vacía es un no-op. Los parámetros SQL conservan `[]` como valor
  predeterminado para que consumidores anteriores fallen de forma cerrada.
- La migración no activa workers, perfiles, rutas ni entidades.

## Árbol de decisión

```text
Relay deshabilitado -> no consultar tenants
Relay habilitado + allowlist vacía -> no consultar ni mutar
Entidad fuera de allowlist -> conservar LocalOutbox intacto
Entidad dentro de allowlist -> permitir release/claim según estado y lease
Evento promovido a Master -> el claim central vuelve a aplicar la misma allowlist
```

## Quality gates

- Interfaz, relay, repositorio y SQL comparten el mismo filtro.
- Lista vacía no descubre tenants ni abre conexiones.
- Claim y liberación de leases excluyen entidades no habilitadas.
- Normalización determinista, sin duplicados y sin nombres vacíos.
- Script tenant idempotente, forward-only y registrado después de `162`.
- Build y pruebas completas sin fallos.
- SQL real y runtime requieren autorización independiente; hasta entonces los
  workers permanecen deshabilitados.

## Validación estática — 2026-08-01

- Pruebas dirigidas del aislamiento: 5 aprobadas.
- Build completo: 0 errores y 0 advertencias.
- Suite completa: 752 aprobadas, 5 diagnósticas omitidas y 0 fallidas.
- `git diff --check`: aprobado.
- Skill `nuansystem-master-branch-sync`: validación estructural aprobada.
- SQL, API y workers: no ejecutados.
