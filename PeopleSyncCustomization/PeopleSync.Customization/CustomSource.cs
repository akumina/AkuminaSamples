using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akumina.PeopleSync.Core;
using Akumina.PeopleSync.Core.Entities;
using Akumina.PeopleSync.Core.Implementation;
using Akumina.PeopleSync.Core.Interfaces;

namespace PeopleSync.Customization
{
    public class CustomSource : AadSyncBase, ISyncSource
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
            throw new NotImplementedException();
        }

        public string FetchGroupExtensions(string lastExtensionLink, List<KeyValuePair<string, (string CreationDate, string Visibility, string Tags, string Types)>> groupExtensions)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetData(string objectId, dynamic client, SyncTask.ProcessTask callback)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetAssignedLicenses(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetAppRoleAssignments(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetDirectReports(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetOwnedDevices(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetRegisteredDevices(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetExtendedProperties(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetManager(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetGroupMembers(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetExtendedPropertiesForGroup(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> OnPremisesExtensionAttributes(string objectId, dynamic client)
        {
            throw new NotImplementedException();
        }
    }
}
