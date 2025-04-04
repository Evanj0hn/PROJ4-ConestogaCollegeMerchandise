using Microsoft.EntityFrameworkCore;
using ConestogaCollegeMerchandise.Models;
using System.Collections.Generic;

namespace ConestogaCollegeMerchandise.Data;

public class MerchandiseContext : DbContext
{
    public MerchandiseContext(DbContextOptions<MerchandiseContext> options)
        : base(options)
    {
    }

    public DbSet<Merchandise> MerchandiseItems { get; set; }
}
