using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Starbender.FileClerk.Features;

public static class FileClerkProviderSelection
{
    public static bool TryParse(string? value, out int[] providerIds)
    {
        providerIds = [];

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(value);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            var values = new List<int>();
            foreach (var element in document.RootElement.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.Number ||
                    !element.TryGetInt32(out var id) ||
                    id <= 0)
                {
                    return false;
                }

                values.Add(id);
            }

            providerIds = values.Distinct().Order().ToArray();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static string Serialize(IEnumerable<int> providerIds)
    {
        return JsonSerializer.Serialize(providerIds.Distinct().Order());
    }

    public static bool IsCanonical(string? value)
    {
        return TryParse(value, out var providerIds) &&
               string.Equals(value, Serialize(providerIds), StringComparison.Ordinal);
    }
}
