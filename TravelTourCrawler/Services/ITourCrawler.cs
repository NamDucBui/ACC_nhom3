using TravelTourCrawler.Models;

namespace TravelTourCrawler.Services
{
    public interface ITourCrawler
    {
        Task<List<Tour>> CrawlToursAsync();

    }
}
