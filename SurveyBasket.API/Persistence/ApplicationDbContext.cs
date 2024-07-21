using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.API.Persistence.EntitiesConfigrations;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.API.Persistence
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public DbSet<Poll> Polls { get; set; }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
		{
			_httpContextAccessor = httpContextAccessor;
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			base.OnModelCreating(modelBuilder);
		}
		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var entries = ChangeTracker.Entries<AuditableEntity>();
			foreach (var entityEntry in entries)
			{
				var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
				if (entityEntry.State == EntityState.Added)
				{
					entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId;
				}
				if (entityEntry.State == EntityState.Modified)
				{
					entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
					entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
				}

			}
			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
