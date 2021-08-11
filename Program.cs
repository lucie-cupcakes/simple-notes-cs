using System;
using System.Collections.Generic;
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
            Console.WriteLine("TIP: write help for the command list.");
            while (!exit) {
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
                    var cmdArr = cmd.Split("del", 2);
                    if (cmdArr.Length == 2) {
                        var guidStr = cmdArr[1].Trim();
                        var guid = Guid.NewGuid();
                        try {
                            guid = new Guid(guidStr);
                        } catch {}
                        if (noteList.Value.ContainsKey(guid)) {
                            try {
                                await dbHandle.DeleteEntry(guidStr);
                                if (!noteList.Value.Remove(guid)) {
                                    throw new Exception("Cannot remove note from list.");
                                }
                                await noteList.Save(dbHandle);
                            } catch (Exception e){
                                Console.WriteLine(e.ToString());
                            }
                        } else {
                            Console.WriteLine("error: note does not exists.");
                        }
                    } else {
                        Console.WriteLine("Usage: del <Guid>");
                    }
                } else if (cmd.StartsWith("mod")) {
                    var cmdArr = cmd.Split("mod", 2);
                    try {
                        if (cmdArr.Length == 2) {
                            var guidStr = cmdArr[1].Trim();
                            if (! noteList.ContainsKey(guidStr)) {
                                throw new Exception("error: note does not exists.");
                            }
                            var guid = new Guid(guidStr);
                            var note = await Note.Load(dbHandle, guid);
                            Console.WriteLine(note.ToString());
                            Console.Write("title: ");
                            var noteTitle = Console.ReadLine().Trim();
                            var noteContents = ReadUntilFinish();
                            note.Modify(noteContents, noteTitle);
                            await note.Save(dbHandle);
                            noteList.Value[guid] = note.Title;
                            await noteList.Save(dbHandle);
                        } else {
                            Console.WriteLine("Usage: mod <Guid>");
                        }
                    } catch(Exception e) {
                        Console.WriteLine(e.ToString());
                    }
                } else if (cmd.StartsWith("list")) {
                    if (noteList.Value.Keys.Count > 0) {
                    foreach (var kv in noteList.Value) {
                        Console.WriteLine(kv.Key.ToString() + "\t" + kv.Value);
                    }
                    } else {
                        Console.WriteLine("There are not saved notes");
                    }
                } else if (cmd.StartsWith("print")) {
                    var cmdArr = cmd.Split("print", 2);
                    if (cmdArr.Length == 2) {
                        var guidStr = cmdArr[1].Trim();
                        try {
                            if (! noteList.ContainsKey(guidStr)) {
                                throw new Exception("error: note does not exists.");
                            }
                            var note = await Note.Load(dbHandle, new Guid(guidStr));
                            Console.WriteLine(note.ToString());
                        } catch (Exception e){
                            Console.WriteLine(e.ToString());
                        }
                    } else {
                        Console.WriteLine("Usage: print <Guid>");
                    }
                } else if (cmd.StartsWith("help")) {
                    Console.WriteLine(cmdHelp);
                } else if (cmd.StartsWith("exit")) {
                    exit = true;
                } else {
                    Console.WriteLine("Command not found.");
                }
            }
        }
    }

}
