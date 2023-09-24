using BusinessLayer.Models;

namespace BusinessLayer.Interfaces.Publishers;

public interface IPublishersRepository
{
    Task<Publisher?> Get(Guid publisherId);
    Task<PagedList<Publisher>> GetAll(PublisherParameters parameters);
    Task<Publisher> Create(Publisher publisher);
    Task<Publisher?> Update(Guid publisherId, Publisher publisher);
    Task Delete(Guid publisherId);
    Task<bool> Contains(Guid publisherId);
    Task<bool> IsPublisherNameExisting(string name);
}