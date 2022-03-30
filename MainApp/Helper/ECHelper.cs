using MainApp.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MainApp.Helper
{
    public class ECHelper
    { 

        #region      Instance

        private ECHelper() { }

        public static ECHelper Instance { get { return new ECHelper(); } }
        string AppLogFile = Application.StartupPath + $"\\{DateTime.Now.ToString("yyyyMMdd")}.log";
        string AuthFile = Application.StartupPath + "\\lineces.xml";
        #endregion
        Dictionary<string,string> namePrefix = new Dictionary<string, string>() {{ "SP", "蝦皮" }, { "AS", "蝦皮24" }, { "MO", "MO" },{ "PO", "松果" }, { "LM", "生活市集" } ,{ "MS","人工銷貨"} };

        public void KillProcess()
        {
            Process[] _p = Process.GetProcessesByName("UploadHelper");
            if (_p.Count() > 1)
            {
                foreach (Process p in _p)
                {
                    if (Process.GetCurrentProcess().Id == p.Id)
                        continue;
                    p.Kill();
                }
            }
        } 
        public void CreateUploadFolder(string uploadPath)
        { 
            if (!System.IO.Directory.Exists(uploadPath))
                System.IO.Directory.CreateDirectory(uploadPath);
            foreach (string nKey in namePrefix.Keys)
            {
                if (!System.IO.Directory.Exists(uploadPath + "\\" + namePrefix[nKey]))
                    System.IO.Directory.CreateDirectory(uploadPath + "\\" + namePrefix[nKey]);
            }  
        }
        public List<FileInfo> GetUploadFiles(string path)
        {
            CreateUploadFolder(path);
            List<FileInfo> filteredFiles = new List<FileInfo>();
            foreach (string nKey in namePrefix.Keys)
            {
                DirectoryInfo di = new DirectoryInfo(path + "\\" + namePrefix[nKey]);
                foreach (FileInfo _file in di.GetFiles("*.*", SearchOption.AllDirectories).Where(s => s.Extension.EndsWith(".xls") || s.Extension.EndsWith(".xlsx")))
                {
                    filteredFiles.Add(_file);
                }
            } 
            return filteredFiles;
        } 
        public bool UploadFileCompleteAsync(FileInfo doneFile, string _token)
        {

            if (Upload(doneFile, _token) != null)
            {
                if (!System.IO.Directory.Exists(Application.StartupPath + "\\UploadComplete"))
                    System.IO.Directory.CreateDirectory(Application.StartupPath + "\\UploadComplete");
                doneFile.CopyTo(Application.StartupPath + "\\UploadComplete\\" + namePrefix.Where(x => x.Value == doneFile.Directory.Name).FirstOrDefault().Key + DateTime.Now.ToString("yyyyMMddHHmmss") + doneFile.Extension, true);
                doneFile.Delete();
                WriteLog("上傳完畢:" + DateTime.Now + ", 檔案名稱:" + doneFile.FullName);
            }
            else
            {
                WriteLog("上傳失敗:" + DateTime.Now + ", 檔案名稱:" + doneFile.FullName); 
            }

            return true;  
        }
        #region     POST/GET

        public async Task<string> Upload(FileInfo file,string _token)
        {
            using (var client = new HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    StringContent token = new StringContent(_token);

                    content.Add(new StreamContent(new MemoryStream(File.ReadAllBytes(file.FullName))), "excel", namePrefix.Where(x => x.Value == file.Directory.Name).FirstOrDefault().Key + DateTime.Now.ToString("yyyyMMddHHmmss")+ file.Extension);
                    content.Add(token, "token");
                    using (
                       var message =
                                                //await client.PostAsync("https://chunmei2020.luckywave.com.tw/v1/service/uploadExcel", content))
                                                //await client.PostAsync("https://demo.luckywave.com.tw/v1/service/uploadExcel", content))
                                                await client.PostAsync("https://www.lordroach.com.tw/v1/service/uploadExcel", content))
                    {

                        string input = await message.Content.ReadAsStringAsync();
                        if (input.Contains("308"))
                            return null;
                        return !string.IsNullOrWhiteSpace(input) ? Regex.Match(input, @"http://\w*\.directupload\.net/images/\d*/\w*\.[a-z]{3}").Value : null;
                    }
                }
            }
        }
        public class PostData {
            public string oken { get; set; }
            public string orders_id { get; set; }
        }
        public async Task<string> PickupOrderItems(string _orders_id,string _token)
        {
            using (var client = new HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent("PickupOrderItems----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    StringContent token = new StringContent(_token);
                    StringContent orders_id = new StringContent(_orders_id);
                    content.Add(token, "token");
                    content.Add(orders_id, "orders_id");
                    using (
                       var message =
                                                //await client.PostAsync("https://chunmei2020.luckywave.com.tw/v1/service/checkComplete", content))
                                                //await client.PostAsync("https://demo.luckywave.com.tw/v1/service/checkComplete", content))
                                                await client.PostAsync("https://www.lordroach.com.tw/v1/service/checkComplete", content))

                    {

                        string input = await message.Content.ReadAsStringAsync();
                        if (input.Contains("200"))
                        {
                            return "撿貨完成!!";
                        }
                        else
                        {
                            return "Api Token 過期  請重新執行程式";
                        }
                    }
                }
            }
        }
        public async Task<string> QueryOrderStatus(string _orders_id, string _token)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    using (var content =
                        new MultipartFormDataContent("checkOrderStatus----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        StringContent token = new StringContent(_token);
                        StringContent orders_id = new StringContent(_orders_id);
                        content.Add(token, "token");
                        content.Add(orders_id, "orders_id");
                        using (
                           //var message = await client.PostAsync("https://demo.luckywave.com.tw/v1/service/checkOrderStatus", content))
                           var message = await client.PostAsync("https://www.lordroach.com.tw/v1/service/checkOrderStatus", content))
                        {

                            string input = await message.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(input))
                            {
 
                                if (input.Contains("200"))
                                {
                                    RequestModel md = JsonConvert.DeserializeObject<RequestModel>(input);

                                    return md.content.ToString(); 
                                }
                                else
                                {
                                    return "Api Token 過期  請重新執行程式";
                                }
                            }

                            return "API網址錯誤";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("QueryOrderStatus :" + ex.ToString());
                return ex.ToString();
            } 
        }
        /// <summary>
        /// 檢核網路連線狀態
        /// </summary>
        /// <returns></returns>
        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 向指定URL發起請求(可用於遠端傳送資料)
        /// </summary>
        /// <param name="url"> 
        /// <returns></returns>
        public bool CheckAuthIsActived(string key, string pw,ref TokenModel token)
        {
            try
            { 
                string unixSTimestamp = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();

                Uri url = new Uri("https://www.lordroach.com.tw/v1/service/check");
                //Uri url = new Uri("https://api.luckywave.com.tw/v1/service/check"); 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                AuthModel data = new AuthModel() { api_key = key, password = pw, time = unixSTimestamp };
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(data));
                }
                string responseStr = "";

                //發出Request
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = sr.ReadToEnd();

                        token = JsonConvert.DeserializeObject<TokenModel>(responseStr);
                        return token.result;
                    }
                }
            }
            catch 
            {
                return false;
            }
        }

        public bool CheckLoginXmlIsExisit()
        { 
            return GetAuthModel() != null;
        }
        public AuthModel GetAuthModel( )
        {
            if (!System.IO.File.Exists(AuthFile))
                return null;
            XDocument xDoc = XDocument.Load(AuthFile);
            AuthModel model =  (from s in xDoc.Descendants("AuthModel")
                 select new AuthModel()
                 {
                     api_key = s.Element("api_key") != null ? s.Element("api_key").Value : null,
                     password = s.Element("password") != null ? s.Element("password").Value : null,
                     uploadPath = s.Element("uploadPath") != null ? s.Element("uploadPath").Value : null,
                     time = s.Element("time") != null ? s.Element("time").Value : null 
                 }).First() ;
            return model;
        }
        #endregion
        #region LOG
        public void WriteLog(string Msg)
        {
             try
            {
                List<FileInfo> filteredFiles = new List<FileInfo>();
                DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
                foreach (FileInfo _file in di.GetFiles("*.*", SearchOption.AllDirectories).Where(s => s.Extension.EndsWith(".log")))
                {
                    string _logDate = _file.Name.Replace(".log", "").Substring(0, 4) + "-" + _file.Name.Replace(".log", "").Substring(4, 2) + "-" + _file.Name.Replace(".log", "").Substring(6, 2);
                    DateTime logDate = DateTime.Parse(_logDate);
                    if (DateTime.Now.Subtract(logDate).Days > 5)
                    {
                        _file.Delete();
                    } 
                }
                using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(AppLogFile, true))
                {
                    outfile.WriteLine("Time:" + DateTime.Now.ToString() + " Message:" + Msg);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        #region XML Tool
        public string ToXML<T>(T obj)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }
        public enum xmlName
        {
            lineces,
            uploadFolder
        }
        public void SaveXml(string xml, xmlName fileName)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            xdoc.Save($"{fileName.ToString()}.xml");
        }
        #endregion
    }
}
