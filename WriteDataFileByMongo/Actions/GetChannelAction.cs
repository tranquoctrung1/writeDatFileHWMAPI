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
    public class GetChannelAction
    {
        public async Task<List<ChannelConfigModel>> GetChannelByLoggerId(string loggerid)
        {
            WriteLogAction writeLogAction = new WriteLogAction();

            List<ChannelConfigModel> list = new List<ChannelConfigModel>();

            try
            {
                Connect connect = new Connect();

                var collection = connect.db.GetCollection<ChannelConfigModel>("t_Channel_Configurations");

                list = collection.Find(c => c.LoggerId == loggerid).ToList();
            }
            catch (Exception ex)
            {
                await writeLogAction.WriteErrorLog(ex.Message);
            }

            return list;

        }
    }
}
