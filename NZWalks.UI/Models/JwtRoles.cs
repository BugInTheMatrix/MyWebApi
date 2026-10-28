using System.Text;
using System.Text.Json;

namespace NZWalks.UI.Models
{
    // Reads role claims from the saved JWT so views can hide actions the user can't perform.
    // The signature isn't checked here; the API still enforces [Authorize(Roles = ...)].
    public static class JwtRoles
    {
        private const string ShortRoleClaim = "role";
        private const string LongRoleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        public static bool HasRole(string? token, string role)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            try
            {
                var payload = parts[1].Replace('-', '+').Replace('_', '/');
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                using var json = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));

                foreach (var claimName in new[] { ShortRoleClaim, LongRoleClaim })
                {
                    if (!json.RootElement.TryGetProperty(claimName, out var value))
                    {
                        continue;
                    }

                    // A single role is serialized as a string, multiple roles as an array
                    if (value.ValueKind == JsonValueKind.String && value.GetString() == role)
                    {
                        return true;
                    }
                    if (value.ValueKind == JsonValueKind.Array &&
                        value.EnumerateArray().Any(r => r.GetString() == role))
                    {
                        return true;
                    }
                }
            }
            catch (FormatException)
            {
            }
            catch (JsonException)
            {
            }

            return false;
        }
    }
}
