# Module Boundaries

## Administrative CRUD

Use CRUD patterns for stable catalogs and setup:

- Brands, groups, families, units, taxes.
- Customers and suppliers.
- Users, roles, menus, forms, fields.
- Configuration tables.

## Operational Processes

Use case-based commands for processes that affect business state:

- Register sale.
- Register payment.
- Close cash shift.
- Transfer stock.
- Adjust inventory.
- Register purchase receipt.
- Cancel or return document.
- Calculate price or promotion.

## Domain Rules

Place pure business rules in `Domain`:

- Stock policy.
- Document state transitions.
- Totals, taxes, rounding.
- Promotion eligibility.
- Cash closing constraints.

## Configuration

Use company capabilities for variation:

- Lots, expiration dates, serials, variants.
- Negative stock.
- Multiple warehouses.
- Multiple units.
- Multiple barcodes.
- Sale by weight.
- Credit sales.
- Electronic invoicing.
