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

namespace PepinoDB {
    class Database {
        private string url;
        private string password;
        private string dbName;

        public Database(string url, string dbName, string password) {
            this.url = url;
            this.dbName = dbName;
            this.password = password;
        }

        private Uri buildURIForEntry(string entryName) {
            var uriSb = new StringBuilder();
            uriSb.Append(this.url + "/?password=" + this.password + "&db=");
            uriSb.Append(this.dbName + "&entry=" + entryName);
            return new Uri(uriSb.ToString());
        }

        public async Task SaveEntry(string entryName, byte[] entryValue) {
            var httpClient = new HttpClient();
            var uri = this.buildURIForEntry(entryName);
            var httpData = new ByteArrayContent(entryValue);
            HttpResponseMessage res = await httpClient.PostAsync(uri, httpData);
            var resBody = await res.Content.ReadAsStringAsync();
            if (res.StatusCode == HttpStatusCode.OK) {
                return;
            }
            var msg = "There was an error on the Database:@@httpStatus={#1}";
            msg = msg + "@@" + resBody;
            msg = msg.Replace("@@", Environment.NewLine);
            msg = msg.Replace("{#1}", res.StatusCode.ToString());
            throw new Exception(msg);
        }

        public async Task<byte[]> LoadEntry(string entryName) {
            var httpClient = new HttpClient();
            var uri = this.buildURIForEntry(entryName);
            HttpResponseMessage res = await httpClient.GetAsync(uri);
            var resBody = await res.Content.ReadAsByteArrayAsync();
            if (res.StatusCode == HttpStatusCode.OK) {
                return resBody;
            }
            var msg = "There was an error on the Database:@@httpStatus={#1}";
            msg = msg + "@@" + Encoding.UTF8.GetString(resBody);
            msg = msg.Replace("@@", Environment.NewLine);
            msg = msg.Replace("{#1}", res.StatusCode.ToString());
            throw new Exception(msg);
        }

        public async Task DeleteEntry(string entryName) {
            var httpClient = new HttpClient();
            var uri = this.buildURIForEntry(entryName);
            HttpResponseMessage res = await httpClient.DeleteAsync(uri);
            var resBody = await res.Content.ReadAsByteArrayAsync();
            if (res.StatusCode == HttpStatusCode.OK) {
                return;
            }
            var msg = "There was an error on the Database:@@httpStatus={#1}";
            msg = msg + "@@" + Encoding.UTF8.GetString(resBody);
            msg = msg.Replace("@@", Environment.NewLine);
            msg = msg.Replace("{#1}", res.StatusCode.ToString());
            throw new Exception(msg);
        }
    }
}
