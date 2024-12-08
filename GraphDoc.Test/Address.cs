namespace GraphDoc.Test;

/// <summary>
/// A postal address
/// </summary>
public class Address
{
    /// <summary>
    /// The street name. 
    /// </summary>
    public string Street { get; set; }
    /// <summary>
    /// The street number.
    /// </summary>
    public string Number { get; set; }
    /// <summary>
    /// The city.
    /// </summary>
    public string City { get; set; }
    /// <summary>
    /// The postal code.
    /// </summary>
    public string Zip { get; set; }
    /// <summary>
    /// The country.
    /// </summary>
    public string Country { get; set; }
}