# Architecture

IPCO is a modular monolith. Each module owns domain, use cases, adapters, and endpoints. The API host only composes modules.

`Api -> Modules -> Application -> Domain`, while Infrastructure implements ports from Application/Domain.

Modules: Identity, Organization, Scheduling, Clinical, Inventory, Sales, and CRM. Do not create a catch-all `Masters`; reference data belongs to its business owner.

- Aggregates protect invariants and expose behavior, not public setters.
- Cross-aggregate and cross-module references use IDs, not EF navigations.
- Use value objects for normalized or repeatedly validated concepts.
- Issued documents, stock movements, and financial records use explicit states rather than generic soft deletion.
- EF configurations live in Infrastructure.
- Repositories exist only for aggregate roots and expose business-shaped operations.
- Auditing, soft delete, and concurrency are separate capabilities.
- Building blocks contain no module entities, DTOs, repositories, or module-specific helpers.
- Persist identifiers as UUIDs and create new values through `IUuidGenerator`, which emits RFC 9562 UUIDv7 values.
- Return domain/application failures as `Result`; map them to RFC 9457 Problem Details only in the API layer.
- Keep error codes stable and machine-readable (`scope.condition`). Never couple domain errors to HTTP status codes.
- Correlate HTTP failures with `Activity.Current.TraceId` using W3C Trace Context; do not generate an unrelated correlation ID.

## HTTP CRUD conventions

- A CRUD module owns its endpoint mappings in its `Presentation` folder; the API host only composes the module.
- Expose both a paginated collection `GET` and a `GET /{id:guid}` for every CRUD resource.
- Use `PageRequest` and `PageResult<T>` for the stable paging envelope. Search filters, sortable fields, default ordering, and query semantics belong to the owning module and must not be hidden in a generic repository.
- A successful `POST` returns `201 Created`, the canonical resource URL in `Location`, and `CreatedResponse` containing the generated UUID.
- A successful full `PUT` and `DELETE` return `204 No Content` without a body.
- Document every success and RFC 9457 failure response in OpenAPI so Scalar renders the real contract.
