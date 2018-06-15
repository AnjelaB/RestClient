using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace RestClient
{
    /// <summary>
    /// Rest Client class
    /// </summary>
    /// <typeparam name="T">T type of source.</typeparam>
    public class RestClient<T>:IRequests<T> where T:class
    {
        /// <summary>
        /// HttpClient for what will be called the  methods.
        /// </summary>
        HttpClient Client { get; }

        /// <summary>
        /// Uri to sand a request.
        /// </summary>
        string Uri { get; set; }

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public RestClient()
        {
            this.Client = new HttpClient();
        }

        /// <summary>
        /// Method to find out the string either json type or not.
        /// </summary>
        /// <param name="jsonData">The string that is either json type or not.</param>
        public static bool IsJson(string jsonData)
        {
            return jsonData.Trim().Substring(0, 1).IndexOfAny(new[] { '[', '{' }) == 0;
        }

        /// <summary>
        /// Method to get all objects of the source.
        /// </summary>
        /// <param name="nameOfSource">Name of the source.(Example "Products")</param>
        /// <param name="parameters">Parameters if it's necessery.</param>
        public async Task<IEnumerable<T>> GetAll(string nameOfSource, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            
            if (this.Uri != null)
            {
                var uri = Uri + "api/" + nameOfSource;
                uri = AddParameter(uri, parameters);
                var elements =await Client.GetStringAsync(uri);
                if (IsJson(elements))
                {
                    return JsonConvert.DeserializeObject<T[]>(elements);
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(elements));
                    return (T[])serializer.Deserialize(memStream);
                }
                
            }

            throw new Exception("Uri is not defined.");
        }

        /// <summary>
        /// Method to get object of the source by given id.
        /// </summary>
        /// <param name="nameOfSource">Name of the source.(Example "Products")</param>
        /// <param name="id">Id of the necessery object.</param>
        public async Task<T> GetById(string nameOfSource,int id)
        {
            if (this.Uri != null)
            {
                var elements = await Client.GetStringAsync(Uri + "api/" + nameOfSource + "/" + id);
                if (IsJson(elements))
                {
                    return  JsonConvert.DeserializeObject<T>(elements);
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(elements));
                    return (T)serializer.Deserialize(memStream);
                }

            }

            throw new Exception("Uri is not defined.");
        }

        /// <summary>
        /// Method to insert an object to the source where MediaType is "application/json".
        /// </summary>
        /// <param name="nameOfSource"> Name of the source.(Example "Products") </param>
        /// <param name="element"> Object that will be inserted. </param>
        public async Task<HttpResponseMessage> InsertJson(string nameOfSource,T element)
        {
            
            var json = JsonConvert.SerializeObject(element);
            if (Uri != null)
            {
                return await Client.PostAsync(Uri + "api/" + nameOfSource, new StringContent(json, Encoding.UTF8, "application/json"));
            }
            throw new Exception("Uri is not defined.");
        }

        /// <summary>
        /// Method to insert an object to the source where MediaType is "application/json".
        /// </summary>
        /// <param name="nameOfSource"> Name of the source.(Example "Products") </param>
        /// <param name="element"> Object that will be inserted. </param>
        /// <returns></returns>
        public  async Task<HttpResponseMessage> InsertXml(string nameOfSource,T element)
        {
            var serializer = new XmlSerializer(element.GetType());
            var result = new StringBuilder();
            if (Uri != null)
            {
                using (var writer = XmlWriter.Create(result))
                {
                    serializer.Serialize(writer, element);
                }

                return await Client.PostAsync(Uri + "api/" + nameOfSource, new StringContent(result.ToString(), Encoding.UTF8, "application/xml"));
            }

            throw new Exception("Uri is not defined.");
        }

        /// <summary>
        /// Method to update the object with given id where MediaType is "application/json".
        /// </summary>
        /// <param name="nameOfSource"> Name of the source.(Example "Products") </param>
        /// <param name="element"> The object that will be put. </param>
        /// <param name="id"> The id of the object that will be updated. </param>
        public async Task<HttpResponseMessage> UpdateJson(string nameOfSource,T element,int id)
        {
            var json = JsonConvert.SerializeObject(element);
            if (Uri != null)
            {
                return await Client.PutAsync(Uri + "api/" + nameOfSource + "/" + id, new StringContent(json, Encoding.UTF8, "application/json"));
            }
            throw new Exception("Uri is not defined.");
        }

        /// <summary>
        /// Method to update the object with given id where MediaType is "application/xml".
        /// </summary>
        /// <param name="nameOfSource"> Name of the source.(Example "Products") </param>
        /// <param name="element"> The object that will be put. </param>
        /// <param name="id"> The id of the object that will be updated. </param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateXml(string nameOfSource, T element,int id)
        {
            var serializer = new XmlSerializer(element.GetType());
            var result = new StringBuilder();
            if (Uri != null)
            {
               using (var writer = XmlWriter.Create(result))
                {
                    serializer.Serialize(writer, element);
                }

                return await Client.PostAsync(Uri + "api/" + nameOfSource + "/" + id, new StringContent(result.ToString(), Encoding.UTF8, "application/xml"));
            }
            throw new Exception("Uri is not defined.");
        }

        /// <summary>
        /// Method to delete the object of the source with the given id.
        /// </summary>
        /// <param name="nameOfSource"> Name of the source.(Example "Products") </param>
        /// <param name="id"> The id of the  object, that will be deleted. </param>
        public async void Delete(string nameOfSource,int id)
        {
            if (Uri != null)
            {
                await Client.DeleteAsync(Uri + "api/" + nameOfSource + "/" + id);
            }

            throw new Exception("Uri is not defined.");
        }

        /// <summary>
        /// Method to add parameters to the url.
        /// </summary>
        public string AddParameter(string url, IEnumerable<KeyValuePair<string,string>>  parameters)
        {
            if (parameters!=null)
            {
                url += "?";
                bool current = true;
                var enumerator = parameters.GetEnumerator();
                enumerator.MoveNext();
                while(current)
                {
                    url += enumerator.Current.Key + "=" + enumerator.Current.Value;
                    if (enumerator.MoveNext())
                    {
                        url += "&";
                    }
                    else
                    {
                        current = false;
                    }
                }
            }

            return url;
        }
    }
}
