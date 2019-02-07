using APIMuseos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIMuseos
{
    // https://museowebapp.azurewebsites.net/api/MuseosAPI
    // https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm

    class Program
    {
        static void Main(string[] args)
        {
            //leeMuseos();
            //leeMuseosVisitas();
            PhotosEx();

            Console.ReadKey();
        }

        static async void leeMuseos()
        {
            using (var Client = new HttpClient())
            {
                List<Museo> ListaMuseos = new List<Museo>();

                using (var response = await Client.GetAsync(new Uri(@"https://museowebapp.azurewebsites.net/api/MuseosAPI")))
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
            using (var Client = new HttpClient())
            {
                var ListaMuseos = new List<Museo>();

                using (var response = await Client.GetAsync(new Uri("https://museowebapp.azurewebsites.net/api/MuseosAPI")))
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
            Console.WriteLine($"API URL -> {Root}. Fetching resources...");

            #region Crear las colecciones con los datos.
            List<Post> Posts;
            List<Comment> Comments;
            List<Album> Albums;
            List<Photo> Photos;
            List<Todo> Todos;
            List<User> Users;

            using (var Client = new HttpClient())
            {
                Posts = await FetchTo<Post>(Client, $"{Root}posts");
                Comments = await FetchTo<Comment>(Client, $"{Root}comments");
                Albums = await FetchTo<Album>(Client, $"{Root}albums");
                Photos = await FetchTo<Photo>(Client, $"{Root}photos");
                Todos = await FetchTo<Todo>(Client, $"{Root}todos");
                Users = await FetchTo<User>(Client, $"{Root}users");
            }
            #endregion

            Console.WriteLine($"Starting LINQ queries...{Environment.NewLine}");

            #region 1: Ver cuáles son los comentarios que hay para un post determinado
            var PostID = 1;

            var CommentsForSinglePost = from c in Comments
                                        where c.postId == (from p in Posts where p.id == PostID select p.id).FirstOrDefault()
                                        select c;

            Console.WriteLine($"Comments for post {PostID}:{Environment.NewLine}");
            foreach (var Comment in CommentsForSinglePost)
            {
                Console.WriteLine($"- {Comment.name} ({Comment.email}) -> {Comment.body}");
            }
            Console.WriteLine("");
            #endregion

            #region 2: Ver cuáles son los álbums que hay y la cantidad de fotos que contiene cada uno de ellos
            var PhotosInAlbums = from p in Photos
                                 join a in Albums on p.albumId equals a.id
                                 group p by a.title into photos
                                 orderby photos.Key
                                 select new { Title = photos.Key, Total = photos.Count() };

            Console.WriteLine($"Photos in each album:");
            foreach (var Album in PhotosInAlbums)
            {
                Console.WriteLine($"- {Album.Title}: {Album.Total}");
            }
            Console.WriteLine("");
            #endregion

            #region 3: Ver cuáles son los nombres de los usuarios ordenados descendentemente
            var UserNames = from u in Users
                            orderby u.name descending
                            select u.name;

            Console.WriteLine($"Sorted down user names:");
            foreach (var UserName in UserNames)
            {
                Console.WriteLine($"- {UserName}");
            }
            Console.WriteLine("");
            #endregion

            #region 4: Tomar los cinco álbums donde más fotos existan
            var Top5MostPopulatedAlbums = (from a in Albums
                                           join p in Photos on a.id equals p.albumId
                                           group p by a.title into photos
                                           orderby photos.Count(), photos.Key
                                           select new { Title = photos.Key, Total = photos.Count() })
                                          .Take(5);

            Console.WriteLine($"Top 5 Most Populated albums:");
            foreach (var Album in Top5MostPopulatedAlbums)
            {
                Console.WriteLine($"- {Album.Title}: {Album.Total}");
            }
            Console.WriteLine("");
            #endregion

            #region 5: Ver qué cantidad de comentarios totales ha recibido un usuario
            var UserID = 3;

            var CommentsPerUser = from u in Users
                                  join p in Posts on u.id equals p.userId
                                  join c in Comments on p.id equals c.postId
                                  where u.id == UserID
                                  group c by u.name into comments
                                  orderby comments.Count(), comments.Key
                                  select new { UserName = comments.Key, TotalComments = comments.Count() };

            Console.WriteLine($"Comments per user:");
            foreach (var User in CommentsPerUser)
            {
                Console.WriteLine($"- {User.UserName} - Comments = {User.TotalComments}");
            }
            Console.WriteLine("");
            #endregion

            #region 6: Mostrar el usuario que más comentarios ha recibido
            var MostCommentedUser = (from u in Users
                                     join p in Posts on u.id equals p.userId
                                     join c in Comments on p.id equals c.postId
                                     group c by u.name into comments
                                     orderby comments.Count() descending
                                     select new { UserName = comments.Key, Total = comments.Count() }).First();

            Console.WriteLine($"Most commented user:");
            Console.WriteLine($"- {MostCommentedUser.UserName} - Comments = {MostCommentedUser.Total}{Environment.NewLine}{Environment.NewLine}");
            #endregion

            #region 7: Mostrar para cada usuario cuántas tareas tiene terminadas y cuáles no
            var TaskStatusesPerUser = from u in Users
                                      select new
                                      {
                                          UserName = u.name,
                                          FinishedTasksCount = (from t in Todos where t.userId == u.id && t.completed select t).Count(),
                                          UnfinishedTasks = from t in Todos where t.userId == u.id && !t.completed select t
                                      };

            Console.WriteLine($"Todos status per user:");
            foreach (var User in TaskStatusesPerUser)
            {
                Console.WriteLine($"- User: {User.UserName} (Finished todos: {User.FinishedTasksCount}) (Remaining todos: {User.UnfinishedTasks.Count()})");
                foreach (var Todo in User.UnfinishedTasks)
                {
                    Console.WriteLine($"  * Todo: {Todo.title}");
                }
            }
            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
            #endregion

            #region 8: Mostrar cuál es el usuario que más tareas tiene por finalizar
            var MostDelayedUser = (from u in Users
                                   join t in Todos on u.id equals t.userId
                                   where !t.completed
                                   group t by u.name into users
                                   orderby users.Count() descending
                                   select new { UserName = users.Key, Total = users.Count() }).First();

            Console.WriteLine($"Most delayed user:");
            Console.WriteLine($"- {MostDelayedUser.UserName} - Todos = {MostDelayedUser.Total}{Environment.NewLine}{Environment.NewLine}");
            #endregion

            #region 9: Mostrar todas las urls de las imágenes del usuario que más fotos ha subido
            var ImagesFromMostCollaborativeUser = (from u in Users
                                                   join a in Albums on u.id equals a.userId
                                                   join p in Photos on a.id equals p.albumId
                                                   group p by u.name into photos
                                                   orderby photos.Count() descending
                                                   select new { UserName = photos.Key, Total = photos.Count(), Photos = photos.ToList() }).First();

            Console.WriteLine($"Photos from most collaborative user:");
            Console.WriteLine($"- User: {ImagesFromMostCollaborativeUser.UserName} (Total photos: {ImagesFromMostCollaborativeUser.Total})");
            foreach (var Photo in ImagesFromMostCollaborativeUser.Photos)
            {
                Console.WriteLine($"  * Url: {Photo.url}");
            }
            #endregion
        }

        static async Task<List<T>> FetchTo<T>(HttpClient Client, string Resource) where T : class
        {
            List<T> Result = new List<T>();

            using (var response = await Client.GetAsync(new Uri(Resource)))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Result = JsonConvert.DeserializeObject<List<T>>(content);
                }
            }

            return Result;
        }
    }
}
