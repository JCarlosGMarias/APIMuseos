using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIMuseos.Models
{
    /// <summary>
    /// Contenido de la API https://museowebapp.azurewebsites.net/api/MuseosAPI
    /// </summary>
    public class Museo
    {
        public int Id { get; set; }
        public string Coordenadas { get; set; }
        public string Datacion { get; set; }
        public string Descripcion { get; set; }
        public string Titulo { get; set; }
        public string Imagen { get; set; }
    }
}
