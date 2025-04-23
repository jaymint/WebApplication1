using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<DataEntry> dataentries { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Sender> Senders { get; set; }
    public DbSet<City> Cities { get; set; }

    public DbSet<Truck> Trucks { get; set; }
    public DbSet<ShipmentTruckAssignment> ShipmentTruckAssignments { get; set; }

    public DbSet<User> users { get; set; }



}
