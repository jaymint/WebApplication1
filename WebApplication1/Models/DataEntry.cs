/*public class DataEntry 
{
    public DataEntry()
    {
        Id = 0;
        Name = string.Empty;
        Email = string.Empty;
    }
    public DataEntry(int id, string name, string email):this()
    {
        Id = id;
        Name = name;
        Email = email;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}*/

public class DataEntry
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; } // New field
    public string CityName { get; set; }    // New field
}

