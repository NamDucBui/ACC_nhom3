using TravelTourCrawler.Models;

namespace TravelTourCrawler.Services
{
    public interface ITourCrawler
    {
        string Source { get; }
        Task<List<Tour>> CrawlToursAsync(string url);

    }
}
