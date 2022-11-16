using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EyeTrackingEmotions.LogDataContainers
{

    public enum Emotion
    {
        None,
        Ashamed,
        Neutral,
        Pride,
        Anger,
        Disgust,
        Fear,
        Happiness,
        Sad
    }
    public enum Gender
    {
        Male,
        Female
    }
    public enum RunState
    {
        Practice,
        Test
    }
    public enum DataCollectionState
    {
        pictureView,
        blankView
    }


    [System.Serializable]
    [XmlRoot("ExperimentLog")]
    public class ExperimentLog
    {
        [XmlElement("SubjectNumber")]
        public int numOfSubjects;
        
        public ExperimentLog() { }
        public ExperimentLog(int numOfSubjects)
        {
            this.numOfSubjects = numOfSubjects;
        }

    }

}
