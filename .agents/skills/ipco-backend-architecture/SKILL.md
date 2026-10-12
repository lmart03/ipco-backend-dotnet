---
name: ipco-backend-architecture
description: Design, implement, migrate, or review IPCO backend features in its .NET modular-monolith and hexagonal architecture. Use for domain modeling, Java migration, module boundaries, adapters, endpoints, and tests.
---

# IPCO backend architecture

Preserve observable Java behavior while improving module boundaries and object-oriented modeling. Treat Java as a behavioral reference, not a class-by-class template.

Do not migrate or scaffold business-domain code until the user explicitly selects the feature or migration slice.

1. Identify the owning module and aggregate.
2. Read the relevant reference: [architecture](references/architecture.md), [testing](references/testing.md), or [Java migration](references/java-migration.md).
3. Implement a vertical slice inside the module.
4. Keep adapters behind outbound ports and map HTTP contracts at the endpoint.
5. Add tests for domain behavior and affected boundaries.
6. Run focused tests, then the full suite when practical.

Avoid speculative abstractions. Share a building block only when multiple modules need the same stable semantics.
