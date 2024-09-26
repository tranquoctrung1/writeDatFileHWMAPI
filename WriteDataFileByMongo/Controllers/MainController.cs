using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WriteDataFileByMongo.Actions;
using WriteDataFileByMongo.Models;
using static MongoDB.Driver.WriteConcern;

namespace WriteDataFileByMongo.Controllers
{
    public class MainController
    {
        public async void run()
        {
            PMACController pMACController = new PMACController();
            GetDataLoggerAction getDataLoggerAction = new GetDataLoggerAction();

            WriteLogAction writeLogAction = new WriteLogAction();
            GetTimeStampDataLogger getTimeStampDataLogger = new GetTimeStampDataLogger();

            GetLoggerInConfigFileAction getLoggerInConfigFileAction = new GetLoggerInConfigFileAction();
            GetDataAPIAction getDataAPIAction = new GetDataAPIAction();

            try
            {
                string originPath = ConfigurationManager.AppSettings["OriginPath"];

                List<string> loggers = await getLoggerInConfigFileAction.GetLoggerInConfigFile();


                if (loggers.Count > 0)
                {
                    List<string> transferLoggers = await getLoggerInConfigFileAction.GetTransferLoggerInConfigFile();

                    for (int i = 0; i < loggers.Count; i++)
                    {
                        List<string> listChannelId = new List<string>();

                        string pressure = $"{transferLoggers[i]}_01";
                        string forward = $"{transferLoggers[i]}_02";
                        string reverse = $"{transferLoggers[i]}_03";

                        listChannelId.Add(pressure);
                        listChannelId.Add(forward);
                        listChannelId.Add(reverse);

                        foreach (string channelid in listChannelId)
                        {

                            try
                            {
                                string channelPMAC = channelid;

                                string fileName = Path.Combine(originPath, channelPMAC + ".dat");

                                if (File.Exists(fileName))
                                {
                                    DateTime? lTime = pMACController.Ltime(channelPMAC);
                                    DateTime? fTime = pMACController.Ftime(channelPMAC);

                                    double dbRange = pMACController.Range(channelPMAC);
                                    double dbOffset = pMACController.Offset(channelPMAC);

                                    int byes_per_reading = pMACController.Byes_Per_Reading(channelPMAC);
                                    int rate = pMACController.Rate(channelPMAC);

                                    double valueFind = 255.0;

                                    if (byes_per_reading == 2)
                                    {
                                        valueFind = 32767.0;
                                    }

                                    if (lTime.HasValue)
                                    {

                                        DateTime end = DateTime.Now.AddDays(1);
                                        List<DataLoggerModel> listDataLogger = await getDataAPIAction.GetDataAPI(loggers[i], channelid, lTime.Value, end);

                                        if (listDataLogger.Count > 0)
                                        {
                                            DateTime? currentTime = listDataLogger[listDataLogger.Count - 1].TimeStamp;
                                            if (!currentTime.HasValue)
                                            {
                                                for (int j = listDataLogger.Count - 1; j >= 0; j--)
                                                {
                                                    if (listDataLogger[j].TimeStamp.HasValue)
                                                    {
                                                        currentTime = listDataLogger[j].TimeStamp;
                                                        break;
                                                    }
                                                }
                                            }

                                            int count = 0;
                                            DateTime time = lTime.Value.AddSeconds(rate);
                                            while (time <= currentTime)
                                            {
                                                DataLoggerModel data = listDataLogger.Find(d => d.TimeStamp >= time && d.TimeStamp <= time.AddSeconds(rate));

                                                if(data.Value < 0)
                                                {
                                                    if (channelid[channelid.Length - 1] == '2')
                                                    {
                                                        fileName = Path.Combine(originPath, reverse + ".dat");
                                                        data.Value = data.Value * -1;
                                                    }
                                                }

                                                if (data != null && data.TimeStamp != null && data.Value != null)
                                                {
                                                    long diff = (long)(time - fTime.Value).TotalSeconds;

                                                    if (diff >= 0 && byes_per_reading > 0)
                                                    {
                                                        if (diff % rate == 0)
                                                        {
                                                            using (BinaryWriter reader = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate)))
                                                            {
                                                                double rs = Math.Round((double)((data.Value * valueFind) / dbRange - dbOffset), 3);
                                                                ushort vlbit16 = 0;
                                                                if (byes_per_reading == 2)
                                                                {
                                                                    vlbit16 = Convert.ToUInt16(rs);
                                                                }
                                                                else
                                                                {
                                                                    vlbit16 = Convert.ToByte(rs);
                                                                }
                                                                reader.BaseStream.Position = 78 + (((diff / rate) + 0) * byes_per_reading);
                                                                reader.Write(vlbit16);
                                                            }
                                                        }
                                                    }
                                                }

                                                time = time.AddSeconds(rate);
                                                count++;
                                            }

                                        }

                                    }
                                    else
                                    {
                                        DateTime start = new DateTime(2024, 09, 01);
                                        DateTime end = DateTime.Now;

                                        List<DataLoggerModel> listDataLogger = await getDataAPIAction.GetDataAPI(loggers[i], channelid, start, end.AddDays(1));

                                        if (listDataLogger.Count > 0)
                                        {
                                            int count = 0;
                                            DateTime? time = listDataLogger[0].TimeStamp;
                                            if (!time.HasValue)
                                            {
                                                for (int j = 0; j < listDataLogger.Count; j++)
                                                {
                                                    if (listDataLogger[j].TimeStamp.HasValue)
                                                    {
                                                        time = listDataLogger[j].TimeStamp;
                                                        break;
                                                    }
                                                }
                                            }

                                            DateTime? currentTime = listDataLogger[listDataLogger.Count - 1].TimeStamp;
                                            if (!currentTime.HasValue)
                                            {
                                                for (int j = listDataLogger.Count - 1; j >= 0; j--)
                                                {
                                                    if (listDataLogger[j].TimeStamp.HasValue)
                                                    {
                                                        currentTime = listDataLogger[j].TimeStamp;
                                                        break;
                                                    }
                                                }
                                            }

                                            while (time <= currentTime)
                                            {
                                                DataLoggerModel data = listDataLogger.Find(d => d.TimeStamp >= time && d.TimeStamp <= time.Value.AddSeconds(rate));

                                                if (data.Value < 0)
                                                {
                                                    if (channelid[channelid.Length - 1] == '2')
                                                    {
                                                        fileName = Path.Combine(originPath, reverse + ".dat");
                                                        data.Value = data.Value * -1;
                                                    }
                                                }

                                                if (data != null && data.TimeStamp != null && data.Value != null)
                                                {
                                                    long diff = (long)(time.Value - fTime.Value).TotalSeconds;

                                                    if (diff >= 0 && byes_per_reading > 0)
                                                    {
                                                        if (diff % rate == 0)
                                                        {
                                                            using (BinaryWriter reader = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate)))
                                                            {
                                                                double rs = Math.Round((double)((data.Value * valueFind) / dbRange - dbOffset), 3);
                                                                ushort vlbit16 = 0;
                                                                if (byes_per_reading == 2)
                                                                {
                                                                    vlbit16 = Convert.ToUInt16(rs);
                                                                }
                                                                else
                                                                {
                                                                    vlbit16 = Convert.ToByte(rs);
                                                                }
                                                                reader.BaseStream.Position = 78 + (((diff / rate) + 0) * byes_per_reading);
                                                                reader.Write(vlbit16);
                                                            }
                                                        }
                                                    }
                                                }

                                                time = time.Value.AddSeconds(rate);
                                                count++;
                                            }

                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                await writeLogAction.WriteErrorLog(ex.Message);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                await writeLogAction.WriteErrorLog(ex.Message);

            }
        }

        public async void test()
        {
            PMACController pMACController = new PMACController();
            GetDataLoggerAction getDataLoggerAction = new GetDataLoggerAction();
            WriteLogAction writeLogAction = new WriteLogAction();
            GetTimeStampDataLogger getTimeStampDataLogger = new GetTimeStampDataLogger();

            string channelPMAC = "8650_02";

            string originPath = ConfigurationManager.AppSettings["OriginPath"];

            string fileName = Path.Combine(originPath, channelPMAC + ".dat");

            DateTime? lTime = pMACController.Ltime(channelPMAC);
            DateTime? fTime = pMACController.Ftime(channelPMAC);

            double dbRange = pMACController.Range(channelPMAC);
            double dbOffset = pMACController.Offset(channelPMAC);

            int byes_per_reading = pMACController.Byes_Per_Reading(channelPMAC);
            int rate = pMACController.Rate(channelPMAC);

            double valueFind = 255.0;

            if (byes_per_reading == 2)
            {
                valueFind = 32767.0;
            }

            DateTime time = lTime.Value.AddSeconds(rate);

            long diff = (long)(time - fTime.Value).TotalSeconds;

            if (diff >= 0 && byes_per_reading > 0)
            {
                if (diff % rate == 0)
                {
                    using (BinaryWriter reader = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate)))
                    {
                        double rs = Math.Round((double)((37 * valueFind) / dbRange - dbOffset), 2);
                        ushort vlbit16 = Convert.ToUInt16(rs);
                        reader.BaseStream.Position = 78 + (((diff / rate) + 0) * byes_per_reading);
                        reader.Write(vlbit16);
                    }
                }
            }
        }
    }
}
