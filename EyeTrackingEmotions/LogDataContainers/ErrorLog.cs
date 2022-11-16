using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EyeTrackingEmotions.LogDataContainers
{
    [System.Serializable]
    [XmlType(TypeName = "SubjectErrorLog")]
    public class ErrorLog
    {
        [XmlElement("HoveredEmotion")]
        public Emotion hoveredEmotion;

        [XmlElement("HoverTime")]
        public double hoverTime;
        
        public ErrorLog() { }
        public ErrorLog(Emotion hoveredEmotion, double hoverTime) {
            this.hoveredEmotion = hoveredEmotion;
            this.hoverTime = hoverTime;
        }
    }
}
