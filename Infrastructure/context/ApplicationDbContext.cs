using Domain.Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Quest> Quests { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<Space> Spaces { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<SpaceMember> SpaceMembers { get; set; }
    public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }

    override protected void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>().ToTable("UsersAccount");
        builder.Entity<User>().Property(u => u.Id).HasMaxLength(128);
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");


        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var filter = ConvertFilterExpression(entityType.ClrType);
                builder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    private static LambdaExpression ConvertFilterExpression(Type type)
    {
        var parameter = Expression.Parameter(type, "e");
        var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var softDeleteEntries = ChangeTracker.Entries<ISoftDelete>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in softDeleteEntries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTime.UtcNow;

            if (entry.Entity is User user)
            {
                await SoftDeleteRelated<WorkspaceMember>(wm => wm.UserId == user.Id);
                await SoftDeleteRelated<SpaceMember>(sm => sm.UserId == user.Id);
                await SoftDeleteRelated<Comment>(c => c.UserId == user.Id);
                await SoftDeleteRelated<UserQuest>(q => q.UserId == user.Id);
            }

            else if (entry.Entity is Workspace workspace)
            {
                await SoftDeleteRelated<WorkspaceMember>(wm => wm.WorkspaceId == workspace.Id);
                await SoftDeleteRelated<Invitation>(i => i.WorkspaceId == workspace.Id);
                await SoftDeleteRelated<Space>(s => s.WorkspaceId == workspace.Id);
                await SoftDeleteRelated<Category>(c => c.WorkspaceId == workspace.Id);
                await SoftDeleteRelated<Tag>(t => t.WorkspaceId == workspace.Id);
            }
            else if (entry.Entity is Space space)
            {
                await SoftDeleteRelated<Group>(g => g.SpaceId == space.Id);
                await SoftDeleteRelated<Invitation>(i => i.SpaceId == space.Id);
                await SoftDeleteRelated<SpaceMember>(sm => sm.SpaceId == space.Id);
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task SoftDeleteRelated<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, ISoftDelete
    {
        var relatedEntities = await this.Set<TEntity>()
                                  .Where(predicate)
                                  .Where(e => !e.IsDeleted)
                                  .ToListAsync();

        foreach (var item in relatedEntities)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
        }
    }
}
