---
name: nuansystem-operational-usecase
description: Implement or review NuanSystem non-CRUD operational use cases that affect stock, money, prices, purchases, cash, documents, audit-sensitive records, or concurrency. Use for sales registration, payments, cash shifts, inventory movements, transfers, adjustments, purchase receipts, returns, cancellations, promotion/price calculation, or any flow requiring transaction boundaries and domain rules.
---

# NuanSystem Operational Use Case

## Workflow

1. Read `docs/ARQUITECTURA-COMERCIAL.md`.
2. Name the use case as a business action, not CRUD: `RegisterSale`, `TransferStock`, `CloseCashShift`, `CalculateSalePrice`.
3. Identify affected domains: inventory, sales, purchasing, cash, pricing, documents, audit.
4. Identify required capabilities using `$nuansystem-business-capabilities`.
5. Put pure rules in `Domain`; orchestrate dependencies in Application handlers.
6. Use `ITransactionRunner` for multi-write tenant operations.
7. Persist through repositories/stored procedures using `$nuansystem-sql-standards`.
8. Register audit entries in the same transaction when they describe the operation being committed.
9. Return purpose-built DTOs for the operation; do not reuse CRUD detail DTOs when the workflow needs optimized data.

## Required Analysis

For every operational use case, define:

- Inputs and actor.
- Company capabilities that affect behavior.
- State transitions.
- Transaction boundary.
- Stock impact.
- Money/cash impact.
- Document impact.
- Audit impact.
- Concurrency risk.
- Failure behavior.

## References

- Load `references/operational-checklist.md` before implementing a process that is not simple maintenance CRUD.
