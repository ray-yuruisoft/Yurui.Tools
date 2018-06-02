using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yurui.Tools
{
    public class CacheService : ICacheService
    {
        protected MemoryCache _cache = MemoryCache.Default;
        public CacheService()
        {
        }
        public void Dispose()
        {
            if (_cache != null)
                _cache.Dispose();
            GC.SuppressFinalize(this);
        }

        public T GetOrCreate<T>(string key, TimeSpan expiresSliding, TimeSpan expiressAbsoulte, Func<T> factory) where T : class, new()
        {
            dynamic value = _cache.Get(key) as T;
            if (value == default(T) || (value is IEnumerable && value.Count == 0))
            {
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.Add(expiressAbsoulte),
                    SlidingExpiration = expiresSliding
                };

                var res = factory();
                if (res != null)
                {
                    _cache.Set(key, res, policy);
                    value = _cache.Get(key) as T;
                    return value;
                }
            }
            return value;
        }

        #region 添加缓存

        public bool Add(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _cache.Set(key, value, System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration);
            return Exists(key);
        }

        public bool Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _cache.Set(key, value,
                new CacheItemPolicy
                {
                    SlidingExpiration = expiresSliding,
                    AbsoluteExpiration = DateTime.Now.Add(expiressAbsoulte)
                });

            return Exists(key);
        }

        public bool Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (isSliding)
                _cache.Set(key, value,
                    new CacheItemPolicy
                    {
                        SlidingExpiration = expiresIn
                    });
            else
                _cache.Set(key, value,
                   new CacheItemPolicy
                   {
                       AbsoluteExpiration = DateTime.Now.Add(expiresIn)
                   });
            return Exists(key);
        }

        public Task<bool> AddAsync(string key, object value)
        {
            return Task.Factory.StartNew(() =>
            {
                return Add(key, value);
            });
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            return Task.Factory.StartNew(() =>
            {
                return Add(key, value, expiresSliding, expiressAbsoulte);
            });
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            return Task.Factory.StartNew(() =>
            {
                return Add(key, value, expiresIn, isSliding);
            });
        }

        #endregion

        #region 验证缓存项是否存在

        public bool Exists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Contains(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                return Exists(key);
            });
        }

        #endregion

        #region 获取缓存

        public T Get<T>(string key) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Get(key) as T;
        }

        public object Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Get(key);
        }

        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var dict = new Dictionary<string, object>();
            keys.ToList().ForEach(item => dict.Add(item, _cache.Get(item)));
            return dict;
        }

        public Task<IDictionary<string, object>> GetAllAsync(IEnumerable<string> keys)
        {
            return Task.Factory.StartNew(() =>
            {
                return GetAll(keys);
            });
        }

        public Task<T> GetAsync<T>(string key) where T : class
        {
            return Task.Factory.StartNew(() =>
            {
                return Get<T>(key);
            });
        }

        public Task<object> GetAsync(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                return Get(key);
            });
        }

        #endregion

        #region 删除缓存

        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _cache.Remove(key);
            return !Exists(key);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            keys.ToList().ForEach(item => _cache.Remove(item));
        }

        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            return Task.Factory.StartNew(() =>
            {
                RemoveAll(keys);
            });
        }

        public Task<bool> RemoveAsync(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                return Remove(key);
            });
        }

        #endregion

        #region 修改缓存

        public bool Replace(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (Exists(key))
                if (!Remove(key)) return false;
            return Add(key, value);
        }

        public bool Replace(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (Exists(key))
                if (!Remove(key)) return false;

            return Add(key, value, expiresSliding, expiressAbsoulte);
        }

        public bool Replace(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (Exists(key))
                if (!Remove(key)) return false;

            return Add(key, value, expiresIn, isSliding);
        }

        public Task<bool> ReplaceAsync(string key, object value)
        {
            return Task.Factory.StartNew(() =>
            {
                return Replace(key, value);
            });
        }

        public Task<bool> ReplaceAsync(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            return Task.Factory.StartNew(() =>
            {
                return Replace(key, value, expiresSliding, expiressAbsoulte);
            });
        }

        public Task<bool> ReplaceAsync(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            return Task.Factory.StartNew(() =>
            {
                return Replace(key, value, expiresIn, isSliding);
            });
        }

        #endregion

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public void RemoveCacheRegex(string pattern)
        {
            IList<string> l = SearchCacheRegex(pattern);
            foreach (var s in l)
            {
                Remove(s);
            }
        }

        /// <summary>
        /// 搜索 匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IList<string> SearchCacheRegex(string pattern)
        {
            var cacheKeys = GetCacheKeys();
            var l = cacheKeys.Where(k => Regex.IsMatch(k, pattern)).ToList();
            return l.AsReadOnly();
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public List<string> GetCacheKeys()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                keys.Add(cacheItem.Key.ToString());
            }
            return keys;
        }
    }
}
