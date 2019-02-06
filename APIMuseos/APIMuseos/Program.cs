using APIMuseos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIMuseos
{

//    https://museowebapp.azurewebsites.net/api/MuseosAPI

//https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm
    class Museo /*Contenido de la API https://museowebapp.azurewebsites.net/api/MuseosAPI */
    {
        public int Id { get; set; }
        public string Coordenadas { get; set; }
        public string Datacion { get; set; }
        public string Descripcion { get; set; }
        public string Titulo { get; set; }
        public string Imagen { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            leeMuseos();
            Console.ReadLine();

            PhotosEx();
            
        }

        static async void leeMuseos()
        {
            List<Museo> ListaMuseos = new List<Museo>();

            using (var Clientes = new HttpClient())
            {
                var uri = new Uri(@"https://museowebapp.azurewebsites.net/api/MuseosAPI");

                using (var response = await Clientes.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        ListaMuseos = JsonConvert.DeserializeObject<List<Museo>>(content);

                        foreach (var mus in ListaMuseos)
                        {
                            Console.WriteLine(mus.Descripcion);
                        }
                    }
                }
            }
        }

        static async void leeMuseosVisitas()
        {
            List<Museo> ListaMuseos = new List<Museo>();

            using (HttpClient Clientes = new HttpClient())
            {
                string url = @"https://museowebapp.azurewebsites.net/api/MuseosAPI";
                var uri = new Uri(url);

                using (var response = await Clientes.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        JArray Array = JArray.Parse(content);
                        var visitasS = from o in Array
                                       where (p => p["visitas"].Equals("S"))
                                       select o;
                    }
                }
            }
        }

        static async void PhotosEx()
        {
            //usando jsonplaceholder.typicode.com
            var Root = new Uri(@"https://jsonplaceholder.typicode.com");

            var Posts = await FetchFrom<Post>(Root + "/posts");
            var Comments = await FetchFrom<Comment>(Root + "/comments");
            var Albums = await FetchFrom<Album>(Root + "/albums");
            var Photos = await FetchFrom<Photo>(Root + "/photos");
            var Todos = await FetchFrom<Todo>(Root + "/todos");
            var Users = new List<User>();

            

            //Crear las colecciones con los datos.
            var Photos =

            //Ver cuáles son los comentarios que hay para un post determinado


            //Ver cuáles son los álbums que hay y la cantidad de fotos que contiene cada uno de eellos


            //Ver cuáles son los nombres de los usuarios ordenados descendentemente


            //Tomar los cinco álbums donde más fotos existan


            //Ver qué cantidad de comentarios totales ha recibido un usuario


            //Mostrar el usuario que más comentarios ha recibido


            //Mostrar para cada usuario cuántas tareas tiene terminadas y cuáles no


            //Mostrar cuál es el usuario que más tareas tiene por finalizar


            //Mostrar todas las urls de las imágenes del usuario que más fotos ha subido
        }

        static async Task<List<T>> FetchFrom<T>(string Resource) where T : class
        {
            List<T> Result = new List<T>();

            using (var Client = new HttpClient())
            {
                using (var response = await Client.GetAsync(new Uri(Resource)))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Result = JsonConvert.DeserializeObject<List<T>>(content);
                    }
                }
            }

            return Result;
        }
    }
}
