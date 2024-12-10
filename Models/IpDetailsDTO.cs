namespace IP2C_consumer.Models
{
    /// <summary>
    /// Represents the details of a country corresponding to an IP address.
    /// This record is used as a Data Transfer Object (DTO) for transferring data between layers in the application.
    /// </summary>
    public record IpDetailsDTO(string CountryName, string TwoLetterCode, string ThreeLetterCode);
}
