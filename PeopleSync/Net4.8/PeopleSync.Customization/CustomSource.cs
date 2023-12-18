using Akumina.PeopleSync.Core;
using Akumina.PeopleSync.Core.Entities;
using Akumina.PeopleSync.Core.Enums;
using Akumina.PeopleSync.Core.Implementation;
using Akumina.PeopleSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeopleSync.Customization
{
    public class CustomSource : SyncBase, ISyncSource
    {
        public CustomSource(SyncRequest syncRequest)
        {
            Configuration = syncRequest;
        }
        public QueryResult QueryUsers(string resourceSet, string nextLink, ICollection<string> objectClassList, ICollection<string> propertyList)
        {
            throw new NotImplementedException();
        }

        public QueryResult QueryGroups(string resourceSet, string nextLink, ICollection<string> objectClassList, ICollection<string> propertyList)
        {
            return null;
        }

        public string FetchGroupExtensions(string lastExtensionLink, GroupExtensionTarget targets, List<KeyValuePair<string, (string CreationDate, string Visibility, string Tags, string Types)>> groupExtensions)
        {
            return "";
        }

        public Dictionary<string, string> GetData(string objectId, SyncTask.ProcessTask callback)
        {
            return new Dictionary<string, string>();
        }

        public async Task<Dictionary<string, string>> GetAssignedLicenses(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetAppRoleAssignments(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetDirectReports(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetOwnedDevices(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetRegisteredDevices(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetExtendedProperties(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetManager(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetUserGroups(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<QueryResult> GetGroupMembers(string objectId, string nextLink)
        {
            return await Task.Run(() => new QueryResult(new Dictionary<string, string>(), false, nextLink));
        }

        public async Task<Dictionary<string, string>> GetExtendedPropertiesForGroup(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> OnPremisesExtensionAttributes(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }
    }
}