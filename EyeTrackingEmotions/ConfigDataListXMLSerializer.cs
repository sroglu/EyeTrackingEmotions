using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Security.AccessControl;

namespace EyeTrackingEmotions
{

    #region ConfigHandlers

    /// <summary>
    /// Config data list interface for IO operations.
    /// </summary>
    public interface ConfigDataListIOHandler<T>
    {
        string ConfigPath { get; }
        bool GetConfigs(out List<T> dataListHolder);
        bool SaveConfigData(ref List<T> dataList);
    }

    /// <summary>
    /// Config data list interface for IO operations.
    /// </summary>
    public interface ConfigDataIOHandler<T>
    {
        string ConfigPath { get; }
        bool GetConfigs(out T dataHolder);
        bool SaveConfigData(ref T data);
    }


    /// <summary>
    /// XML Serializer for any template type.
    /// </summary>
    public class ConfigDataListXMLSerializer<T> : ConfigDataListIOHandler<T>
    {
        public List<T> dataList = new List<T>();

        const string defaultConfigPath = "Configs.xml";
        public string configPath;

        public string ConfigPath { get { return configPath; } }

        //Config path predefined defined by default path
        public ConfigDataListXMLSerializer(string path = defaultConfigPath)
        {
            configPath = path;
        }

        bool ConfigDataListIOHandler<T>.GetConfigs(out List<T> dataListHolder)
        {
            dataListHolder = new List<T>();

            if (!File.Exists(configPath))
                return false;

            return Deserialize(ref dataListHolder);
        }

        bool ConfigDataListIOHandler<T>.SaveConfigData(ref List<T> list)
        {
            dataList.Clear();
            dataList = list;

            return Serialize(ref dataList);
        }

        bool Serialize(ref List<T> list)
        {
            bool result = false;
            try
            {
                FileStream fs = new FileStream(configPath, FileMode.Create);                
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                serializer.Serialize(fs, list);
                fs.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
        bool Deserialize(ref List<T> list)
        {
            bool result = false;
            try
            {
                FileStream fs = new FileStream(configPath, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                list.Clear();
                list = (List<T>)serializer.Deserialize(fs);
                fs.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }

    public class ConfigDataXMLSerializer<T> : ConfigDataIOHandler<T>
    {
        const string defaultConfigPath = "Configs.xml";
        public string configPath;

        public T data;

        public string ConfigPath { get { return configPath; } }

        //Config path predefined defined by default path
        public ConfigDataXMLSerializer(string path = defaultConfigPath)
        {
            configPath = path;
        }

        public bool GetConfigs(out T dataHolder)
        {
            dataHolder = default(T);
            if (!File.Exists(configPath))
                return false;

            return Deserialize(ref dataHolder);
        }

        public bool SaveConfigData(ref T data)
        {
            this.data = data;

            return Serialize(ref this.data);
        }


        bool Serialize(ref T data)
        {
            bool result = false;
            try
            {
                FileStream fs = new FileStream(configPath, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fs, data);
                fs.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
        bool Deserialize(ref T data)
        {
            bool result = false;
            try
            {
                FileStream fs = new FileStream(configPath, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                data = (T)serializer.Deserialize(fs);
                fs.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }

    static class Utils
    {
        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            DirectoryInfo dInfo = new DirectoryInfo(FileName);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
            dInfo.SetAccessControl(dSecurity);
        }
        public static void RemoveDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            DirectoryInfo dInfo = new DirectoryInfo(FileName);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
            dInfo.SetAccessControl(dSecurity);
        }
    }


    #endregion
}
