using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
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
    public sealed class DriveApiService
    {

        private string[] Scopes = { DriveService.Scope.Drive };
        private string ApplicationName = "Genext-Employee-IMG";
        private FilesResource.ListRequest listRequest;
        private static readonly object Instancelock = new object();
        private static int counter = 0;
        private static DriveApiService instance = null;
        DriveService service;

        private DriveApiService() {
            counter++;
        }

        public static DriveApiService GetInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (Instancelock)
                    {
                        if (instance == null)
                        {
                            instance = new DriveApiService();
                        }
                    }
                }
                return instance;
            }
        }

        public void getAuth() {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None/*,
                    new FileDataStore(credPath, true)*/).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";
            
        }

        public void listFiles() {
            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }

        public Google.Apis.Drive.v3.Data.File CreateFolder(string name, string id = "root")
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new[] { id }
            };

            var request = service.Files.Create(fileMetadata);
            request.Fields = "id, name, parents, createdTime, modifiedTime, mimeType";

            return request.Execute();
        }

        public void upload(string imagePath, string employeeId, string id = "root")
        {
            Google.Apis.Drive.v3.Data.File fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = employeeId,
                Parents = new[] { id }
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(imagePath,
                                    System.IO.FileMode.Open))
            {
                request = service.Files.Create(
                    fileMetadata, stream, "image/jpg");
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
            Console.WriteLine(file.Id);
        }

        public async Task<Stream> Download(string fileId)
        {
                Stream outputstream = new MemoryStream();
                var request = service.Files.Get(fileId);

                await request.DownloadAsync(outputstream);

            outputstream.Position = 0;

                return outputstream;
        }

    }
}
