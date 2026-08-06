#!/usr/bin/env bash

set -euo pipefail

if [[ $# -ne 2 ]]; then
  echo "Usage: $0 <package-directory> <expected-version>" >&2
  exit 2
fi

package_directory=$1
expected_version=$2

package_ids=(
  Starbender.FileClerk.Application.Contracts
  Starbender.FileClerk.Application
  Starbender.FileClerk.Blazor.Server
  Starbender.FileClerk.Blazor.WebAssembly.Bundling
  Starbender.FileClerk.Blazor.WebAssembly
  Starbender.FileClerk.Blazor
  Starbender.FileClerk.Domain.Shared
  Starbender.FileClerk.Domain
  Starbender.FileClerk.EntityFrameworkCore
  Starbender.FileClerk.HttpApi.Client
  Starbender.FileClerk.HttpApi
  Starbender.FileClerk.Installer
  Starbender.FileClerk.Web
)

mapfile -t package_files < <(find "$package_directory" -maxdepth 1 -type f -name '*.nupkg' -printf '%f\n' | sort)

if [[ ${#package_files[@]} -ne ${#package_ids[@]} ]]; then
  echo "Expected ${#package_ids[@]} packages, found ${#package_files[@]}." >&2
  printf '  %s\n' "${package_files[@]}" >&2
  exit 1
fi

for package_id in "${package_ids[@]}"; do
  package_path="$package_directory/$package_id.$expected_version.nupkg"

  if [[ ! -f "$package_path" ]]; then
    echo "Missing expected package: $package_path" >&2
    exit 1
  fi

  archive_listing=$(unzip -Z1 "$package_path")
  nuspec=$(unzip -p "$package_path" '*.nuspec')

  if [[ $(grep -c '^Nuget.png$' <<<"$archive_listing") -ne 1 ]]; then
    echo "$package_path must contain exactly one root Nuget.png." >&2
    exit 1
  fi

  if [[ $(grep -c '^README.md$' <<<"$archive_listing") -ne 1 ]]; then
    echo "$package_path must contain exactly one root README.md." >&2
    exit 1
  fi

  grep -Fq "<id>$package_id</id>" <<<"$nuspec"
  grep -Fq "<version>$expected_version</version>" <<<"$nuspec"
  grep -Fq '<authors>Starbender Systems</authors>' <<<"$nuspec"
  grep -Fq '<license type="expression">MIT</license>' <<<"$nuspec"
  grep -Fq '<icon>Nuget.png</icon>' <<<"$nuspec"
  grep -Fq '<readme>README.md</readme>' <<<"$nuspec"
  grep -Fq '<projectUrl>https://github.com/Starbender-Systems/Starbender.FileClerk</projectUrl>' <<<"$nuspec"
  grep -Fq "<description>$package_id package for the FileClerk ABP module.</description>" <<<"$nuspec"
  grep -Fq '<tags>FileClerk ABP BLOB storage</tags>' <<<"$nuspec"
  grep -Fq '<repository type="git" url="https://github.com/Starbender-Systems/Starbender.FileClerk.git"' <<<"$nuspec"

  if grep -Eq '(^|/)(bin|obj|node_modules|\.nuget)(/|$)|(^|/)\.env$|\.(cs|csproj|slnx)$' <<<"$archive_listing"; then
    echo "$package_path contains source, a cache, a secret file, or unrelated build output." >&2
    exit 1
  fi

  while IFS= read -r dependency; do
    if [[ "$dependency" != *"version=\"$expected_version\""* ]]; then
      echo "$package_path contains an internal dependency with a mismatched version: $dependency" >&2
      exit 1
    fi
  done < <(grep -oE '<dependency id="Starbender\.FileClerk\.[^"]+" version="[^"]+"' <<<"$nuspec" || true)
done

installer_listing=$(unzip -Z1 "$package_directory/Starbender.FileClerk.Installer.$expected_version.nupkg")
grep -Eq '^content/+Starbender\.FileClerk\.abpmdl$' <<<"$installer_listing"
grep -Eq '^content/+Starbender\.FileClerk\.[^/]+/Starbender\.FileClerk\.[^/]+\.abppkg$' <<<"$installer_listing"

if find "$package_directory" -maxdepth 1 -type f -name '*Tests*.nupkg' -print -quit | grep -q .; then
  echo "Test packages must not be produced." >&2
  exit 1
fi

echo "Validated ${#package_ids[@]} NuGet packages at version $expected_version."
