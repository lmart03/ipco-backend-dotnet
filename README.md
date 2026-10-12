# IPCO Backend .NET

Backend de IPCO construido con .NET 10 como monolito modular, organizado por módulos de negocio y límites hexagonales. La primera vertical implementada es el CRUD de pacientes.

## Requisitos

- .NET SDK 10.0.401 o una revisión compatible.
- PostgreSQL.
- Docker para las pruebas de integración con Testcontainers.

## Configuración local

La cadena de conexión no se versiona. Defínela mediante una variable de entorno antes de iniciar la API:

```powershell
$env:ConnectionStrings__IpcoDatabase = "Host=localhost;Port=5432;Database=ipco;Username=postgres;Password=tu-clave"
```

Para conservarla únicamente durante el desarrollo también puedes usar .NET User Secrets:

```powershell
dotnet user-secrets init --project src/Ipco.Api
dotnet user-secrets set "ConnectionStrings:IpcoDatabase" "Host=localhost;Port=5432;Database=ipco;Username=postgres;Password=tu-clave" --project src/Ipco.Api
```

## Ejecución

```powershell
dotnet restore Ipco.slnx
dotnet tool restore
dotnet ef database update --project src/Modules/Ipco.Modules.Patients --startup-project src/Ipco.Api --context PatientsDbContext
dotnet run --project src/Ipco.Api/Ipco.Api.csproj
```

En Development:

- Scalar: `https://localhost:7007/scalar/v1`
- OpenAPI: `https://localhost:7007/openapi/v1.json`
- Health check: `https://localhost:7007/health/live`

## Calidad

```powershell
dotnet build Ipco.slnx --no-restore
dotnet test Ipco.slnx --no-build
```

La suite contiene pruebas de dominio, aplicación, arquitectura e integración HTTP/PostgreSQL. Las pruebas relacionales utilizan PostgreSQL real mediante Testcontainers.

## Arquitectura

- `src/BuildingBlocks`: contratos técnicos estables compartidos.
- `src/Modules`: módulos de negocio dueños de dominio, aplicación, infraestructura y presentación.
- `src/Ipco.Api`: composition root y host HTTP.
- `tests`: pruebas separadas por responsabilidad.
- `docs`: contratos y decisiones específicas de cada módulo.

Los errores HTTP usan RFC 9457 Problem Details y el identificador W3C Trace Context. Los identificadores nuevos son UUIDv7 conforme a RFC 9562. Las reglas para colaboradores y agentes están en `AGENTS.md` y `.agents/skills/ipco-backend-architecture/`.

Consulta el contrato y comportamiento del módulo de pacientes en `docs/patients-api.md`.
