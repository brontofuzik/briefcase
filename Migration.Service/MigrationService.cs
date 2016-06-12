using Migration.Common;
using Migration.Contracts;
using System;

namespace Migration.Service
{
    public class MigrationService : IMigrationService
    {
        public void Migrate(Migrator migrator)
        {
            object unknown = migrator.Migrant;
            Console.WriteLine(unknown.ToString());
        }
    }
}
