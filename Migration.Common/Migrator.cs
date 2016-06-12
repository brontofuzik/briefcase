using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Migration.Common
{
    [DataContract]
    public class Migrator
    {
        #region Fields

        // Not serialized
        private object migrant;

        [DataMember]
        private byte[] serializedMigrant;

        // Not serialized
        private Assembly assembly;

        [DataMember]
        private string assemblyName;

        [DataMember]
        private byte[] serializedAssembly;

        #endregion // Fields

        public Migrator(object migrant)
        {
            this.migrant = migrant;
            SerializeMigrant();

            assembly = migrant.GetType().Assembly;         
            SerializeAssembly();
        }

        public object Migrant
        {
            get
            {
                if (assembly == null) DeserializeAssembly();
                if (migrant == null) DeserializeMigrant();
                return migrant;
            }
        }

        private void SerializeMigrant()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, migrant);

                // Read bytes from stream.
                serializedMigrant = stream.ToArray();
            }
        }

        private void DeserializeMigrant()
        {         
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                // Write bytes to stream.
                stream.Write(serializedMigrant, 0, serializedMigrant.Length);
                stream.Seek(0, SeekOrigin.Begin);

                migrant = formatter.Deserialize(stream);
            }
        }

        private void SerializeAssembly()
        {         
            string path = assembly.Location;
            assemblyName = assembly.GetName().Name + Path.GetExtension(path);
            serializedAssembly = File.ReadAllBytes(path);
        }

        public void DeserializeAssembly()
        {
            File.WriteAllBytes(assemblyName, serializedAssembly);
            assembly = Assembly.Load(serializedAssembly);
        }
    }
}
