using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EyeTrackingEmotions
{
    public partial class MainForm : Form
    {

        public static string baseFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string experimentInfoFile = @"\ExperimentInfo.xml";

        ConfigDataIOHandler<ExperimentInfo> experimentInfoHandler;
        ExperimentInfo experimentInfo;

        ExperimentForm expForm;
        public MainForm()
        {
            InitializeComponent();

            Console.WriteLine();
            comboBox1.SelectedItem = comboBox1.Items[0];

            experimentInfoHandler = new ConfigDataXMLSerializer<ExperimentInfo>(baseFolder+ experimentInfoFile);
            experimentInfoHandler.GetConfigs(out experimentInfo);

            if (experimentInfo == null)
            {
                experimentInfo = new ExperimentInfo();
                experimentInfo.header = String.Empty;
                experimentInfo.paragraphs = new string[] { "" };
                experimentInfoHandler.SaveConfigData(ref experimentInfo);
            }
            
            label2.Text = experimentInfo.header;
            richTextBox1.Clear();
            foreach (string p in experimentInfo.paragraphs)
            {
                richTextBox1.Text += "    " + p + Environment.NewLine;
            }
            

            expForm = new ExperimentForm();
        }

        private void StartNewExperiment(object sender, EventArgs e)
        {
            //Application.Run(expForm);
            expForm.Hide();
            expForm.Show();
            expForm.StartExperiment((LogDataContainers.Gender)comboBox1.Items.IndexOf(comboBox1.SelectedItem));
            expForm.BringToFront();
        }

        private void OpenLogFolder(object sender, EventArgs e)
        {
            Process.Start(ExperimentForm.configPath);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
