using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleNotes {
    class NoteList {
        public Dictionary<Guid, String> Value { get; set; }

        public static async Task<NoteList> Load(PepinoDB.Database db) {
            byte[] jsonBytes = await db.LoadEntry("List");
            return JsonSerializer.Deserialize<NoteList>(jsonBytes);
        }

        public async Task Save(PepinoDB.Database db) {
            var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this));
            await db.SaveEntry("List", jsonBytes);
        }
    }
}
