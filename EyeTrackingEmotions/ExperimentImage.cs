using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeTrackingEmotions.LogDataContainers;

namespace EyeTrackingEmotions
{
    public class ExperimentImage
    {
        public string name;
        public Emotion emotion;
        public Gender gender;
        public Image image;

        public ExperimentImage(Emotion emotion, Gender gender, Image image)
        {
            this.emotion = emotion;
            this.gender = gender;
            this.image = image;
            this.name = emotion.ToString() + gender.ToString();
        }
    }
}
