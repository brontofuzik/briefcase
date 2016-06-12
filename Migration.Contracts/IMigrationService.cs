using Migration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Migration.Contracts
{
    [ServiceContract]
    public interface IMigrationService
    {
        [OperationContract]
        void Migrate(Migrator migrator);
    }
}
