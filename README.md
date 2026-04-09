# Exchange Web Services Managed API — .NET 10 Port

A community-maintained .NET 10 port of the Exchange Web Services (EWS) Managed API.

## Fork lineage

This project is the third in a chain of forks:

1. **[OfficeDev/ews-managed-api](https://github.com/OfficeDev/ews-managed-api)** — The original C# library published by Microsoft (archived, no longer maintained).
2. **[sherlock1982/ews-managed-api](https://github.com/sherlock1982/ews-managed-api)** — Migrated to .NET Standard / async, removed legacy `Begin`/`End` async pattern.
3. **This repo** — Migrated to .NET 10, updated dependencies, resolved warnings, and made general code improvements.

## What's different from the original

- Targets **net10.0**
- All HTTP-bound public API methods are **async** (`Task<T>` + optional `CancellationToken`)
- Legacy `Begin`/`End` async pattern removed
- Dependencies updated to latest .NET 10 packages
- **LDAP Autodiscovery** does not work on .NET (non-Framework)
- **DNS Autodiscovery** does not work on Linux

## NuGet package

[![NuGet](https://img.shields.io/nuget/v/Microsoft.Exchange.WebServices.Core)](https://www.nuget.org/packages/Microsoft.Exchange.WebServices.Core/)

```
dotnet add package Microsoft.Exchange.WebServices.Core
```

## Prerequisites

- .NET 10 SDK or later
- A mailbox on Exchange Server 2007 or later, or Exchange Online / Office 365

> **Note:** EWS is in sustaining mode since July 2018 — no new features will be added. For new integrations with Exchange Online, [Microsoft Graph](https://graph.microsoft.com) is recommended.

## Getting started

See the original Microsoft documentation for API usage:

- [Get started with EWS Managed API](https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/get-started-with-ews-managed-api-client-applications)
- [EWS Managed API reference](https://docs.microsoft.com/en-us/dotnet/api/microsoft.exchange.webservices.data)

## License

MIT — see [license.txt](license.txt).  
Original code © Microsoft Corporation. See license file for full attribution.

