using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notch_API.Models;

namespace Notch_API.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.Property(d => d.DepartmentId)
                .ValueGeneratedOnAdd(); // Auto-generates DepartmentId
        }
    }

}
