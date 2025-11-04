using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Server.Entity;

namespace RS.Server.DAL.Mapping
{
    /// <summary>
    /// 数据表权限表配置映射
    /// </summary>
    internal class TablePermissionMapping : IEntityTypeConfiguration<TablePermissionEntity>
    {
        public void Configure(EntityTypeBuilder<TablePermissionEntity> builder)
        {
            builder.ToTable("TablePermission").HasKey(t => t.Id);
            //设置不自动增长
            builder.Property(t => t.Id).ValueGeneratedNever();
        }
    }
}