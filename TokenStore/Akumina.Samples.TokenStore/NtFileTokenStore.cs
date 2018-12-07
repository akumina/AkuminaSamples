// Copyright (c) Akumina, Inc., All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akumina.Common.Utilities;
using Akumina.Common;
using Akumina.Common.Entities;
using Akumina.Common.Interfaces.Repository;

namespace Akumina.Samples.TokenStore
{
    /// <summary>
    /// This provides sample code to read/write Akumina tokens in local file system;  not recommended to use with production server.
    /// </summary>
    public class NtFileTokenStore : IRepository<UserToken>
    {
        //change this path to read from config
        private static readonly string TokenFilePath = $"C:\\temp\\{AppSettings.UserTokenEntityName}.akt";

        static NtFileTokenStore()
        {
            if (!File.Exists(TokenFilePath))
            {
                File.WriteAllText(TokenFilePath, "");
            }
        }

        #region IRepository<UserToken>
        /// <summary>
        /// Adds new token to token store
        /// </summary>
        /// <param name="token">UserToken</param>
        /// <returns>true if success otherwise false</returns>
        public async Task<bool> AddAsync(UserToken token)
        {
            return await WriteToken(token);
        }

        /// <summary>
        /// Delete the current token
        /// </summary>
        /// <param name="token">UserToken</param>
        /// <returns>true if success otherwise false</returns>
        public async Task<bool> DeleteAsync(UserToken token)
        {
            return await WriteToken(token, false);
        }

        /// <summary>
        /// GetAll Tokens for given QueryContext
        /// </summary>
        /// <param name="criteria">UserTokenQueryContext</param>
        /// <returns>Gets all token for the given QueryContext</returns>
        public async Task<List<UserToken>> GetAllAsync(dynamic criteria)
        {
            if (!(criteria is UserTokenQueryContext tokenQueryCriteria)) return null;
            var tokens = await ReadToken();
            return (List<UserToken>)tokens.Where(p => p.UserId == tokenQueryCriteria.UserId);
        }

        /// <summary>
        /// Get a Token for given QueryContext
        /// </summary>
        /// <param name="criteria">UserTokenQueryContext</param>
        /// <returns>gets a token for the given QueryContext</returns>
        public async Task<UserToken> GetAsync(dynamic criteria)
        {
            if (!(criteria is UserTokenQueryContext tokenQueryCriteria)) return null;
            var tokens = await ReadToken();
            return tokens.FirstOrDefault(p => p.UserId == tokenQueryCriteria.UserId && p.SessionId == tokenQueryCriteria.SessionId);
        }

        /// <summary>
        /// returns the matched count
        /// </summary>
        /// <param name="criteria">UserTokenQueryContext</param>
        /// <returns>returns integer value of matched count</returns>
        public async Task<int> GetCountAsync(dynamic criteria)
        {
            if (!(criteria is UserTokenQueryContext tokenQueryCriteria)) return 0;
            var tokens = await ReadToken();
            return tokens.Count(p => p.UserId == tokenQueryCriteria.UserId);
        }

        /// <summary>
        /// Updates the token if it exists otherwise insert as a new token
        /// </summary>
        /// <param name="token">UserToken</param>
        /// <returns>true if success otherwise false</returns>
        public async Task<bool> UpdateAsync(UserToken token)
        {
            return await WriteToken(token);
        }
        #endregion

        #region helpers
        private static async Task<bool> WriteToken(UserToken token, bool addToken = true)
        {
            await Task.Run(() =>
            {
                using (var mutex = new Mutex(false, "Akumina sample ntfile token store"))
                {
                    mutex.WaitOne();
                    var allText = File.ReadAllText(TokenFilePath);
                    var entries = string.IsNullOrEmpty(allText) ? new List<UserToken>() : Serializer.JsonDeserialize<List<UserToken>>(allText);
                    entries.RemoveAll(p => p.UserId == token.UserId);
                    if (addToken)
                    {
                        entries.Add(token);
                    }
                    allText = Serializer.JsonSerialize(entries);
                    File.WriteAllText(TokenFilePath, allText);
                    mutex.ReleaseMutex();
                }
            });
            return true;
        }
        private static async Task<List<UserToken>> ReadToken()
        {
            var response = new List<UserToken>();
            await Task.Run(() =>
            {
                var allText = File.ReadAllText(TokenFilePath);
                response = Serializer.JsonDeserialize<List<UserToken>>(allText);
            });
            return response;
        }
        #endregion 
    }
}
