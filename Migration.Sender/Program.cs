using Migration.Common;
using Migration.Contracts;
using Migration.Migrant;
using System.ServiceModel;

namespace Migration.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IMigrationService> channelFactory = new ChannelFactory<IMigrationService>(new NetTcpBinding(), "net.tcp://localhost:8080/Migration");
            IMigrationService service = channelFactory.CreateChannel();

            Foo foo = new Foo(4, 2);
            service.Migrate(new Migrator(foo));
            
            (service as ICommunicationObject).Close();
        }
    }
}
