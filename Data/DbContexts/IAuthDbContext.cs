using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MusicBlendHub.Identity.Models;
using System.Collections.Generic;

namespace MusicBlendHub.Identity.Data.DbContexts
{
    public interface IAuthDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<UnconfirmedUserRecord> UnconfirmedUserRecords { get; set; }
        Task<int> SaveChangesAsync(CancellationToken token);
        Task<int> SaveChangesAsync();
    }
}
