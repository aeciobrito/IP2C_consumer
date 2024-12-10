using System.Text.RegularExpressions;

namespace IP2C_consumer.Services
{
    public static class IPAddressValidator
    {
        private const string ValidIpAddressPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        public static bool IsValidIpAddress(string ipAddress)
        {
            return Regex.IsMatch(ipAddress, ValidIpAddressPattern);
        }
    }
}
