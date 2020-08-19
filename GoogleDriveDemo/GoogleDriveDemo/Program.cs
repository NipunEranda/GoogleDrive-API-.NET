using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveDemo
{
    class Program
    {

        public static void Main(String[] args)
        {
            DriveApiService das = DriveApiService.GetInstance;
            das.getAuth();
            das.listFiles();
            //Employee GUID
            das.upload("C:\\Users\\BHANUKA\\Pictures\\158835.jpg", "10025", "12OfpW2BfTPZX_lc-wTrqa0jL-X_NAKTB");

            Console.Read();
        }
    }
}
