using CloudSupportSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudSupportSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SupportTicket> SupportTickets { get; set; }
    }
}
