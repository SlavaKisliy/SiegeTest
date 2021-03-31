using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomerAPI.Data;
using CustomerAPI.Models;
using AutoMapper;
using CustomerAPI.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Diagnostics;

namespace CustomerAPI.Controllers
{
    public class CacheItem<T>
    {
        public T Item {get; set;}
        public long TimeToComputeItem {get; set;}
    }

    [Route("api/[controller]")]
    [ApiController]
    public class NewCacheCustomersController : ControllerBase
    {
        private readonly ICustomerAPIRepo _repository;
        private readonly IMapper _mapper;
        private IMemoryCache _cache;
        public NewCacheCustomersController(ICustomerAPIRepo repository, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        /* Referense
            function x-fetch(key, ttl, beta=1) {
                value, delta, expiry ← cache_read(key)
                if (!value || time() − delta * beta * log(rand(0,1)) ≥ expiry) {
                    start ← time()
                    value ← recompute_value()
                    delta ← time() – start
                    cache_write(key, (value, delta), ttl)
                }
                return value
            }
        */

        public T GetFromCache<T>(Func<T> getFromRepoFunc, int id, DateTime expiration)
        {
            var valueInCache = _cache.TryGetValue(id, out CacheItem<T> item);

            var randExpirationTime = DateTime.Now;

            if (valueInCache)
            {
                var randFactor = new Random(DateTime.Now.Millisecond).NextDouble();

                randFactor = randFactor == 0.0 ? 1 : randFactor;

                var randTimeDelta = Convert.ToInt32(item.TimeToComputeItem * Math.Log(randFactor));

                randExpirationTime = DateTime.Now.AddMilliseconds(-randTimeDelta);
            }

            if (!valueInCache || randExpirationTime >= expiration)
            {
                var watch = Stopwatch.StartNew();

                T result = getFromRepoFunc();

                watch.Stop();

                item = new CacheItem<T>
                {
                    Item = result,
                    TimeToComputeItem = watch.ElapsedMilliseconds
                };

                var cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = expiration,
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(100)
                };

                _cache.Set(id, item, cacheExpirationOptions);
            }

            return item.Item;        
        }

        [HttpGet("{id}", Name="GetCustomerByIdCached")]
        public ActionResult<CustomerReadDto> GetCustomerById(int id)
        {
            var cust = this.GetFromCache<Customer>(
                () => _repository.GetCustomerById(id), 
                id, 
                DateTime.Now.AddSeconds(100));

            if (cust == null)
                return NotFound();

            return Ok(_mapper.Map<CustomerReadDto>(cust));
        }
    }
}