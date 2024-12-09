namespace GraphDoc.Test;

/// <summary>
/// A natural person
/// </summary>
public class Person
{
    /// <summary>
    /// The person's first name.
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// The person's last name.
    /// </summary>
    public string LastName { get; set; }
    /// <summary>
    /// The person's birth date.
    /// </summary>
    public DateOnly? BirthDate { get; set; }
    /// <summary>
    /// The person's addresses.
    /// </summary>
    public List<Address> Addresses { get; set; }
}