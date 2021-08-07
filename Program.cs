using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SimpleNotes {
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
