using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EyeTrackingEmotions
{
    [System.Serializable]
    [XmlType(TypeName = "ExperimentInfo")]
    public class ExperimentInfo
    {
        [XmlElement("h")]
        public string header;

        [XmlArrayItem("p")]
        public string[] paragraphs;
    }
}
