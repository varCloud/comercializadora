# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

`lluviaBackEnd` is an ASP.NET MVC 5 (.NET Framework 4.6.1) web application — the back-office / WMS system for Comercializadora Lluvia (production, inventory, sales, purchasing, invoicing/CFDI). It is a classic (non-SDK-style) Visual Studio project using Web Forms-era tooling: `packages.config` for NuGet, `Web.config` for configuration, and IIS Express for local hosting.

Note: this folder (`lluviaBackEnd/`) sits inside a larger git repository rooted at the parent `comercializadora/` directory, which also contains a sibling project `PrintDocumentolluvia` (a largely duplicated copy of this app's `Models`, `DAO`, and `WebServices` — same namespaces, forked source, not shared via a common library) and a `refactor-api` folder. Changes to shared business logic (models, DAOs, stored procedures) often need to be mirrored manually in `PrintDocumentolluvia` if that surface is also affected — check before assuming a fix is complete.

The solution file (`lluviaBackEnd.sln`) only includes the `lluviaBackEnd` project. Sibling folders `lluviaBackEndDAO/` and `lluviaBackEndEntidades/` contain `.csproj` files but are **not** part of the solution and are not referenced — treat them as dead/unused scaffolding, not as the real data-access layer.

## Build & run

Requires Visual Studio's MSBuild (the .NET Framework/Core SDK's `dotnet build` cannot build this `System.Web`-based project) and IIS Express.

```powershell
# Build (adjust path to installed VS edition/version)
& "C:\Program Files\Microsoft Visual Studio\<version>\<edition>\MSBuild\Current\Bin\MSBuild.exe" `
  lluviaBackEnd.sln /p:Configuration=Debug /m

# Restore NuGet packages if `packages\` is missing/incomplete (packages.config style, not PackageReference)
# via nuget.exe restore, or open+build once in Visual Studio

# Run locally with IIS Express (site name/port come from .vs/<solution>/config/applicationhost.config,
# generated the first time the solution is opened in Visual Studio)
& "C:\Program Files\IIS Express\iisexpress.exe" `
  /config:".vs\lluviaBackEnd\config\applicationhost.config" /site:lluviaBackEnd
```

There is no automated test suite in this project.

## Configuration

- `lluviaBackEnd/Web.config` — the SQL Server connection string is **not** in the standard `<connectionStrings>` section; it's a plain `<appSettings>` key: `ConfigurationManager.AppSettings["conexionString"]`, read individually inside almost every DAO method's `using (db = new SqlConnection(...))` block.
- Other business config also lives in `<appSettings>` (e.g. `urlDominio`, `pathLog`, `claveGeneraSello`, `FacturarPro`), not in dedicated config sections.
- The active `conexionString` points at the live/shared database (`DB_A57E86_comercializadora` on `site4now.net`) — there is no local database. A commented-out alternate key points at a `lluviadesarrollo` database for development use.

## Architecture

**Almost all business logic lives in SQL Server stored procedures, not in C#.** Controllers and DAOs are thin: a controller action builds/receives a filter model, a DAO method opens a `SqlConnection`, calls a stored procedure by name via Dapper (`db.QueryMultiple("SP_...", parameters, commandType: CommandType.StoredProcedure)`), and maps the result into a `Notificacion<T>`. When a report/screen produces "wrong" numbers or needs different filtering behavior, the fix usually has to happen in the stored procedure (in the actual SQL Server database), not just in C#. SP source scripts (when they exist as files, not always kept in sync with the live DB) live under `Analisis/Scripts .../` in the parent repo, one `.sql` file per procedure/function.

**Response envelope convention.** Nearly every DAO method returns `Notificacion<T>` (`Models/Notificacion.cs`): `Estatus` (200 = ok, anything else = error/no-results), `Mensaje`, `Modelo` (the payload). The convention is: every stored procedure's first result set is a status row (`select 200 status, 'mensaje'` or similar) read via `result.ReadFirst()`, and the second result set (`result.Read<T>()`) is the actual data — read the SP together with the DAO method to know both result-set shapes.

**UI pattern: server-rendered partial views over AJAX, not an SPA.** Screens are Razor views with a filter form wired via `Ajax.BeginForm(...)` (Microsoft's unobtrusive AJAX, not a JS framework) that posts to a controller action returning a `PartialView`, which gets injected into a placeholder `<div>` and then wired up as a DataTables table (jQuery DataTables + Buttons for PDF/Excel export, Select2 for dropdowns, bootstrap-daterangepicker for date ranges via the shared `InitRangePicker(...)` helper in `js/Index.js`). New screens should follow this exact plumbing (see e.g. `Views/ProduccionAgranel/` + `js/EvtMPLIndividual.js` for a recent example) rather than introducing a different frontend pattern.

Avoid the inconsistent alternative that also exists in the codebase: an action that returns raw serialized JSON (`Json(JsonConvert.SerializeObject(notificacion), ...)`) for what is really an HTML table, with the `<table>` markup then hand-built in JavaScript from the parsed JSON (e.g. `ProductosController.BuscarCargaMercanciaLiquidos` + `onSuccessCargaMercanciaLiquidos` in `js/EvtConsultarProduccionProductos.js`). That duplicates rendering logic in JS instead of Razor and drifts from every other screen's convention — new work should return a `PartialView` with the markup in a `.cshtml`, not a full JSON payload rendered client-side.

**Auth/session, and a permissions gotcha.** The logged-in user is a `Sesion` object in `Session["UsuarioActual"]` (`Models/Sesion.cs`), including `permisosModulo` (a list of module permissions) and role info. `Filters/PermisoAttribute.cs` looks like it enforces per-action permission checks (`[PermisoAttribute(Permiso = EnumRolesPermisos.X)]` on controller actions), but **its actual permission check is commented out** — today it only redirects when a physical-inventory lock is active. Real permission enforcement is done by hiding/showing menu links in `Views/Shared/_Layout.cshtml` via `sesion.permisosModulo.Exists(p => p.idModulo == (int)EnumRolesPermisos.X && p.tienePermiso)`. This means an action is not actually protected from direct URL access by the attribute alone — don't assume `[PermisoAttribute]` is a real server-side guard when reasoning about security.

**External/precompiled dependency.** The `AccesoDatos` reference (`Reference Include="AccesoDatos"` in the `.csproj`) is a precompiled DLL (`DLL/AccesoDatos.dll`) with no source in this repo — it's a black box.

**Menus and permissions enum.** New screens are gated by `EnumRolesPermisos` (`Models/Enumeraciones.cs`) and wired into `Views/Shared/_Layout.cshtml`'s permission-checked `<li>` blocks. Follow the existing pattern of single-link vs. dropdown-with-sub-`<li>` items already in that file when adding or restructuring a menu entry.
