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
            var dbHandle = new PepinoDB.Database("http://localhost:50200", "Notes", "caipiroska");
            var cmdHelp = ("Commands: @@"+
                "new-\tCreate a Note@@"+
                "del-\tDelete a Note@@"+
                "mod-\tModify a Note@@"+
                "list-\tList Notes@@"+
                "print-\tPrint a Note to the screen@@"+
                "exit-\tLeave the program.").Replace("@@", Environment.NewLine);
            NoteList noteList;

            try {
                noteList = await NoteList.Load(dbHandle)
            } catch (Exception e) {
                noteList = new NoteList();
            }

            bool exit = false;
            Console.WriteLine("Welcome to the Notes program!");
            while (!exit) {
                Console.WriteLine(cmdHelp);
                Console.Write("Notes> ");
                string cmd = Console.ReadLine();
                if (cmd.StartsWith("new")) {
                    Console.Write("title: ");
                    var noteTitle = Console.ReadLine().Trim();
                    var noteContents = ReadUntilFinish();
                    var note = new Note(noteContents, noteTitle);
                    try {
                        await note.Save(dbHandle);
                        noteList[note.Id] = note.Title;
                        await noteList.Save(dbHandle);
                        Console.WriteLine("Note saved sucessfully!");
                    } catch(Exception e) {
                        Console.WriteLine(e.ToString());
                    }
                } else if (cmd.StartsWith("del")) {
                    Console.WriteLine("TODO: Delete command");
                } else if (cmd.StartsWith("mod")) {
                    Console.WriteLine("TODO: Modify command");
                } else if (cmd.StartsWith("list")) {
                    foreach (var kv in noteList.Value) {
                        Console.WriteLine(kv.Key.ToString() + "\t" + kv.Value);
                    }
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
