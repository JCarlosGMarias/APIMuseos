using System;
using System.Linq;
using APIMuseos.Services.Albums;
using APIMuseos.Services.Museums;

namespace APIMuseos
{
    // https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 1)
            {
                bool.TryParse(args[0], out bool IsRaw);

                
                MuseumsEx(IsRaw);
            }
            else
            {
                PhotosEx();
            }

            Console.ReadKey();
        }

        static void MuseumsEx(bool IsRaw)
        {
            var Service = new MuseumService();

            if (!IsRaw)
            {
                Service.SetRawStrategy();
            }

            Service.Execute();
        }

        static async void PhotosEx()
        {
            var Service = new AlbumService();

            await Service.FetchData();

            Console.WriteLine($"Starting LINQ queries...{Environment.NewLine}");

            Service.CommentsForSinglePost(1);
            Service.PhotosInAlbums();
            Service.SortedDownUserNames();
            Service.Top5MostPopulatedAlbums();
            Service.CommentsPerUser(3);
            Service.MostCommentedUser();
            Service.TaskStatusesPerUser();
            Service.MostDelayedUser();
            Service.ImagesFromMostCollaborativeUser();
        }
    }
}
