using Microsoft.EntityFrameworkCore;
using SurveyBasket.API.Persistence.EntitiesConfigrations;
using System.Reflection;

namespace SurveyBasket.API.Persistence
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Poll> Polls { get; set; }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			base.OnModelCreating(modelBuilder);
		}
	}
}
