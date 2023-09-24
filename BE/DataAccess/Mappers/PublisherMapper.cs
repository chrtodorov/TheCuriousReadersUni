using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Entities;

namespace DataAccess.Mappers;

public static class PublisherMapper
{
    public static PublisherEntity ToPublisherEntity(this Publisher publisher)
    {
        return new PublisherEntity
        {
            Name = publisher.Name
        };
    }

    public static Publisher ToPublisher(this PublisherEntity publisherEntity)
    {
        return new Publisher
        {
            PublisherId = publisherEntity.PublisherId,
            Name = publisherEntity.Name
        };
    }

    public static Publisher ToPublisher(this PublisherRequest publisherCreateRequest)
    {
        return new Publisher
        {
            Name = publisherCreateRequest.Name.Trim()
        };
    }
}