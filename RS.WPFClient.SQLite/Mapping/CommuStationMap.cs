using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.WPFClientData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.WPFClientData.Mapping
{
    public sealed class CommuStationMap : IEntityTypeConfiguration<CommuStation>
    {
        public void Configure(EntityTypeBuilder<CommuStation> builder)
        {
            builder.ToTable("CommuStation").HasKey(t=>t.Id);
        }
    }
}
