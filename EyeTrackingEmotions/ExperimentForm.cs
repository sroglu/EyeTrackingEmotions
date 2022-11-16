using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using EyeTrackingController;
using EyeTrackingEmotions.LogDataContainers;


namespace EyeTrackingEmotions
{
    public partial class ExperimentForm : Form
    {
        public static string configPath = MainForm.baseFolder + @"\LogFolder";
        static string settingsFile = @"\ExperimentLogs.xml";
        ConfigDataListIOHandler<SubjectLog> subjectLogHandler;
        ConfigDataListIOHandler<SubjectEyeLog> subjectEyeLogHandler;
        ConfigDataIOHandler<ExperimentLog> experimentLogHandler;

        List<SubjectLog> subjectListLog;
        List<ErrorLog> subjectErrorListLog;

        List<SubjectEyeLog> subjectEyeLog;
        List<EyeData> eyeDataList;

        ExperimentLog experimentLogs;

        Dictionary<string, ExperimentImage> practiceImages, testImages;
        List<string> imagePool;
        
        Image blankImage;

        ExperimentImage currentExperimentImage;
        Stopwatch stopwatch = new Stopwatch();
        TimeSpan timeSpan;
        bool nextImageIsPractice = true;
        bool experimentEnded = false;
        int logCounter = 0;

        System.Timers.Timer blankImageTimer;

        EyeTrackingController.EyeTrackingController eyeTrackingController;
        

        // callback routine declaration
        private delegate void GetSampleCallback(EyeTrackingController.EyeTrackingController.SampleStruct sampleData);

        // callback function instances
        GetSampleCallback sampleCallback;

        BackgroundWorker backgroundWorker;

        enum ExperimentState
        {
            Started,
            InPractice,
            StartTestVerification,
            InTest,
            Ended
        }
        ExperimentState expState = ExperimentState.Started;
        public ExperimentForm()
        {
            InitializeComponent();


            InitSettings();
            
            //TestLog();
        }



        void InitSettings()
        {
            // instantiate the eye tracking controller 
            eyeTrackingController = new EyeTrackingController.EyeTrackingController();

            // define callbacks for data stream 
            sampleCallback = new GetSampleCallback(GetSampleCallbackFunction);

            eyeTrackingController.iV_SetSampleCallback(sampleCallback);

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(CheckEyeTrackerStatus);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(StartEyeTracker);
            backgroundWorker.RunWorkerAsync();


            Utils.AddDirectorySecurity(configPath, Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
            experimentLogHandler = new ConfigDataXMLSerializer<ExperimentLog>(configPath + settingsFile);
            experimentLogHandler.GetConfigs(out experimentLogs);
            if (experimentLogs == null)
                experimentLogs = new ExperimentLog(0);
            LoadImages();

            pictureBox.Location = new Point(this.Width / 2- pictureBox.Width/2, this.Height / 2- (int)(pictureBox.Height*(float)(0.8)) );
            flowLayoutPanel2.Location = new Point(this.Width / 2 - flowLayoutPanel2.Width / 2, this.Height - flowLayoutPanel2.Height-panel1.Height*3);
            panel1.Location = new Point(this.Width / 2 - panel1.Width / 2, this.Height - panel1.Height * 4);

            groupBox1.Location = new Point(this.Width / 2 - groupBox1.Width / 2, (this.Height - groupBox1.Height) / 2);

            groupBox1.Visible = false;
        }


        private void StartEyeTracker(object sender, ProgressChangedEventArgs e)
        {
            int ret = 0;
            string result = "";

            try
            {
                // start the myGaze eye tracking server  
                ret = eyeTrackingController.iV_Start();
                if (ret == 1) result = "Started server";
                if (ret == 4) result = "Server already running";
                if (ret >= 100) result = "Failed to start server: " + ret;

            }
            catch (Exception exc)
            {
                result = "Exception during eyeTrackingController start: " + exc.Message;
            }

            Console.WriteLine(result);
            if (ret == 1)
            {
                eyeTrackingController.iV_Connect();
                Console.WriteLine("Connected");
            }

        }
        

        private void CheckEyeTrackerStatus(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                //Console.WriteLine(eyeTrackingController.iV_IsConnected());
                int result = eyeTrackingController.iV_IsConnected();
                if (result >= 100)
                    (sender as BackgroundWorker).ReportProgress(1, "StartConncetion");
                Thread.Sleep(1000);
            }
        }

        void LoadImages()
        {


            blankImage = Properties.Resources.plus;

            practiceImages = new Dictionary<string, ExperimentImage>();
            AddImagesTo(ref practiceImages,new ExperimentImage(Emotion.Ashamed,     Gender.Male,    Properties.Resources.ashamedM));
            AddImagesTo(ref practiceImages,new ExperimentImage(Emotion.Ashamed,     Gender.Female,  Properties.Resources.ashamedW));
            AddImagesTo(ref practiceImages, new ExperimentImage(Emotion.Neutral,    Gender.Male,    Properties.Resources.neutralM));
            AddImagesTo(ref practiceImages, new ExperimentImage(Emotion.Neutral,    Gender.Female,  Properties.Resources.neutralW));
            AddImagesTo(ref practiceImages, new ExperimentImage(Emotion.Pride,      Gender.Male,    Properties.Resources.prideM));
            AddImagesTo(ref practiceImages, new ExperimentImage(Emotion.Pride,      Gender.Female,  Properties.Resources.prideW));


            testImages = new Dictionary<string, ExperimentImage>();
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Anger,      Gender.Male,    Properties.Resources.angerM));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Anger,      Gender.Female,  Properties.Resources.angerW));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Disgust,    Gender.Male,    Properties.Resources.disgustM));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Disgust,    Gender.Female,  Properties.Resources.disgustW));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Fear,       Gender.Male,    Properties.Resources.fearM));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Fear,       Gender.Female,  Properties.Resources.fearW));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Happiness,  Gender.Male,    Properties.Resources.happinessM));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Happiness,  Gender.Female,  Properties.Resources.happinessW));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Sad,        Gender.Male,    Properties.Resources.sadM));
            AddImagesTo(ref testImages, new ExperimentImage(Emotion.Sad,        Gender.Female,  Properties.Resources.sadW));
            
        }
        void AddImagesTo(ref Dictionary<string, ExperimentImage> imageDictionary,ExperimentImage eImage)
        {
            imageDictionary.Add(eImage.name, eImage);
        }

        Gender gender;
        public void StartExperiment(Gender gender)
        {
            if (experimentLogs == null) return;

            this.gender = gender;

            int subNo = experimentLogs.numOfSubjects;

            experimentEnded = false;
            nextImageIsPractice = true;

            if (subjectListLog != null)
                subjectListLog.Clear();
            subjectListLog = new List<SubjectLog>();

            if (subjectEyeLog != null)
                subjectEyeLog.Clear();
            subjectEyeLog = new List<SubjectEyeLog>();


            subjectLogHandler = new ConfigDataListXMLSerializer<SubjectLog>(configPath + @"\" + experimentLogs.numOfSubjects + ".xml");
            subjectEyeLogHandler = new ConfigDataListXMLSerializer<SubjectEyeLog>(configPath + @"\" + experimentLogs.numOfSubjects + "_EyeLog.xml");


            if (subjectErrorListLog != null)
                subjectErrorListLog.Clear();
            subjectErrorListLog = new List<ErrorLog>();

            if (eyeDataList != null)
                eyeDataList.Clear();
            eyeDataList = new List<EyeData>();

            logCounter = 0;

            imagePool = new List<string>();
            imagePool.AddRange(practiceImages.Keys);
            
            expState = ExperimentState.InPractice;

            panel1.Focus();
            ShowBlankImage();
        }
        void TestLog()
        {
            subjectListLog.Add(new SubjectLog(experimentLogs.numOfSubjects,0, Gender.Female, RunState.Test, DataCollectionState.blankView, Emotion.Fear, Emotion.Fear, 435674574, "blank1", null));
            subjectListLog.Add(new SubjectLog(experimentLogs.numOfSubjects,0, Gender.Female, RunState.Test, DataCollectionState.blankView, Emotion.Neutral, Emotion.Fear, 435674574, "blank2", null));
            subjectListLog.Add(new SubjectLog(experimentLogs.numOfSubjects,0, Gender.Female, RunState.Test, DataCollectionState.blankView, Emotion.Ashamed, Emotion.Ashamed, 435674574, "blank3", null));

            if (subjectLogHandler != null && experimentLogHandler != null)
            {
                subjectLogHandler.SaveConfigData(ref subjectListLog);
                experimentLogHandler.SaveConfigData(ref experimentLogs);
            }
        }
        private void BlankImageShown(object sender, ElapsedEventArgs e)
        {
            blankImageTimer.Elapsed -= BlankImageShown;
            blankImageTimer.Dispose();
            ShowNextImage();
        }
        void ShowBlankImage()
        {            
            pictureBox.Image = blankImage;

            if (blankImageTimer != null)
            {                
                blankImageTimer.Dispose();
            }
            blankImageTimer = new System.Timers.Timer(1000);
            blankImageTimer.Elapsed += BlankImageShown;
            blankImageTimer.Start();
            
        }

        bool passMouseLeave = false;
        void ShowNextImage()
        {
            passMouseLeave = true;
            int titleBarHeight = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            Cursor.Position = new Point(this.Location.X + panel1.Location.X + panel1.Width / 2, this.Location.Y + panel1.Location.Y + panel1.Height / 2);
            
            if (imagePool != null && imagePool.Count > 0){
                if (nextImageIsPractice)
                {
                    int nextImageIndex = new Random().Next(0, imagePool.Count - 1);
                    currentExperimentImage = practiceImages[imagePool[nextImageIndex]];
                    imagePool.RemoveAt(nextImageIndex);
                    if (!(imagePool.Count > 0))
                    {
                        //No test img 
                        nextImageIsPractice = false;

                        imagePool = new List<string>();
                        imagePool.AddRange(testImages.Keys);
                    }
                }
                else
                {
                    int nextImageIndex = new Random().Next(0, imagePool.Count - 1);
                    currentExperimentImage = testImages[imagePool[nextImageIndex]];
                    imagePool.RemoveAt(nextImageIndex);

                    if (!(imagePool.Count > 0))
                    {
                        experimentEnded = true;
                    }

                }
            }

            pictureBox.Image = currentExperimentImage.image;
            decisionFlag = true;

        }
        private void OnMouseLeaveBounds(object sender, EventArgs e)
        {
            stopwatch.Start();
        }
        Stopwatch errorSw = new Stopwatch();
        private void OnDecisionErrorStarted(object sender, EventArgs e)
        {
            if (currentExperimentImage != null && decisionFlag)
            {
                errorSw.Reset();
                errorSw.Start();
            }
        }        
        private void OnDecisionError(object sender, EventArgs e)
        {
            if (passMouseLeave)
            {
                passMouseLeave = false;
                return;
            }

            if (currentExperimentImage != null && decisionFlag)
            {
                errorSw.Stop();
                TimeSpan errorTime = errorSw.Elapsed;

                subjectErrorListLog.Add(new ErrorLog(GetEmotionFromButtons(sender as Button), errorTime.Milliseconds));
            }

        }
        Emotion GetEmotionFromButtons(Button emoButton)
        {
            switch (emoButton.Text)
            {
                case "Ashamed":
                    return Emotion.Ashamed;
                case "Neutral":
                    return Emotion.Neutral;
                case "Pride":
                    return Emotion.Pride;
                case "Anger":
                    return Emotion.Anger;
                case "Disgust":
                    return Emotion.Disgust;
                case "Fear":
                    return Emotion.Fear;
                case "Happiness":
                    return Emotion.Happiness;
                case "Sad":
                    return Emotion.Sad;
            }
            return Emotion.None;
        }

        bool decisionFlag=true;
        object decisionLockObj = new object();
        bool testStarted = false;

        private void OnButtonClicked(object sender, EventArgs e)
        {
            panel1.Focus();

            lock (decisionLockObj)
            {
                decisionFlag = false;

                stopwatch.Stop();
                timeSpan = stopwatch.Elapsed;
                stopwatch.Reset();

                Button button = sender as Button;

                Emotion selectedEmotion = GetEmotionFromButtons(button);

                RunState logRunState = RunState.Practice;
                if (expState == ExperimentState.InTest)
                    logRunState = RunState.Test;

                //--------------------Creating Subject Choice Logs---------------------------
                subjectListLog.Add(
                    new SubjectLog(
                        experimentLogs.numOfSubjects,
                        logCounter,
                        gender,
                        logRunState,
                        DataCollectionState.pictureView,
                        selectedEmotion,
                        currentExperimentImage.emotion,
                        timeSpan.TotalMilliseconds,
                        currentExperimentImage.name,
                        subjectErrorListLog.ToArray()
                        ));

                if (subjectErrorListLog != null)
                    subjectErrorListLog.Clear();
                subjectErrorListLog = new List<ErrorLog>();
                
                //-------------------Creating Eye Logs----------------------------------------
                subjectEyeLog.Add(new SubjectEyeLog(experimentLogs.numOfSubjects, logCounter, eyeDataList.ToArray()));

                if (eyeDataList != null)
                    eyeDataList.Clear();
                eyeDataList = new List<EyeData>();
                                      






                if (!nextImageIsPractice && expState == ExperimentState.InPractice)
                {

                    groupBox1.Visible = true;
                    pictureBox.Visible = false;
                    flowLayoutPanel2.Visible = false;

                    expState = ExperimentState.StartTestVerification;
                }
                

                logCounter++;


                //-------------------------Selection Log Ended ------------------------------
                if (expState == ExperimentState.StartTestVerification)
                {

                }
                else if (!experimentEnded)
                {
                    ShowBlankImage();
                }
                else
                {
                    experimentLogs.numOfSubjects++;
                    
                    expState = ExperimentState.Ended;

                    if (subjectLogHandler != null && experimentLogHandler != null)
                    {
                        subjectLogHandler.SaveConfigData(ref subjectListLog);
                        subjectEyeLogHandler.SaveConfigData(ref subjectEyeLog);
                        experimentLogHandler.SaveConfigData(ref experimentLogs);
                    }

                    this.Hide();
                }
            }

        }


        private void X_Click(object sender, EventArgs e)
        {
            Hide();
        }

        string eyeData;

        private void button1_Click(object sender, EventArgs e)
        {
            testStarted = true;
            groupBox1.Visible = false;
            pictureBox.Visible = true;
            flowLayoutPanel2.Visible = true;

            expState = ExperimentState.InTest;
            ShowBlankImage();
        }

        private void ExperimentForm_Load(object sender, EventArgs e)
        {

        }

        void GetSampleCallbackFunction(EyeTrackingController.EyeTrackingController.SampleStruct sampleData)
        {
            /*
            eyeData = "Time: ~" + sampleData.timestamp.ToString() +
                "~ - X: ~" + sampleData.leftEye.gazeX.ToString() +
                "~ - Y: ~" + sampleData.leftEye.gazeY.ToString();

            Console.WriteLine(eyeData);
            */

            if (eyeDataList != null)
                eyeDataList.Add(new EyeData(sampleData.timestamp, sampleData.leftEye.gazeX, sampleData.leftEye.gazeY));

        }
        
    }
}
