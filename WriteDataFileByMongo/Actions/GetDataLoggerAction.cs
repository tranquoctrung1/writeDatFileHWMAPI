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
    public class GetDataLoggerAction
    {
        public async Task<List<DataLoggerModel>> GetDataLogger(string channelid)
        {
            WriteLogAction writeLogAction = new WriteLogAction();

            List<DataLoggerModel> list = new List<DataLoggerModel>();

            try
            {
                Connect connect = new Connect();

                var collection = connect.db.GetCollection<DataLoggerModel>($"t_Data_Logger_{channelid}");

                list = collection.Find(_ => true).ToList().OrderBy(x => x.TimeStamp).ToList();
            }
            catch (Exception ex)
            {
                await writeLogAction.WriteErrorLog(ex.Message);
            }

            return list;
        }

        public async Task<List<DataLoggerModel>> GetDataLoggerByTimeStamp(string channelid, DateTime start)
        {
            WriteLogAction writeLogAction = new WriteLogAction();

            List<DataLoggerModel> list = new List<DataLoggerModel>();

            try
            {
                Connect connect = new Connect();

                var collection = connect.db.GetCollection<DataLoggerModel>($"t_Data_Logger_{channelid}");

                list = collection.Find(d => d.TimeStamp >= start).ToList().OrderBy(x => x.TimeStamp).ToList();
            }
            catch (Exception ex)
            {
                await writeLogAction.WriteErrorLog(ex.Message);
            }

            return list;
        }
    }
}
