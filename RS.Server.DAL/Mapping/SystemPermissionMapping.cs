using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Server.Entity;

namespace RS.Server.DAL.Mapping
{
    /// <summary>
    /// 系统权限表配置映射
    /// </summary>
    internal class SystemPermissionMapping : IEntityTypeConfiguration<SystemPermissionEntity>
    {
        public void Configure(EntityTypeBuilder<SystemPermissionEntity> builder)
        {
            builder.ToTable("SystemPermission").HasKey(t => t.Id);
            //设置不自动增长
            builder.Property(t => t.Id).ValueGeneratedNever();
        }
    }
}