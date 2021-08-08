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

        public async Task Save(PepinoDB.Database db) {
            var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this));
            await db.SaveEntry(this.Id.ToString(), jsonBytes);
        }
    }
}
