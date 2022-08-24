using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace TylerHendricks_Web.Claim
{
    public class UserService : IUserService
    {
        #region Initialize private field
        private readonly IHttpContextAccessor _httpContext;
        private ISession _session => _httpContext.HttpContext.Session;
        #endregion
        public UserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        /// <summary>
        /// Added by Munesh Sharma
        /// Retun loggedIn userID
        /// </summary>
        /// <returns>UserId</returns>
        public string GetUserId()
        {
            return _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Added by Munesh Sharma
        /// set Sessio value
        /// </summary>
        /// <typeparam name="T">Genric</typeparam>
        /// <param name="Key">Genric</param>
        /// <param name="value">Genric</param>
        public void SetSeesionvalue<T>(string Key, T value)
        {
            _session.Set(Key, JsonSerializer.SerializeToUtf8Bytes(value));
        }

        /// <summary>
        /// Added by Munesh Sharma
        /// get the value from session
        /// </summary>
        /// <typeparam name="T">Genric</typeparam>
        /// <param name="Key">Genric</param>
        /// <returns>Genric</returns>
        public T GetSeesionvalue<T>(string Key)
        {
            var value = _session.Get(Key);

            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }

        /// <summary>
        /// Added by Munesh Sharma
        /// Clear the session value
        /// </summary>
        public void ClearSession()
        {
            _session.Clear();
        }
    }
}
