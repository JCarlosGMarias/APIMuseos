
using System.Threading.Tasks;

namespace APIMuseos.Services.Museums.Strategies
{
    public class DataContext
    {
        private IDataStrategy DataStrategy;

        #region Public Methods
        public async Task FetchData()
        {
            await this.DataStrategy.FetchData();
        }

        public void WatchMuseums()
        {
            this.DataStrategy.WatchMuseums();
        }

        public void WatchVisitedMuseums()
        {
            this.DataStrategy.WatchVisitedMuseums();
        }
        #endregion

        #region Public Methods: Strategy Methods
        public void SetStrategy(IDataStrategy Strategy)
        {
            this.DataStrategy = Strategy;
        }
        #endregion
    }
}
