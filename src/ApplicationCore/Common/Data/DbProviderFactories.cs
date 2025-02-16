namespace GamaEdtech.Backend.Common.Data
{
    using GamaEdtech.Backend.Common.Core;

    public static class DbProviderFactories
    {
        private static DbProviderFactory? factory;

        public static DbProviderFactory GetFactory
        {
            get
            {
                if (factory is not null)
                {
                    return factory;
                }

                factory = Globals.ProviderType switch
                {
                    DbProviderType.SqlServer => new SqlServerProvider(),
                    DbProviderType.DevartOracle => new DevartOracleProvider(),
                    DbProviderType.MySql => new MySqlProvider(),
                    _ => throw new NotSupportedException(Globals.ProviderType.ToString()),
                };
                return factory;
            }
        }
    }
}
