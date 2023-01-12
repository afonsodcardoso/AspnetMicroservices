using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        //Below are the CRUD operations
        //GET all the "ShoppingCartItems" from the "userName"'s basket
        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await _redisCache.GetStringAsync(userName);

            if (String.IsNullOrEmpty(basket))
                return null;

            //use JSONconvert class to save and extract json objects from redis cache db
            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        //POST "ShoppingCartItems" to the basket
        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);
        }

        //DELETE the basket and all the "ShoppingCartItems" from "userName"'s basket
        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RemoveAsync(userName);
        }
    }
}
