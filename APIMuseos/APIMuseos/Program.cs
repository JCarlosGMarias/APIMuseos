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
            //leeMuseos();

            PhotosEx();

            Console.ReadLine();
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
                        //JArray Array = JArray.Parse(content);
                        //var visitasS = from o in Array
                        //               where p => p["visitas"] == "S"
                        //               select o;
                    }
                }
            }
        }

        static async void PhotosEx()
        {
            // Usando jsonplaceholder.typicode.com
            var Root = new Uri(@"https://jsonplaceholder.typicode.com");

            // Crear las colecciones con los datos.
            var Posts = await FetchTo<Post>($"{Root}posts");
            var Comments = await FetchTo<Comment>($"{Root}comments");
            var Albums = await FetchTo<Album>($"{Root}albums");
            var Photos = await FetchTo<Photo>($"{Root}photos");
            var Todos = await FetchTo<Todo>($"{Root}todos");
            var Users = await FetchTo<User>($"{Root}users");

            // Ver cuáles son los comentarios que hay para un post determinado
            var PostID = 1;

            var CommentsForSinglePost = from c in Comments
                                        where c.postId == (from p in Posts where p.id == PostID select p).First().id
                                        select c;

            Console.WriteLine($"Comments for post {PostID}:{Environment.NewLine}");
            foreach (var Comment in CommentsForSinglePost)
            {
                Console.WriteLine($"- {Comment.name} ({Comment.email}) -> {Comment.body}");
            }
            Console.WriteLine("");

            // Ver cuáles son los álbums que hay y la cantidad de fotos que contiene cada uno de ellos
            var PhotosInAlbums = from p in Photos
                                 join a in Albums on p.albumId equals a.id
                                 group p by a.title into photos
                                 orderby photos.Key
                                 select photos;

            Console.WriteLine($"Comments for post {PostID}:{Environment.NewLine}");
            foreach (var Album in PhotosInAlbums)
            {
                Console.WriteLine($"- {Album.Key}:");
                foreach (var Photo in Album)
                {
                    Console.WriteLine($"  * {Photo.title}: {Photo.thumbnailUrl}");

                }
            }
            Console.WriteLine("");

            // Ver cuáles son los nombres de los usuarios ordenados descendentemente
            var UserNames = from u in Users
                            orderby u.name descending
                            select u.name;

            Console.WriteLine($"Sorted down user names:");
            foreach (var UserName in UserNames)
            {
                Console.WriteLine($"- {UserName}");
            }
            Console.WriteLine("");

            // Tomar los cinco álbums donde más fotos existan


            // Ver qué cantidad de comentarios totales ha recibido un usuario


            // Mostrar el usuario que más comentarios ha recibido


            // Mostrar para cada usuario cuántas tareas tiene terminadas y cuáles no


            // Mostrar cuál es el usuario que más tareas tiene por finalizar


            // Mostrar todas las urls de las imágenes del usuario que más fotos ha subido
        }

        static async Task<List<T>> FetchTo<T>(string Resource) where T : class
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
