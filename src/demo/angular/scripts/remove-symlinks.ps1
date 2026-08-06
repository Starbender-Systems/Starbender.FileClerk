#!/usr/bin/env pwsh

# Import shared configuration
. "$PSScriptRoot/symlink-config.ps1"

# Get configuration from shared module
$packageDirectories = Get-PackageDirectories

function Remove-Symlinks {
    Write-Host "🗑️  Removing symlinks from library directories...`n" -ForegroundColor Cyan

    $totalRemoved = 0
    $totalSkipped = 0

    foreach ($packageDir in $packageDirectories) {
        $resolvedPath = Resolve-Path $packageDir -ErrorAction SilentlyContinue
        $targetNodeModules = Join-Path $resolvedPath "node_modules"

        if (-not $targetNodeModules) {
            Write-Host "⚠️  Skipping $packageDir (directory not found)" -ForegroundColor Yellow
            $totalSkipped++
            continue
        }

        Write-Host "📁 Processing $packageDir..." -ForegroundColor Cyan

        if (-not (Test-Path $targetNodeModules)) {
            Write-Host "   ⚠️  No node_modules directory found" -ForegroundColor Yellow
            $totalSkipped++
            continue
        }

        try {
            # Remove the entire node_modules directory (which contains symlinks)
            Remove-Item $targetNodeModules -Recurse -Force
            Write-Host "   ✅ Removed node_modules directory" -ForegroundColor Green
            $totalRemoved++
        }
        catch {
            Write-Host "   ❌ Failed to remove $targetNodeModules`: $($_.Exception.Message)" -ForegroundColor Red
            $totalSkipped++
        }
    }

    Write-Host "`n🎉 Cleanup completed! Removed: $totalRemoved, Skipped: $totalSkipped" -ForegroundColor Green
    Write-Host "💡 Libraries will now use their own local dependencies (if any)" -ForegroundColor Cyan
}

# Run the cleanup
Remove-Symlinks