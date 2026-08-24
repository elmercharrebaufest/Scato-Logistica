---
applyTo:
  - "Molinos.Scato.sln"
  - "**/*.csproj"
  - ".nuget/NuGet.targets"
---

# Compilación en este entorno (Windows)

## Objetivo
Evitar fallas de build por toolchain y restore en este repositorio legacy (.NET Framework 4.5.2 + NuGet targets clásicos).

## Hacer
- Compilar con MSBuild de Visual Studio (no con `dotnet msbuild` para la solución completa legacy):
  - `C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe Molinos.Scato.sln /p:Configuration=Debug /nologo /m`
- Si `msbuild` no está en PATH, invocar siempre la ruta completa anterior.
- Mantener `.nuget/NuGet.targets` compatible con rutas que contienen `%`:
  - Escapar `%` en variables usadas por `Exec` (`NuGetExePath`, `PackagesConfig`, `SolutionDir`) para que `cmd.exe` no intente expansión inválida.
  - No agregar espacios finales dentro de `-solutionDir`.

## No hacer
- No asumir que `dotnet msbuild` puede compilar todos los proyectos web/WCF/SSDT de esta solución.
- No revertir el escape de `%` en `.nuget/NuGet.targets`, porque vuelve a romper el restore en rutas como `Scato%20Logistica`.
- No introducir cambios de framework o SDK para “forzar” la compilación.

## Comando de referencia
```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" Molinos.Scato.sln /p:Configuration=Debug /nologo /m
```
