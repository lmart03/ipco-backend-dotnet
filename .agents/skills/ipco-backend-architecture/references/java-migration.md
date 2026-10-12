# Java to .NET migration

Use `C:\dev\personal\ipco-backend` as the behavioral source.

Preserve domain rules, PostgreSQL constraints, concurrency, required API compatibility, private object storage, immutable documents, and valuable tests.

- Spring Data `Repository<T, UUID>` becomes aggregate-specific ports, not a generic repository.
- JPA annotations become EF fluent configuration in Infrastructure.
- Large Spring services become focused handlers.
- Application must not import presentation DTOs.
- Cross-feature entity references become IDs and read ports.
- `Clock` becomes `TimeProvider`.
- HTTP-shaped domain exceptions become domain errors translated by API.

Migrate one vertical slice at a time: characterize Java behavior, write .NET tests, implement domain/use case, add adapters, expose endpoint.
