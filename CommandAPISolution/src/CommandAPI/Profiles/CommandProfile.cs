using AutoMapper;
using CustomerAPI.Dtos;
using CustomerAPI.Models;

namespace CustomerAPI.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerReadDto>();
        }
    }
}