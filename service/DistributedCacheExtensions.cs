
﻿using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;
/*Microsoft.Extensions.Caching.Abstractions*/
namespace SCZS.DistributedCacheExtensions
{
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// 自动缓存管理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key">cacheKey</param>
        /// <param name="predicate">判断条件</param>
        /// <param name="options">缓存过期策略</param>
        /// <returns></returns>
        public static T GetOrCreate<T>(this IDistributedCache cache, string key, Func<T> predicate, DistributedCacheEntryOptions options)
        {
            T entity = cache.Get<T>(key);

            if (entity != null) { return entity; }

            entity = predicate();

            cache.SetString(key, entity.ToSerialize<T>(), options);

            return entity;
        }

        /// <summary>
        /// 自动缓存管理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key">cacheKey</param>
        /// <param name="predicate">判断条件</param>
        /// <param name="timeSpan">缓存过期时间默认为无限期</param>
        /// <returns></returns>
        public static T GetOrCreate<T>(this IDistributedCache cache, string key, Func<T> predicate, TimeSpan? timeSpan = default)
        {
            if (timeSpan == null)
            {
                return GetOrCreate<T>(cache, key, predicate);
            }

            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(timeSpan.Value);

            return GetOrCreate<T>(cache, key, predicate, options);
        }



        /// <summary>
        /// 自动缓存管理异步模式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key">cacheKey</param>
        /// <param name="predicate">判断条件</param>
        /// <param name="options">缓存过期策略</param>
        /// <returns></returns>
        public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> predicate, DistributedCacheEntryOptions options)
        {
            T entity = cache.Get<T>(key);

            if (entity != null) return entity;

            entity = await predicate();

            await cache.SetStringAsync(key, entity.ToSerialize<T>(), options);

            return entity;
        }

        /// <summary>
        /// 自动缓存管理异步模式
        /// </summary>
        /// <param name="key">cacheKey</param>
        /// <param name="predicate">判断条件</param>
        /// <returns></returns>
        public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> predicate)
        {
            T entity = await cache.GetAsync<T>(key);

            if (entity != null) return entity;

            entity = await predicate();

            await cache.SetStringAsync(key, entity.ToSerialize<T>());

            return entity;
        }


        /// <summary>
        /// 自动缓存管理异步模式
        /// </summary>
        /// <param name="cache">cacheKey</param>
        /// <param name="key">判断条件</param>
        /// <param name="predicate"></param>
        /// <param name="timeSpan">缓存过期时间默认为无限期</param>
        /// <returns></returns>
        public static Task<T> GetOrCreateAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> predicate, TimeSpan? timeSpan = default)
        {
            if (timeSpan == null)
            {
                return GetOrCreateAsync<T>(cache, key, predicate);
            }

            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(timeSpan.Value);

            return GetOrCreateAsync<T>(cache, key, predicate, options);

        }



        /// <summary>
        /// 查询缓存异步模式
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            T result = default;

            var binary = await cache.GetStringAsync(key, cancellationToken);

            if (binary != null) { result = binary.ToDeserialize<T>(); }

            return result;
        }

        public static T Get<T>(this IDistributedCache cache, string key)
        {
            T result = default;

            var binary = cache.GetString(key);

            if (binary != null) { result = binary.ToDeserialize<T>(); }

            return result;
        }


        /// <summary>
        /// 设置缓存，存在的会覆盖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="predicate"></param>
        /// <param name="timeSpan"></param>
        public static void Set<T>(this IDistributedCache cache, string key, Func<T> predicate, TimeSpan? timeSpan = default)
        {
            Set<T>(cache, key, predicate, timeSpan);
        }
        /// <summary>
        /// 设置缓存，存在的会覆盖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="predicate"></param>
        /// <param name="timeSpan"></param>
        public static void Set<T>(this IDistributedCache cache, string key, Func<T> predicate, DistributedCacheEntryOptions options)
        {
            Set<T>(cache, key, predicate, options);
        }


        /// <summary>
        /// 设置缓存，存在的会覆盖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="predicate"></param>
        /// <param name="timeSpan"></param>
        public static void Set<T>(this IDistributedCache cache, string key, T entity, DistributedCacheEntryOptions options)
        {
            cache.SetString(key, entity.ToSerialize<T>(), options);
        }
        /// <summary>
        /// 设置缓存，存在的会覆盖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="predicate"></param>
        /// <param name="timeSpan"></param>
        public static void Set<T>(this IDistributedCache cache, string key, T entity, TimeSpan? timeSpan = default)
        {
            if (timeSpan == null)
            {
                cache.SetString(key, entity.ToSerialize<T>());

                return;
            }

            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(timeSpan.Value);

            cache.SetString(key, entity.ToSerialize<T>(), options);
        }

        /// <summary>
        /// 设置缓存，存在的会覆盖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="predicate"></param>
        /// <param name="timeSpan"></param>
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T entity, DistributedCacheEntryOptions options)
        {
            return cache.SetAsync(key, entity.ToSerialize(), options);
        }

        /// <summary>
        /// 设置缓存，存在的会覆盖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="predicate"></param>
        /// <param name="timeSpan"></param>
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> functask, DistributedCacheEntryOptions options)
        {
            var entity = await functask().ConfigureAwait(false);

            await SetAsync<T>(cache, key, functask, options);
        }
        /// <summary>
        /// 设置缓存，存在的会覆盖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="predicate"></param>
        /// <param name="timeSpan"></param>
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> functask, TimeSpan? timeout = default)
        {
            var entity = await functask().ConfigureAwait(false);

            if (timeout == null)
            {
                await cache.SetStringAsync(key, entity.ToSerialize<T>());

                return;
            }

            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(timeout.Value);

            await cache.SetStringAsync(key, entity.ToSerialize<T>(), options);
        }

    }
}
