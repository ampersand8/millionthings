namespace MillionThings.WebAPI.Models;

public class MillionThingsDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    
    public string DatabaseName { get; set; } = null!;
    
    public string MillionThingsCollectionName { get; set; } = null!;

}