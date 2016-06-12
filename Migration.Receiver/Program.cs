using Migration.Service;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Migration.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri[] baseAddresses = new Uri[] { new Uri("net.tcp://localhost:8080/Migration"), new Uri("http://localhost:8081/Migration") };

            using (ServiceHost host = new ServiceHost(typeof(MigrationService), baseAddresses))
            {
                // Enable metadata publishing.
                ServiceMetadataBehavior metadata = new ServiceMetadataBehavior();
                metadata.HttpGetEnabled = true;
                metadata.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(metadata);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                Console.WriteLine("The service is ready at {0}", host.Description.Endpoints[0].Address);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }
        }
    }
}