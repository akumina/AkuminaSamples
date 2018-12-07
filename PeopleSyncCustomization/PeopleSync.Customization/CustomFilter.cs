using Akumina.PeopleSync.Core;
using Akumina.PeopleSync.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace PeopleSync.Customization
{
    public class CustomFilter:ICustomFilter
    {
        public dynamic UserFilter(dynamic changes)
        {
            ConsoleLogger.ShowInfo("Processing changes from custom library...");
            var result = new JArray();
            var items = (JArray)changes;
            foreach (var item in items)
            {
                if (item["givenName"] == null || item["givenName"] != null && string.IsNullOrEmpty(item["givenName"].ToString()))
                    continue;
                if (item["surname"] == null || item["surname"] != null && string.IsNullOrEmpty(item["surname"].ToString()))
                    continue;
                if (item["accountEnabled"] == null || item["accountEnabled"] != null && item["accountEnabled"].ToString().ToLower() != "true")
                    continue;
                if (item["userType"] == null || item["userType"] != null && item["userType"].ToString().ToLower() != "member")
                    continue;
                if (item["userPrincipalName"] == null || item["userPrincipalName"] != null && item["userPrincipalName"].ToString().StartsWith("ADMIN."))
                    continue;
                if (item["dirsyncenabled"] != null && item["dirsyncenabled"].ToString().Equals("false", System.StringComparison.InvariantCultureIgnoreCase))
                    continue;
                result.Add(item);
            }
            return result;
        }

        public dynamic GroupFilter(dynamic changes)
        {
            return changes;
        }
    }
}