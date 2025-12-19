using Microsoft.EntityFrameworkCore;
using RS.Commons.Helper;
using RS.OmniComLib.SQLite.Entities;
using System.Data;
using System.Reflection;

namespace RS.OmniComLib.SQLite.DbContexts
{
    public sealed class OmniComLibDbContext : DbContext
    {
        public OmniComLibDbContext()
        {
            this.DatabasePath = PathHelper.MapPath("Database/OmniComLib.db");
            this.MigrationDataBase();
        }

        /// <summary>
        /// 通讯站
        /// </summary>
        public DbSet<CommuStation> CommuStation { get; set; }

        /// <summary>
        /// Modbus 通讯配置
        /// </summary>
        public DbSet<ModbusConfig> ModbusConfig { get; set; }

        /// <summary>
        /// 串口通讯配置
        /// </summary>
        public DbSet<SerialPortConfig> SerialPortConfig { get; set; }


        private void MigrationDataBase()
        {
            if (this.Database.GetPendingMigrations().Any())
            {
                this.Database.Migrate();
            }
        }

        private  string databasePath;

        /// <summary>
        /// 数据库文件存储路径
        /// </summary>
        public  string DatabasePath
        {
            get { return databasePath; }
            private set { databasePath = value; }
        }


        /// <summary>
        /// 这是SQLite数据库连接字符串
        /// </summary>
        public string SqliteConnectionString
        {
            get
            {
                return $"Data Source={DatabasePath}";
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Assembly asm = Assembly.GetExecutingAssembly().ManifestModule.Assembly;
            //获取所有要注册的类
            var typeToRegister = asm.ExportedTypes
                .Where(type => string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.Namespace == "RS.OmniComLib.SQLite.Mapping");
            foreach (var type in typeToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //这里如果未配置
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(this.SqliteConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

    }
}
