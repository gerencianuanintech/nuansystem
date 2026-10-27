# Security Endpoint Nomenclature Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Consolidate security endpoints under `Endpoints/Seguridad` and make `/api/security` the sole HTTP prefix for authentication, user, role, and access routes.

**Architecture:** This is an API transport refactor. Endpoint classes move to one namespace, three class/registration names become security-specific, obsolete routes are removed, and all first-party consumers are updated without changing Application, Domain, Persistence, or database behavior.

**Tech Stack:** C# 14, ASP.NET Core Minimal APIs on .NET 10, xUnit, NSubstitute, WinForms typed HTTP client.

**Spec:** `docs/superpowers/specs/2026-10-26-security-endpoint-nomenclature-design.md`

## Global Constraints

- Do not register aliases or redirects for `/api/auth/*` or `/api/users*`.
- Preserve endpoint verbs, bodies, responses, tags, rate limiting, permissions, form operations, audit identity, and cancellation propagation.
- Login remains anonymous; change-password remains authenticated.
- `/api/security/auth/*` must bypass company resolution.
- No changes to Application, Domain, Persistence, SQL, SAP, synchronization, or SRI.
- Preserve unrelated working-tree changes.

## Review Focus

- Login without `X-Company-Code` must reach authentication rather than return `CompanyHeaderMissing`; covered by Task 2 middleware tests.
- A user marked `MustChangePassword=true` must be able to call `/api/security/auth/change-password`; covered by Task 2 middleware tests.
- Old authentication and user routes must return no registered endpoint rather than silently remain available; covered by Task 1 route-contract tests.
- The WinForms login and password-change client must send only the new paths; covered by Task 2 client tests.
- Existing permissions and form-operation metadata must survive file and namespace moves; covered by Task 1 route-contract metadata assertions and the full suite in Task 4.

---

### Task 1: Establish the new route contract and reorganize endpoint classes

**Files:**
- Create: `tests/NuanSystem.Application.Tests/Features/Security/SecurityEndpointRouteContractTests.cs`
- Move: `src/Backend/NuanSystem.Api/Endpoints/AuthEndpoints.cs` to `src/Backend/NuanSystem.Api/Endpoints/Seguridad/SecurityAuthEndpoints.cs`
- Move: `src/Backend/NuanSystem.Api/Endpoints/UserEndpoints.cs` to `src/Backend/NuanSystem.Api/Endpoints/Seguridad/SecurityUserEndpoints.cs`
- Move: `src/Backend/NuanSystem.Api/Endpoints/RoleEndpoints.cs` to `src/Backend/NuanSystem.Api/Endpoints/Seguridad/SecurityRoleEndpoints.cs`
- Move: the nine remaining `Security*Endpoints.cs` files listed in the spec into `src/Backend/NuanSystem.Api/Endpoints/Seguridad/`
- Delete: `src/Backend/NuanSystem.Api/Endpoints/Seguridad/.gitkeep`
- Modify: `src/Backend/NuanSystem.Api/Program.cs`

**Interfaces:**
- Produces: `SecurityAuthEndpoints.MapSecurityAuthEndpoints(IEndpointRouteBuilder)`, `SecurityUserEndpoints.MapSecurityUserEndpoints(IEndpointRouteBuilder)`, and `SecurityRoleEndpoints.MapSecurityRoleEndpoints(IEndpointRouteBuilder)` in namespace `NuanSystem.Api.Endpoints.Seguridad`.
- Produces: canonical route patterns from the approved spec; removed routes are not mapped.

- [ ] **Step 1: Write the failing route-contract test**

Create tests that map the security endpoint groups into a minimal `WebApplication`, inspect `RouteEndpoint.RoutePattern.RawText`, and assert:

- New authentication routes are present.
- `/api/security/users`, `/api/security/users/{id:int}`, `/api/security/users/roles`, `/api/security/roles`, and `/api/security/roles/{id:int}` remain present.
- Every removed route from the spec is absent.
- Login retains anonymous and rate-limit metadata; change-password retains authorization metadata.

- [ ] **Step 2: Run the targeted test and confirm the contract fails**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter FullyQualifiedName~SecurityEndpointRouteContractTests`

Expected: FAIL because authentication still uses `/api/auth/*` and legacy user routes remain registered.

- [ ] **Step 3: Move the twelve classes and align namespaces**

Use `namespace NuanSystem.Api.Endpoints.Seguridad;` in every moved file. Rename only the Auth, User, and Role files/classes/registration methods specified in the Interfaces block; keep all other class and method names unchanged.

- [ ] **Step 4: Replace and remove routes**

In `SecurityAuthEndpoints`, map the two approved `/api/security/auth/*` routes. In `SecurityUserEndpoints`, delete the three legacy mappings while preserving the existing `/api/security/users*` implementations and their `RequireFormOperation` authorization.

- [ ] **Step 5: Update API composition**

Add `using NuanSystem.Api.Endpoints.Seguridad;` to `Program.cs` and replace the three renamed registration calls. Keep every security endpoint group registered exactly once.

- [ ] **Step 6: Run the targeted test**

Run the Task 1 test command.

Expected: PASS.

- [ ] **Step 7: Commit the endpoint contract refactor**

Commit message: `refactor(api): consolidate security endpoint routes`

### Task 2: Update runtime consumers and middleware boundaries

**Files:**
- Modify: `src/Frontend/NuanSystem.WinForms.Services/Authentication/AuthenticationClient.cs`
- Modify: `src/Backend/NuanSystem.Api/Middleware/CompanyContextMiddleware.cs`
- Modify: `src/Backend/NuanSystem.Api/Middleware/RequiredPasswordChangeMiddleware.cs`
- Modify: `src/Backend/NuanSystem.Api/Extensions/ServiceCollectionExtensions.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/Security/SecurityAuthenticationRouteConsumerTests.cs`

**Interfaces:**
- Consumes: the `/api/security/auth/login` and `/api/security/auth/change-password` routes from Task 1.
- Produces: WinForms requests and middleware skip rules using only those canonical routes.

- [ ] **Step 1: Write failing consumer and middleware tests**

Add tests that assert:

- `AuthenticationClient.LoginAsync` posts to `/api/security/auth/login`.
- `AuthenticationClient.ChangePasswordAsync` posts to `/api/security/auth/change-password`.
- `CompanyContextMiddleware` invokes `next` for both new authentication paths without resolving a company.
- `RequiredPasswordChangeMiddleware` invokes `next` for both new authentication paths when the claim requires a password change.
- The equivalent removed authentication paths are not treated as authentication bypasses.

- [ ] **Step 2: Run the targeted tests and confirm they fail**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter FullyQualifiedName~SecurityAuthenticationRouteConsumerTests`

Expected: FAIL because runtime consumers still contain `/api/auth/*`.

- [ ] **Step 3: Update runtime paths**

Replace authentication paths in `AuthenticationClient`, both middleware skip functions, and the Swagger bearer description. Use the exact canonical paths from the spec.

- [ ] **Step 4: Run Task 2 tests**

Run the Task 2 test command.

Expected: PASS.

- [ ] **Step 5: Commit runtime consumer updates**

Commit message: `refactor(security): adopt canonical authentication routes`

### Task 3: Update maintained route documentation

**Files:**
- Modify: `docs/FASE-2-MULTIEMPRESA.md`
- Modify: `docs/FASE-3-SEGURIDAD.md`
- Modify: `docs/FASE-11-FRONTEND-WINFORMS.md`
- Modify: `docs/FASE-14-CONFIGURACION-PARAMETROS.md`
- Modify: `docs/FASE-16-ADMINISTRACION-USUARIOS.md`
- Modify: `docs/FASE-18-AUTORIZACION-POR-PERMISOS.md`
- Modify: `docs/REVIEW-NUANSYSTEM-CODEX.md`

**Interfaces:**
- Consumes: the route contract finalized in Tasks 1 and 2.
- Produces: documentation that names only canonical security routes and no longer instructs maintainers to preserve legacy user/authentication routes.

- [ ] **Step 1: Replace obsolete route references and compatibility guidance**

Update each listed document to use `/api/security/auth/*` and `/api/security/users*`. Remove statements that require preserving `/api/auth/*` or `/api/users*`; do not rewrite unrelated historical content.

- [ ] **Step 2: Verify documentation route consistency**

Run: `rg -n '/api/auth|/api/users(?:/|\b)' docs src tests -g '*.md' -g '*.cs'`

Expected: no executable-code or maintained-document references to removed routes. References inside this design and implementation plan are permitted because they document deliberate removal.

- [ ] **Step 3: Commit documentation updates**

Commit message: `docs(security): document canonical endpoint nomenclature`

### Task 4: Validate the complete refactor

**Files:**
- Verify only; modify solely to fix failures caused by Tasks 1–3.

**Interfaces:**
- Consumes: all preceding task outputs.
- Produces: build, test, and repository-search evidence for the final handoff.

- [ ] **Step 1: Verify removed symbols and paths**

Run searches for `AuthEndpoints`, `UserEndpoints`, `RoleEndpoints`, `MapAuthEndpoints`, `MapUserEndpoints`, `MapRoleEndpoints`, `/api/auth`, and `/api/users` across executable code. Confirm only the new `Security*` names and `/api/security/*` routes remain.

- [ ] **Step 2: Build the solution**

Run: `dotnet build nuanapi_umco.sln`

Expected: exit code 0 with no compilation errors.

- [ ] **Step 3: Run the complete automated suite**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --no-build`

Expected: zero failed tests.

- [ ] **Step 4: Inspect the final diff**

Confirm there are no changes to Application, Domain, Persistence, SQL, SAP, synchronization, or SRI, and no unrelated user files were staged.

- [ ] **Step 5: Commit any validation-driven corrections**

If required, use commit message: `test(security): verify endpoint nomenclature migration`
