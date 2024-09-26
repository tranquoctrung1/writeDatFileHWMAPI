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
    public class GetSiteAction
    {
        public async Task<List<SiteModel>> GetSites()
        {
            WriteLogAction writeLogAction = new WriteLogAction();

            List<SiteModel> list = new List<SiteModel>();

            try
            {
                Connect connect = new Connect();

                var collection = connect.db.GetCollection<SiteModel>("t_Sites");

                list = collection.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                await writeLogAction.WriteErrorLog(ex.Message);
            }

            return list;

        }
    }
}
