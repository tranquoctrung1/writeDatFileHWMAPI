using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WriteDataFileByMongo.Models
{
    public class MessageDataModel
    {
        public string siteId { get; set; }
        public string type { get; set; }
        public string RST { get; set; }
        public string RTC { get; set; }
        public string DST { get; set; }
        public string mode { get; set; }
        public string SR { get; set; }
        [XmlElement(ElementName = "pt")]
        public List<PTModel> pt { get; set; }
    }
}
