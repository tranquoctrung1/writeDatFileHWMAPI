using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WriteDataFileByMongo.ConnectDB;
using WriteDataFileByMongo.Models;

namespace WriteDataFileByMongo.Actions
{
    public class GetTimeStampDataLogger
    {
        public async Task<DateTime?> GetFirstTimeStampDataLogger(string channelid)
        {
            WriteLogAction writeLogAction = new WriteLogAction();

            DateTime? timeStamp = null;

            try
            {
                Connect connect = new Connect();

                var collection = connect.db.GetCollection<DataLoggerModel>($"t_Data_Logger_{channelid}");

                List<DataLoggerModel> list = collection.Find(_ => true).ToList().OrderBy(s => s.TimeStamp).ToList();

                if (list.Count > 0)
                {
                    timeStamp = list[0].TimeStamp;
                }

            }
            catch (Exception ex)
            {
                await writeLogAction.WriteErrorLog(ex.Message);
            }

            return timeStamp;
        }

        public async Task<DateTime?> GetCurrentTimeStampDataLogger(string channelid)
        {
            WriteLogAction writeLogAction = new WriteLogAction();

            DateTime? timeStamp = null;

            try
            {
                Connect connect = new Connect();

                var collection = connect.db.GetCollection<DataLoggerModel>($"t_Data_Logger_{channelid}");

                List<DataLoggerModel> list = collection.Find(_ => true).ToList().OrderByDescending(s => s.TimeStamp).ToList();

                if (list.Count > 0)
                {
                    timeStamp = list[0].TimeStamp;
                }
            }
            catch (Exception ex)
            {
                await writeLogAction.WriteErrorLog(ex.Message);
            }

            return timeStamp;
        }
    }
}
