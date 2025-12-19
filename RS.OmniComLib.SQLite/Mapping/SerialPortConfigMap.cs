using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.OmniComLib.SQLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.OmniComLib.SQLite.Mapping
{
    public sealed class SerialPortConfigMap : IEntityTypeConfiguration<SerialPortConfig>
    {
        public void Configure(EntityTypeBuilder<SerialPortConfig> builder)
        {
            builder.ToTable("SerialPortConfig").HasKey(t=>t.Id);
        }
    }
}
