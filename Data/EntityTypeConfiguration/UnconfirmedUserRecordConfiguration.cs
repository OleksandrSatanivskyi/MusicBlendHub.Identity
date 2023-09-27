using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MusicBlendHub.Identity.Models;

namespace MusicBlendHub.Identity.Data.EntityTypeConfiguration
{
    public class UnconfirmedUserRecordConfiguration : IEntityTypeConfiguration<UnconfirmedUserRecord>
    {
        public void Configure(EntityTypeBuilder<UnconfirmedUserRecord> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Name).HasMaxLength(16);
            builder.Property(x => x.Password).HasMaxLength(256);
        }
    }
}