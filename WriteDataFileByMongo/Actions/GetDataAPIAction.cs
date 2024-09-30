using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WriteDataFileByMongo.Controllers;
using WriteDataFileByMongo.Models;

namespace WriteDataFileByMongo.Actions
{
    public class GetDataAPIAction
    {
        public async Task<List<DataLoggerModel>> GetDataAPI(string loggerid,string channelid, DateTime start, DateTime end)
        {
            PMACController pMACController  = new PMACController();

            List<DataLoggerModel> result = new List<DataLoggerModel>();

            XMLResponseModel data = new XMLResponseModel();

            try
            {
                string hostname = ConfigurationManager.AppSettings["apiGetData"];
                string username = ConfigurationManager.AppSettings["username"];
                string password = ConfigurationManager.AppSettings["password"];

                string startToGetData = $"{start.Year}-{start.Month}-{start.Day}";
                string endToGetData = $"{end.Year}-{end.Month}-{end.Day}";

                string url = $"{hostname}username={username}&password={password}&software=APIDocumentation&number={loggerid}&format=xml&begindate={startToGetData}&enddate={endToGetData}";

                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var responseTask = httpClient.GetAsync(url).Result;

                        if (responseTask.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            SerializerAction ser = new SerializerAction();
                            data = ser.Deserialize<XMLResponseModel>(responseTask.Content.ReadAsStringAsync().Result);

                            if (data.message != null)
                            {
                                string[] channelSplit = channelid.Split(new char[] { '_' }, StringSplitOptions.None);
                                string channelNumber = int.Parse(channelSplit[channelSplit.Length - 1]).ToString();

                                foreach (var messages in data.message)
                                {
                                    foreach (var messageData in messages.message)
                                    {
                                        foreach (var pt in messageData.pt)
                                        {
                                            if (pt.ch == channelNumber)
                                            {
                                                DataLoggerModel el = new DataLoggerModel();
                                                try
                                                {
                                                    el.TimeStamp = DateTime.Parse(pt.time);
                                                }
                                                catch (Exception ex)
                                                {
                                                    el.TimeStamp = null;
                                                }
                                                try
                                                {
                                                    if (pMACController.Strategy(channelid) == 10)
                                                    {
                                                        el.Value = Math.Round(double.Parse(pt.value), 2);
                                                        el.Value = el.Value * 3600;
                                                        el.Value = Math.Round(double.Parse(el.Value.ToString()), 1);
                                                    }
                                                    else
                                                    {
                                                        el.Value = Math.Round(double.Parse(pt.value), 2);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    el.Value = null;
                                                }
                                                if (el.TimeStamp != null)
                                                {
                                                    
                                                    if (el.TimeStamp.Value > start)
                                                    {
                                                        result.Add(el);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        WriteLogAction writeLogAction = new WriteLogAction();
                        writeLogAction.WriteErrorLog(ex.Message).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogAction writeLogAction = new WriteLogAction();
                writeLogAction.WriteErrorLog(ex.Message).Wait();
            }

            return result;
        }
    }
}
