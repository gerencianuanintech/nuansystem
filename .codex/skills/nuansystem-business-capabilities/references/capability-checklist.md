# Capability Checklist

Before hard-coding a rule, ask:

- Could a supermarket need this differently from a hardware store?
- Could one company allow this and another forbid it?
- Is this about stock, pricing, cash, tax, documents, integrations, or item structure?
- Should this be enabled by a business preset?
- Does the UI need to hide/show fields based on this capability?
- Does SQL need default seed values?

Implementation checklist:

- Add a stable key.
- Add default value.
- Add validation for accepted value type.
- Load it through Application abstractions.
- Use it in Domain/Application rules.
- Document the behavior in `docs/ARQUITECTURA-COMERCIAL.md` when it introduces a new class of behavior.
