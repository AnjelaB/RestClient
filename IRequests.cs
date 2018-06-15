using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient
{
    interface IRequests<T>
    {

        Task<IEnumerable<T>> GetAll(string nameOfSource, IEnumerable<KeyValuePair<string, string>> parameters = null);
        Task<T> GetById(string nameOfSource, int id);
        Task<HttpResponseMessage> InsertJson(string nameOfSource, T element);
        Task<HttpResponseMessage> InsertXml(string nameOfSource, T element);
        Task<HttpResponseMessage> UpdateJson(string nameOfSource, T element, int id);
        Task<HttpResponseMessage> UpdateXml(string nameOfSource, T element, int id);
        void Delete(string nameOfSource, int id);
        string AddParameter(string url, IEnumerable<KeyValuePair<string, string>> parameters);
    }
}
