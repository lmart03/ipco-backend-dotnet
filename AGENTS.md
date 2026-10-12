# IPCO Backend

This repository is a .NET 10 modular monolith migrated deliberately from the Java backend at `C:\dev\personal\ipco-backend`. Preserve business behavior; do not translate Spring/JPA patterns mechanically.

## Non-negotiable boundaries

- Organize business code by module and use case. Do not add global `Services`, `Helpers`, or generic CRUD folders.
- Domain must not reference ASP.NET Core, EF Core, storage, email, or HTTP response types.
- Application may depend on Domain; Infrastructure implements outbound ports; API is the composition root.
- Do not introduce a generic `IRepository<TEntity>`. Define repositories for aggregate roots with business-shaped operations.
- Child entities are persisted through their aggregate root unless a documented constraint requires otherwise.
- Use `TimeProvider`; do not call `DateTime.Now` or `DateTime.UtcNow` in business code.
- New identifiers are UUIDv7 (`Guid`) following RFC 9562 and are produced through `IUuidGenerator`.
- Model business failures independently from HTTP and translate them at the API boundary to RFC 9457 `ProblemDetails`.
- Error codes are stable machine-readable values in `scope.condition` form; human-readable descriptions are not contracts.
- Never expose exception messages, stack traces, SQL, paths, secrets, or implementation details in an HTTP error.
- Correlate failures using the W3C Trace Context trace ID from `Activity.Current`; do not invent a parallel request-correlation format.
- Every behavior change requires proportionate tests.
- Every endpoint must declare OpenAPI summary, description, request contract, success response, and RFC 9457 error responses so Scalar remains complete.
- Every CRUD module exposes a paginated collection `GET` and a `GET` by UUID. Collection paging uses the shared `PageRequest` and `PageResult<T>` contracts; filters and allowed sort fields remain module-specific.
- Successful creates return `201 Created`, a resource `Location` header, and the shared `{ "id": "<uuid>" }` response. Successful full updates and deletes return `204 No Content` with no response body.
- New modules require domain, application, architecture, persistence, and HTTP tests; PostgreSQL behavior must use Testcontainers rather than EF InMemory.
- Do not migrate or scaffold a business feature unless the user explicitly selects that feature.
- Run `dotnet test Ipco.slnx` before finishing when the environment permits.

For architecture or migration work, load `.agents/skills/ipco-backend-architecture/SKILL.md`.
