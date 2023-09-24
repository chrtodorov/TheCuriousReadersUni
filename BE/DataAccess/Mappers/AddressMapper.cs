using BusinessLayer.Models;
using DataAccess.Entities;

namespace DataAccess.Mappers
{
    public static class AddressMapper
    {
        public static AddressEntity ToAddressEntity(this Address address)
        {
            return new AddressEntity
            {
                Country = address.Country,
                City = address.City,
                Street = address.Street,
                StreetNumber = address.StreetNumber,
                BuildingNumber = address.BuildingNumber,
                ApartmentNumber = address.ApartmentNumber,
                AdditionalInfo = address.AdditionalInfo,
            };
        }
        public static Address ToAddress(this AddressEntity addressEntity)
        {
            return new Address
            {
                Country = addressEntity.Country,
                City = addressEntity.City,
                Street = addressEntity.Street,
                StreetNumber = addressEntity.StreetNumber,
                BuildingNumber = addressEntity.BuildingNumber,
                ApartmentNumber = addressEntity.ApartmentNumber,
                AdditionalInfo = addressEntity.AdditionalInfo,
            };
        }
    }
}