using APIMuseos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIMuseos.Services.Museums.Strategies
{
    public class POCOStrategy : IDataStrategy
    {
        #region Private Attributes
        private MuseumService Reference;
        #endregion

        #region Constructor
        public POCOStrategy(MuseumService Reference)
        {
            this.Reference = Reference;
        }
        #endregion

        #region Public Methods: IDataStrategy
        public async Task FetchData()
        {
            Reference.Museums = new List<Museo>();

            using (var Client = new HttpClient())
            {
                using (var response = await Client.GetAsync(new Uri(ConfigurationManager.AppSettings["MuseosAPI"])))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        #region Método 1: Mapeo a POCOs
                        Reference.Museums = JsonConvert.DeserializeObject<List<Museo>>(content);
                        #endregion
                    }
                }
            }
        }

        public void WatchMuseums()
        {
            Console.WriteLine("All museums (POCO):");
            foreach (var Museum in from m in Reference.Museums select m)
            {
                Console.WriteLine($"- {Museum.Titulo}: {Museum.Descripcion}");
            }
        }

        public void WatchVisitedMuseums()
        {
            Console.WriteLine($"Visited museums (POCO):");
            foreach (var VisitedMuseum in from m in Reference.Museums where "S".Equals(m.Visita) select m)
            {
                Console.WriteLine($"- {VisitedMuseum.Titulo}");
            }
        }
        #endregion
    }
}
