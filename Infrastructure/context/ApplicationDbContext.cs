using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.context;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext(options)
{

}
