using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Othello.Model
{
    public class Serialiser
    {
       public static void SerializeObject(string filename, object objectToSerialize)
       {
          var stream = File.Open(filename, FileMode.Create);
          var bFormatter = new BinaryFormatter();
          bFormatter.Serialize(stream, objectToSerialize);
          stream.Close();
       }

       public static object DeSerializeObject(string filename)
       {
          var stream = File.Open(filename, FileMode.Open);
          var bFormatter = new BinaryFormatter();
          var objectToSerialize = bFormatter.Deserialize(stream);
          stream.Close();
          return objectToSerialize;
       }
    }
}
