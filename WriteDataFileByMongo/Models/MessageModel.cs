using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WriteDataFileByMongo.Models
{
    public class MessageModel
    {
        public string id { get; set; }
        public string number { get; set; }
        public string dateReceived { get; set; }
        public string dateCredited { get; set; }
        [XmlElement(ElementName = "message")]
        public List<MessageDataModel> message { get; set; }
    }
}
