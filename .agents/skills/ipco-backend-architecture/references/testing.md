# Testing

- Domain tests cover invariants, state transitions, and value objects without mocking entities.
- Application tests cover orchestration and substitute only outbound ports.
- Integration tests cover HTTP contracts, EF mappings, PostgreSQL constraints, and storage adapters.
- Architecture tests enforce dependency direction and module boundaries.

Every migrated Java rule needs a corresponding .NET test before Java stops being the reference. Use Testcontainers for PostgreSQL and S3 storage; do not use EF InMemory for relational constraint tests. Inject `TimeProvider` for deterministic time.

Validate HTTP request shape with FluentValidation at the API boundary. Keep domain invariants in the domain even when an equivalent boundary rule exists; boundary validation improves client feedback, while domain validation protects every caller.

For every CRUD module, HTTP contract tests cover the collection `GET`, `GET` by UUID, `POST` status/body/`Location`, bodyless `PUT` and `DELETE` responses, validation failures, missing resources, and relevant conflicts.
