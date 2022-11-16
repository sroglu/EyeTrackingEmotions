using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EyeTrackingEmotions.LogDataContainers
{
    [System.Serializable]
    [XmlType(TypeName = "SubjectEyeLog")]
    public class SubjectEyeLog
    {
        [XmlAttribute(AttributeName = "SubjectID")]
        public int subjectID;

        [XmlAttribute(AttributeName = "LogID")]
        public int logID;

        [XmlArrayItem("EyeDataList")]
        public EyeData[] eyeData;

        public SubjectEyeLog() { }

        public SubjectEyeLog(int subjectID, int logID, EyeData[] eyeData) {
            this.subjectID = subjectID;
            this.logID = logID;
            this.eyeData = eyeData;
        }

    }

    [System.Serializable]
    [XmlType(TypeName = "EyeData")]
    public class EyeData
    {
        [XmlAttribute(AttributeName ="TimeStamp")]
        public long timestamp;

        [XmlElement("X")]
        public double gazeX;

        [XmlElement("Y")]
        public double gazeY;

        public EyeData() { }

        public EyeData(long timestamp, double gazeX, double gazeY) {
            this.timestamp = timestamp;
            this.gazeX = gazeX;
            this.gazeY = gazeY;
        }
    }

}
