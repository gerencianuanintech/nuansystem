# Implemented SAP contracts

Use this map to accelerate discovery; open current source and consumers before relying on behavior.

## Transport

- `src/Backend/NuanSystem.Domain/Tenancy/SapIntegrationMode.cs`
- `src/Backend/NuanSystem.SapIntegration/Abstractions/ISapClient.cs`
- `src/Backend/NuanSystem.SapIntegration/Abstractions/ISapClientFactory.cs`
- `src/Backend/NuanSystem.SapIntegration/Clients/SapClientFactory.cs`
- `src/Backend/NuanSystem.SapIntegration/DependencyInjection/SapIntegrationServiceRegistration.cs`

## Readers/senders

- `Suppliers/SapSupplierReader.cs`
- `Warehouses/SapServiceLayerWarehouseReader.cs`
- `Items/SapServiceLayerItemReader.cs`
- `ServiceLayer/SapServiceLayerPurchaseOrderReader.cs`
- `Documents/SapDocumentSender.cs`
- `Clients/ServiceLayer/SapServiceLayerClient.cs`
- `Clients/DiApi/SapDiApiClient.cs`
- `Hana/SapHanaQueryClient.cs`

## Application/API/database

- `src/Backend/NuanSystem.Application/Features/SapSync`
- `src/Backend/NuanSystem.Api/Endpoints/SapEndpoints.cs`
- SQL `048`, `049`, `050`, `089`, `095`, `096`, and `101`.
