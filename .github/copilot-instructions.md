# Copilot Instructions

## Build & Test

```sh
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

There is a single project: `src/Microsoft.Exchange.WebServices.Core.csproj` targeting `net10.0`.  
There are no test projects in the repository — `dotnet test` will find nothing to run.

## Architecture

This is a .NET port of the Exchange Web Services (EWS) Managed API. The entire library lives under the single namespace `Microsoft.Exchange.WebServices.Data` (Autodiscover uses `Microsoft.Exchange.WebServices.Autodiscover`).

### Core layers

- **`ExchangeService`** (`src/Core/ExchangeService.cs`) — the main public entry point. Inherits `ExchangeServiceBase`. All EWS operations are methods on this class.
- **`ServiceObject`** (`src/Core/ServiceObjects/ServiceObject.cs`) — abstract base for all items (`Item`, `EmailMessage`, `Appointment`, `Contact`, etc.) and folders. Holds a `PropertyBag`.
- **`PropertyBag`** (`src/Core/PropertyBag.cs`) — keyed on `PropertyDefinition` objects; tracks loaded, modified, added, and deleted properties per service object.
- **`ServiceObjectSchema`** (`src/Core/ServiceObjects/Schemas/ServiceObjectSchema.cs`) — registry of all schema types. Each service object subclass has a companion schema (e.g. `EmailMessageSchema`, `AppointmentSchema`) with static `PropertyDefinition` fields.
- **Requests** (`src/Core/Requests/`) — one `*Request` class per EWS operation, all `internal`. They inherit from `ServiceRequestBase`, which handles XML serialization and HTTP dispatch.
- **`PropertyDefinition`** / `PropertyDefinitions/` — typed property descriptors that carry the XML element name, `PropertyDefinitionFlags`, and minimum `ExchangeVersion`.

### Request/response flow

`ExchangeService` method → instantiates a `*Request` → `ServiceRequestBase` serializes to SOAP XML → sends via `EwsHttpWebRequest` → response XML deserialized into response objects → properties loaded into `PropertyBag` of the owning service object.

### XML constants

- `XmlElementNames` — all EWS XML element name string constants.
- `XmlAttributeNames` — all EWS XML attribute name string constants.
- Never hard-code EWS XML strings; reference these static constants instead.

## Key Conventions

### All HTTP-bound public API methods are async

Every public method that triggers a network request returns `Task<T>` and accepts an optional `CancellationToken`. The legacy `Begin`/`End` async pattern has been removed.

### Service object ↔ XML mapping via attributes

Service object classes use `[ServiceObjectDefinition(XmlElementNames.XYZ)]` to declare their EWS XML element name. The `[Attachable]` attribute marks items that can be attached. These attributes are resolved at runtime via reflection.

### New schema type registration is required

When adding a new `ServiceObject` subclass with its own schema, the schema type **must** be added to the `allSchemaTypes` list inside `ServiceObjectSchema` (around line 53). Forgetting this causes a debug-mode assertion failure.

### Property definitions are static fields on Schema classes

Properties exposed by a service object are defined as `static readonly PropertyDefinition` fields on its schema class (e.g. `ItemSchema.Subject`). They are version-gated via `ExchangeVersion` and flagged with `PropertyDefinitionFlags`.

### `LazyMember<T>` for deferred initialization

Shared, expensive-to-initialize members (like the `allSchemaTypes` list) use `LazyMember<T>` with a double-checked lock. Prefer this pattern for new static shared state.

### `EwsUtilities.Assert` instead of `Debug.Assert`

Use `EwsUtilities.Assert(condition, callerName, message)` throughout the codebase rather than `System.Diagnostics.Debug.Assert`.

### License header required on all source files

Every `.cs` file must begin with the MIT license header block (see any existing file for the template).

### `ExchangeVersion` gates availability

Features introduced in a specific Exchange version are gated by `ExchangeVersion`. Property definitions and request classes carry the minimum version; the service validates this at runtime against the configured `RequestedServerVersion`.

### Platform limitations

- `BasicAuthModuleForUTF8.cs` is excluded from compilation (`<Compile Remove=...>` in the csproj).
- LDAP Autodiscovery does not work on .NET Standard/Core.
- DNS Autodiscovery does not work on Linux.
