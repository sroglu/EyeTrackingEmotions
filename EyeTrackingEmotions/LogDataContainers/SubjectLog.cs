using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EyeTrackingEmotions.LogDataContainers
{
    [System.Serializable]
    [XmlType(TypeName = "SubjectLog")]
    public class SubjectLog
    {
        [XmlAttribute(AttributeName = "SubjectID")]
        public int subjectID;

        [XmlAttribute(AttributeName = "logID")]
        public int logID;

        [XmlElement("Gender")]
        public Gender gender;

        [XmlElement("RunState")]
        public RunState runState;

        [XmlElement("DataCollectionState")]
        public DataCollectionState dataCollectionState;

        [XmlElement("SelectedEmotion")]
        public Emotion selectedEmotion;

        [XmlElement("CorrectEmotion")]
        public Emotion correctEmotion;

        [XmlElement("OutOfBoxTime")]
        public double outOfBoxTime;

        [XmlElement("Picture")]
        public string pictureName;

        [XmlElement("IsCorrect")]
        public bool isCorrect;

        [XmlArrayItem("ErrorList")]
        public ErrorLog[] subjectErrors;

        public SubjectLog() { }
        public SubjectLog(int subjectID,int logID, Gender gender, RunState runState, DataCollectionState dataCollectionState, Emotion selectedEmotion, Emotion correctEmotion, double outOfBoxTime, string pictureName, ErrorLog[] subjectErrors) {
            this.subjectID = subjectID;
            this.gender = gender;
            this.logID = logID;
            this.runState = runState;
            this.dataCollectionState = dataCollectionState;
            this.selectedEmotion = selectedEmotion;
            this.correctEmotion = correctEmotion;
            this.outOfBoxTime = outOfBoxTime;
            this.pictureName = pictureName;
            this.isCorrect = selectedEmotion == correctEmotion;
            this.subjectErrors = subjectErrors;
        }

    }


}
