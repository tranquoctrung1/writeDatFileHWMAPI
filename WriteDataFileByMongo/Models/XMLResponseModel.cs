using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WriteDataFileByMongo.Models
{
    [Serializable, XmlRoot("messages")]
    public class XMLResponseModel
    {
        public string APIVersion { get; set; }
        [XmlElement(ElementName = "message")]
        public List<MessageModel> message { get; set; }
    }
}
