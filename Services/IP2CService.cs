﻿using IP2C_consumer.Models;
using IP2C_consumer.Services.Interfaces;

namespace IP2C_consumer.Services
{
    /// <summary>
    /// This service fetches country details for a given IP address from an external API.
    /// It follows the Single Responsibility Principle by focusing solely on making HTTP requests to retrieve data.
    /// Dependency Injection is used to inject an HttpClient, which makes the class more testable and flexible.
    /// </summary>
    public class IP2CService : IIP2CService
    {
        private readonly HttpClient _httpClient;

        public IP2CService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Country> FetchCountryFromIPAsync(string ipAddress)
        {
            var response = await _httpClient.GetStringAsync($"http://ip2c.org/{ipAddress}");
            if (!response.StartsWith("1")) return null;

            var parts = response.Split(';');
            return new Country
            {
                Name = parts[3],
                TwoLetterCode = parts[1],
                ThreeLetterCode = parts[2]
            };
        }
    }
}
