using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomerAPI.Data;
using CustomerAPI.Models;
using AutoMapper;
using CustomerAPI.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerAPIRepo _repository;
        private readonly IMapper _mapper;
        private IMemoryCache _cache;
        public CustomersController(ICustomerAPIRepo repository, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CustomerReadDto>> GetAllCustomers()
        {
            var CustomerItems = _repository.GetAllCustomers();

            return Ok(_mapper.Map<IEnumerable<CustomerReadDto>>(CustomerItems));
        }

        [HttpGet("{id}", Name="GetCustomerById")]
        public ActionResult<CustomerReadDto> GetCustomerById(int id)
        {

            if (!_cache.TryGetValue(id, out Customer CustomerItem))
            {
                CustomerItem = _repository.GetCustomerById(id);

                if (CustomerItem == null)
                    return NotFound();

                var cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(100),
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(100)
                };

                _cache.Set(id, CustomerItem, cacheExpirationOptions);
            }

            return Ok(_mapper.Map<CustomerReadDto>(CustomerItem));
        }
    }
}