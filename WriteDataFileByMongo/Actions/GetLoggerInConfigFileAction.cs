using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteDataFileByMongo.Actions
{
    public class GetLoggerInConfigFileAction
    {
        public async Task<List<string>> GetLoggerInConfigFile()
        {
            List<string> list = new List<string>();

            try
            {
                string loggerInConfig = ConfigurationManager.AppSettings["logger"];

                list = loggerInConfig.Split(new char[] { ';' }).ToList();
            }
            catch (Exception ex)
            {
                WriteLogAction writeLogAction = new WriteLogAction();
                writeLogAction.WriteErrorLog(ex.Message).Wait();
            }


            return list;
        }

        public async Task<List<string>> GetTransferLoggerInConfigFile()
        {
            List<string> list = new List<string>();

            try
            {
                string loggerInConfig = ConfigurationManager.AppSettings["transferlogger"];

                list = loggerInConfig.Split(new char[] { ';' }).ToList();
            }
            catch (Exception ex)
            {
                WriteLogAction writeLogAction = new WriteLogAction();
                writeLogAction.WriteErrorLog(ex.Message).Wait();
            }


            return list;
        }
    }
}
