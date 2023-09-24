using BusinessLayer.Helpers;
using BusinessLayer.Interfaces.Publishers;
using BusinessLayer.Models;

namespace BusinessLayer.Services;

public class PublisherService : IPublishersService
{
    private readonly IPublishersRepository _publisherRepository;

    public PublisherService(IPublishersRepository publishersRepository)
    {
        _publisherRepository = publishersRepository;
    }

    public async Task<bool> Contains(Guid publisherId)
    {
        return await _publisherRepository.Contains(publisherId);
    }

    public async Task<bool> IsPublisherNameExisting(string name)
    {
        return await _publisherRepository.IsPublisherNameExisting(name);
    }

    public async Task<Publisher> Create(Publisher publisher)
    {
        if (await _publisherRepository.IsPublisherNameExisting(publisher.Name))
            throw new AppException($"Publisher with this name: {publisher.Name} is already existing!");
        return await _publisherRepository.Create(publisher);
    }

    public async Task Delete(Guid publisherId)
    {
        if (!await _publisherRepository.Contains(publisherId))
            throw new KeyNotFoundException("Publisher cannot be found!");
        await _publisherRepository.Delete(publisherId);
    }

    public async Task<Publisher?> Get(Guid publisherId)
    {
        var publisher = await _publisherRepository.Get(publisherId);
        if (publisher is null)
            throw new KeyNotFoundException("Publisher cannot be found!");
        return publisher;
    }

    public async Task<PagedList<Publisher>> GetAll(PublisherParameters parameters)
    {
        return await _publisherRepository.GetAll(parameters);
    }

    public async Task<Publisher?> Update(Guid publisherId, Publisher publisher)
    {
        if (!await _publisherRepository.Contains(publisherId))
            throw new KeyNotFoundException("Publisher cannot be found!");
        if (await _publisherRepository.IsPublisherNameExisting(publisher.Name))
            throw new AppException($"Publisher with this name: {publisher.Name} is already existing!");

        return await _publisherRepository.Update(publisherId, publisher);
    }
}