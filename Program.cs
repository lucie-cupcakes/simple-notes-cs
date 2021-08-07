using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace pepino_cs
{
    class DBConfig {
        public string URL;
        public string Password;
        public string DBName;

        public DBConfig() {
            this.URL = "http://localhost:50200";
            this.Password = "caipiroska";
            this.DBName = "Notes";
        }

        public Uri GetURIForEntry(string entryName) {
            var uriSb = new StringBuilder();
            uriSb.Append(this.URL + "/?password=" + this.Password + "&db=");
            uriSb.Append(this.DBName + "&entry=" + entryName);
            return new Uri(uriSb.ToString());
        }

        public async Task<string> SaveEntry<T>(T instance, Guid id, HttpClient httpClient) {
            try {
                var uri = this.GetURIForEntry(id.ToString());
                var jsonData = new StringContent(JsonSerializer.Serialize(instance), Encoding.UTF8);
                HttpResponseMessage res = await httpClient.PostAsync(uri, jsonData);
                var resBody = await res.Content.ReadAsStringAsync();
                if (res.StatusCode == HttpStatusCode.OK) {
                    return "";
                }
                var msg = "There was an error on the Database:@@httpStatus={#1}";
                msg = msg + "@@" + resBody;
                msg = msg.Replace("@@", Environment.NewLine);
                msg = msg.Replace("{#1}", res.StatusCode.ToString());
                throw new Exception(msg);
            } catch (Exception e) {
                return e.ToString();
            }
        }
    }

    class Note {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModified { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }

        public Note(string contents, string title) {
            this.CreationTime = DateTime.UtcNow;
            this.LastModified = this.CreationTime;
            this.Id = Guid.NewGuid();
            this.Contents = contents;
            this.Title = title;
        }

        public void Modify(string newContents, string newTitle) {
            this.LastModified = DateTime.UtcNow;
            this.Title = newTitle;
            this.Contents = newContents;
        }

        public async Task<string> Save(DBConfig dbConfig, HttpClient client) {
            return await dbConfig.SaveEntry<Note>(this, this.Id, client);
        }
    }

    class Program {
        private static readonly HttpClient client = new HttpClient();
        private static readonly DBConfig dbConfig = new DBConfig();

        static string ReadUntilFinish() {
            Console.WriteLine("TIP: type @finish@ to end the note.");
            Console.WriteLine("contents:");
            var contentsSb = new StringBuilder();
            bool keepReading = true;
            while (keepReading) {
                var line = Console.ReadLine();
                if (line.Trim().StartsWith("@finish@")) {
                    keepReading = false;
                } else {
                    contentsSb.AppendLine(line.TrimEnd());
                }
            }
            return contentsSb.ToString();
        }

        static async Task Main(string[] args) {
            bool exit = false;
            Console.WriteLine("Welcome to the Notes program!");
            while (!exit) {
                Console.WriteLine("Commands:"+Environment.NewLine+
                    "new-\tCreate a Note" +Environment.NewLine+
                    "del-\tDelete a Note" +Environment.NewLine+
                    "mod-\tModify a Note" +Environment.NewLine+
                    "list-\tList Notes" +Environment.NewLine+
                    "print-\tPrint a Note to the screen"+Environment.NewLine+
                    "exit-\tLeave the program.");
                Console.Write("Notes> ");
                string cmd = Console.ReadLine();
                if (cmd.StartsWith("new")) {
                    try {
                        Console.Write("title: ");
                        var noteTitle = Console.ReadLine().Trim();
                        var noteContents = ReadUntilFinish();
                        var note = new Note(noteContents, noteTitle);
                        await note.Save(dbConfig, client);
                    } catch (Exception e) {
                        Console.WriteLine(e);
                    }
                } else if (cmd.StartsWith("del")) {
                    Console.WriteLine("TODO: Delete command");
                } else if (cmd.StartsWith("mod")) {
                    Console.WriteLine("TODO: Modify command");
                } else if (cmd.StartsWith("list")) {
                    Console.WriteLine("TODO: List command");
                } else if (cmd.StartsWith("print")) {
                    Console.WriteLine("TODO: Print command");
                } else if (cmd.StartsWith("exit")) {
                    exit = true;
                } else {
                    Console.WriteLine("Command not found.");
                }
            }
        }
    }

}
