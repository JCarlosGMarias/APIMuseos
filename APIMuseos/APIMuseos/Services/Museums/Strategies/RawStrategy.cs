using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIMuseos.Services.Museums.Strategies
{
    public class RawStrategy : IDataStrategy
    {
        #region Private Attributes
        private MuseumService Service;
        #endregion

        #region Constructor
        public RawStrategy(MuseumService Service)
        {
            this.Service = Service;
        }
        #endregion

        #region Public Methods: IDataStrategy
        public async Task FetchData()
        {
            this.Service.MuseumArray = new JArray();

            using (var Client = new HttpClient())
            {
                using (var response = await Client.GetAsync(new Uri(ConfigurationManager.AppSettings["MuseosAPI"])))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        #region Método 2: Parseo directo
                        this.Service.MuseumArray = JArray.Parse(content);
                        #endregion
                    }
                }
            }
        }

        public void WatchMuseums()
        {
            Console.WriteLine("All museums (Raw):");
            foreach (var Museum in from m in this.Service.MuseumArray select m)
            {
                Console.WriteLine($"- {Museum["titulo"]}: {Museum["descripcion"]}");
            }
        }

        public void WatchVisitedMuseums()
        {
            Console.WriteLine($"Visited museums (Raw):");
            foreach (var VisitedMuseum in from m in this.Service.MuseumArray where "S".Equals((string)m["visita"]) select m)
            {
                Console.WriteLine($"- {VisitedMuseum["titulo"]}");
            }
        }
        #endregion
    }
}
