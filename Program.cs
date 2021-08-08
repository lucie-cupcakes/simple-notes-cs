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
                noteList = await NoteList.Load(dbHandle);
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
                        noteList.Value[note.Id] = note.Title;
                        await noteList.Save(dbHandle);
                        Console.WriteLine("Note saved sucessfully!");
                    } catch(Exception e) {
                        Console.WriteLine(e.ToString());
                    }
                } else if (cmd.StartsWith("del")) {
                    Console.WriteLine("TODO: Delete command");
                    // To delete a registry you must find a way to make
                    // and httpClient DELETE request. Insted of Get,Post.
                } else if (cmd.StartsWith("mod")) {
                    Console.WriteLine("TODO: Modify command");
                    // To modify a note basically copy the part of Load Note
                    // in the print command, and call note.modify()
                    // Don't forget to update the title inside noteList
                    // And call note.Save() and noteList.Save()
                } else if (cmd.StartsWith("list")) {
                    foreach (var kv in noteList.Value) {
                        Console.WriteLine(kv.Key.ToString() + "\t" + kv.Value);
                    }
                } else if (cmd.StartsWith("print")) {
                    var cmdArr = cmd.Split("print", 2);
                    if (cmdArr.Length == 2) {
                        var guidStr = cmdArr[1].Trim();
                        var guid = Guid.NewGuid();
                        try {
                            guid = new Guid(guidStr);
                        } catch {}
                        if (noteList.Value.ContainsKey(guid)) {
                            try {
                                var note = await Note.Load(dbHandle, guid);
                                Console.WriteLine("Title:\t" + note.Title);
                                Console.WriteLine("Creation Time:\t" + note.CreationTime.ToString());
                                Console.WriteLine("Last Modified:\t" + note.LastModified.ToString());
                                Console.WriteLine(Environment.NewLine + note.Contents);
                            } catch (Exception e){
                                Console.WriteLine(e.ToString());
                            }
                        } else {
                            Console.WriteLine("error: note does not exists.");
                        }
                    } else {
                        Console.WriteLine("Usage: print <Guid>");
                    }

                } else if (cmd.StartsWith("exit")) {
                    exit = true;
                } else {
                    Console.WriteLine("Command not found.");
                }
            }
        }
    }

}
