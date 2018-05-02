using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Data.Xml.Dom;
using Windows.Web.Http;

namespace AppForNetworktest
{
    class myConnection
    {
        public string res;
        private static myConnection ins;
        private myConnection()
        {
            res = "";
        }

        public static myConnection getConnection()
        {
            if (ins == null)
            {
                ins = new myConnection();
            }

            return ins;
        }

        public async Task<string> getConnectToGetWeatherAsync(string queryString)
        {
            //Create an HTTP client 
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;
            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }
            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }
            //Uri requestUri = new Uri("http://v.juhe.cn/weather/index?format=2&cityname="+queryString+"&key=c8f3ebd5bd91ab7aa0a9be9cc717e5dd");
            Uri requestUri = new Uri("http://v.juhe.cn/weather/index?cityname="+queryString+"&dtype=xml&format=2&key=c8f3ebd5bd91ab7aa0a9be9cc717e5dd");
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try {
                //Send the GET request    
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                return queryString+":"+ getStringFromWeatherXMLString(httpResponseBody);
            } catch (Exception ex){
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                return res;
            }
        }

        private string getStringFromWeatherXMLString(string xmlStr)
        {//200
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);
            IXmlNode root = doc.SelectSingleNode("root");
            XmlNodeList nodeList = root.ChildNodes;
            if (int.Parse(nodeList[0].InnerText) != 200)
            {
                return "city is not exist";
            }
            nodeList = nodeList[2].ChildNodes;
            nodeList = nodeList[1].ChildNodes;
            //遍历所有子节点
            string temperature = "";
            string weather = "";
            string advice = "";
            foreach (IXmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.TagName == "temperature")
                {
                    temperature = "\ntemperature: "+xe.InnerText;
                }
                else if (xe.TagName == "weather")
                {
                    weather = "\nweather: " + xe.InnerText;
                }
                else if (xe.TagName == "dressing_advice")
                {
                    advice = "\nadvice: " + xe.InnerText;
                }
            }
            return temperature+weather+advice;
        }

        //对字符串做md5加密
        private static string GetMD5WithString(string input)
        {
            if (input == null)
            {
                return null;
            }
            MD5 md5Hash = MD5.Create();
            //将输入字符串转换为字节数组并计算哈希数据  
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            //创建一个 Stringbuilder 来收集字节并创建字符串  
            StringBuilder sBuilder = new StringBuilder();
            //循环遍历哈希数据的每一个字节并格式化为十六进制字符串  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            //返回十六进制字符串  
            return sBuilder.ToString();
        }

        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        public async Task<string> getConnectToTranslateAsync(string queryString)
        {
            //Create an HTTP client 
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;
            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }
            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }
            //appid+q+salt+密钥
            Uri requestUri = new Uri("http://api.fanyi.baidu.com/api/trans/vip/translate?q="
                +queryString
                +"&from=en&to=zh&appid=20180424000149859&salt=1435660288&sign="+GetMD5WithString(get_uft8("20180424000149859"+queryString+ "1435660288" + "Jwjp6uJTHbN2YUX7DwNW")));

            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                //Send the GET request    
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

                //"{\"from\":\"en\",\"to\":\"zh\",\"trans_result\":[{\"src\":\"apple\",\"dst\":\"\\u82f9\\u679c\"}]}"
                res = httpResponseBody.ToString();
                JsonObject temp = JsonObject.Parse(res);
                JArray jlist = JArray.Parse(temp["trans_result"].ToString());
                //jp = (JsonObject)JsonConvert.DeserializeObject(res);
                return jlist[0]["dst"].ToString();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                return httpResponseBody;
            }

        }
    }
}
