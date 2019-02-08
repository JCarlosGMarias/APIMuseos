using APIMuseos.Models;
using APIMuseos.Services.Museums.Strategies;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIMuseos.Services.Museums
{
    public class MuseumService
    {
        #region Private Attributes
        private DataContext Context;
        #endregion

        #region Public Properties
        public JArray MuseumArray { get; set; }

        public List<Museo> Museums { get; set; }
        #endregion

        #region Constructor
        public MuseumService(DataContext Context = null)
        {
            this.Context = Context ?? new DataContext();
            this.Context.SetStrategy(new POCOStrategy(this));
        }
        #endregion

        #region Public Methods
        public async void Execute()
        {
            await this.FetchData();
            this.WatchMuseums();
            this.WatchVisitedMuseums();
        }

        public void SetPOCOStrategy()
        {
            this.Context.SetStrategy(new POCOStrategy(this));
        }

        public void SetRawStrategy()
        {
            this.Context.SetStrategy(new RawStrategy(this));
        }
        #endregion

        #region Private Methods
        private async Task FetchData()
        {
            await this.Context.FetchData();
        }

        private void WatchMuseums()
        {
            this.Context.WatchMuseums();
        }

        private void WatchVisitedMuseums()
        {
            this.Context.WatchVisitedMuseums();
        }
        #endregion
    }
}
