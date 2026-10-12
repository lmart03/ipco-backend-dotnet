# Patients API

Base path: `/api/v1/patients`. Scalar renders the OpenAPI contract at `/scalar/v1` in Development.

## Operations

| Method | Path | Success | Errors |
|---|---|---:|---|
| GET | `/api/v1/patients?search=&page=1&pageSize=20` | 200 | 400 |
| GET | `/api/v1/patients/{id}` | 200 | 404 |
| POST | `/api/v1/patients` | 201 | 400, 409 |
| PUT | `/api/v1/patients/{id}` | 204 | 400, 404, 409 |
| DELETE | `/api/v1/patients/{id}` | 204 | 404 |

All errors use RFC 9457 Problem Details and include stable `code` and W3C `traceId` extensions.

`POST` returns the shared JSON contract `{ "id": "<uuid>" }` and the canonical patient URL in the `Location` header. `PUT` and `DELETE` return an empty response body. The collection response uses the shared `PageResult<PatientResponse>` envelope; patient search and ordering remain owned by this module.

## Preserved Java behavior

- Names and phone are required.
- Phone, email, document, and surrounding whitespace are normalized.
- Document type and number must be both present or both absent.
- Birth date is between 1900-01-01 and today (UTC).
- Active patients cannot share document type and number.
- Search covers names, email, document, and phone digits.
- Deletion is logical and releases the unique active-document constraint.
- Results are ordered by last name, first name, and UUID.

Catalog-category validation and catalog labels remain the responsibility of the future catalog/reference-data module. Until that module is connected, catalog UUIDs are stored as opaque references and the generic document format is enforced.

## Database

The module uses PostgreSQL through EF Core/Npgsql. Restore the local tool and apply migrations:

```powershell
dotnet tool restore
dotnet ef database update --project src/Modules/Ipco.Modules.Patients --startup-project src/Ipco.Api --context PatientsDbContext
```
