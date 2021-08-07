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
}
