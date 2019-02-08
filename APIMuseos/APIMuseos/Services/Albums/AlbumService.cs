using APIMuseos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APIMuseos.Services.Albums
{
    public class AlbumService
    {
        #region Public Properties
        public List<Post> Posts { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Album> Albums { get; set; }

        public List<Photo> Photos { get; set; }

        public List<Todo> Todos { get; set; }

        public List<User> Users { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Crear las colecciones con los datos
        /// </summary>
        /// <returns></returns>
        public async Task FetchData()
        {
            using (var Client = new HttpClient())
            {
                // Usando jsonplaceholder.typicode.com
                var Root = new Uri(ConfigurationManager.AppSettings["JsonPlaceholder"]);
                Console.WriteLine($"API URL -> {Root}. Fetching resources...");

                Posts = await FetchTo<Post>(Client, $"{Root}posts");
                Comments = await FetchTo<Comment>(Client, $"{Root}comments");
                Albums = await FetchTo<Album>(Client, $"{Root}albums");
                Photos = await FetchTo<Photo>(Client, $"{Root}photos");
                Todos = await FetchTo<Todo>(Client, $"{Root}todos");
                Users = await FetchTo<User>(Client, $"{Root}users");
            }
        }

        /// <summary>
        /// 1: Ver cuáles son los comentarios que hay para un post determinado
        /// </summary>
        /// <param name="PostID">ID del post a examinar.</param>
        public void CommentsForSinglePost(int PostID)
        {
            var Result = from c in Comments
                         where c.postId == (from p in Posts where p.id == PostID select p.id).FirstOrDefault()
                         select c;

            Console.WriteLine($"Comments for post {PostID}:{Environment.NewLine}");
            foreach (Comment Comment in Result)
            {
                Console.WriteLine($"- {Comment.name} ({Comment.email}) -> {Comment.body}");
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// 2: Ver cuáles son los álbums que hay y la cantidad de fotos que contiene cada uno de ellos
        /// </summary>
        public void PhotosInAlbums()
        {
            var Result = from p in Photos
                         join a in Albums on p.albumId equals a.id
                         group p by a.title into photos
                         orderby photos.Key
                         select new { Title = photos.Key, Total = photos.Count() };

            Console.WriteLine($"Photos in each album:");
            foreach (var Album in Result)
            {
                Console.WriteLine($"- {Album.Title}: {Album.Total}");
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// 3: Ver cuáles son los nombres de los usuarios ordenados descendentemente
        /// </summary>
        public void SortedDownUserNames()
        {
            var UserNames = from u in Users
                            orderby u.name descending
                            select u.name;

            Console.WriteLine($"Sorted down user names:");
            foreach (string UserName in UserNames)
            {
                Console.WriteLine($"- {UserName}");
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// 4: Tomar los cinco álbums donde más fotos existan
        /// </summary>
        public void Top5MostPopulatedAlbums()
        {
            var Result = (from a in Albums
                          join p in Photos on a.id equals p.albumId
                          group p by a.title into photos
                          orderby photos.Count(), photos.Key
                          select new { Title = photos.Key, Total = photos.Count() }).Take(5);

            Console.WriteLine($"Top 5 Most Populated albums:");
            foreach (var Album in Result)
            {
                Console.WriteLine($"- {Album.Title}: {Album.Total}");
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// 5: Ver qué cantidad de comentarios totales ha recibido un usuario
        /// </summary>
        /// <param name="UserID">Usuario a examinar.</param>
        public void CommentsPerUser(int UserID)
        {
            var Result = from u in Users
                         join p in Posts on u.id equals p.userId
                         join c in Comments on p.id equals c.postId
                         where u.id == UserID
                         group c by u.name into comments
                         orderby comments.Count(), comments.Key
                         select new { UserName = comments.Key, TotalComments = comments.Count() };

            Console.WriteLine($"Comments per user:");
            foreach (var User in Result)
            {
                Console.WriteLine($"- {User.UserName} - Comments = {User.TotalComments}");
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// 6: Mostrar el usuario que más comentarios ha recibido
        /// </summary>
        public void MostCommentedUser()
        {
            var Result = (from u in Users
                          join p in Posts on u.id equals p.userId
                          join c in Comments on p.id equals c.postId
                          group c by u.name into comments
                          orderby comments.Count() descending
                          select new { UserName = comments.Key, Total = comments.Count() }).First();

            Console.WriteLine($"Most commented user:");
            Console.WriteLine($"- {Result.UserName} - Comments = {Result.Total}{Environment.NewLine}{Environment.NewLine}");
        }

        /// <summary>
        /// 7: Mostrar para cada usuario cuántas tareas tiene terminadas y cuáles no
        /// </summary>
        public void TaskStatusesPerUser()
        {
            var Result = from u in Users
                         select new
                         {
                             UserName = u.name,
                             FinishedTasksCount = (from t in Todos where t.userId == u.id && t.completed select t).Count(),
                             UnfinishedTasks = from t in Todos where t.userId == u.id && !t.completed select t
                         };

            Console.WriteLine($"Todos status per user:");
            foreach (var User in Result)
            {
                Console.WriteLine($"- User: {User.UserName} (Finished todos: {User.FinishedTasksCount}) (Remaining todos: {User.UnfinishedTasks.Count()})");
                foreach (Todo Todo in User.UnfinishedTasks)
                {
                    Console.WriteLine($"  * Todo: {Todo.title}");
                }
            }
            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
        }

        /// <summary>
        /// 8: Mostrar cuál es el usuario que más tareas tiene por finalizar
        /// </summary>
        public void MostDelayedUser()
        {
            var Result = (from u in Users
                          join t in Todos on u.id equals t.userId
                          where !t.completed
                          group t by u.name into users
                          orderby users.Count() descending
                          select new { UserName = users.Key, Total = users.Count() }).First();

            Console.WriteLine($"Most delayed user:");
            Console.WriteLine($"- {Result.UserName} - Todos = {Result.Total}{Environment.NewLine}{Environment.NewLine}");
        }

        /// <summary>
        /// 9: Mostrar todas las urls de las imágenes del usuario que más fotos ha subido
        /// </summary>
        public void ImagesFromMostCollaborativeUser()
        {
            var Result = (from u in Users
                          join a in Albums on u.id equals a.userId
                          join p in Photos on a.id equals p.albumId
                          group p by u.name into photos
                          orderby photos.Count() descending
                          select new { UserName = photos.Key, Total = photos.Count(), Photos = photos.ToList() }).First();

            Console.WriteLine($"Photos from most collaborative user:");
            Console.WriteLine($"- User: {Result.UserName} (Total photos: {Result.Total})");
            foreach (Photo Photo in Result.Photos)
            {
                Console.WriteLine($"  * Url: {Photo.url}");
            }
        }
        #endregion

        #region Private Methods
        private async Task<List<T>> FetchTo<T>(HttpClient Client, string Resource) where T : class
        {
            var Result = new List<T>();

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
        #endregion
    }
}
