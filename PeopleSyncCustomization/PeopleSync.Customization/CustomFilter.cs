using Akumina.PeopleSync.Core;
using Akumina.PeopleSync.Core.Interfaces;

namespace PeopleSync.Customization
{
    public class CustomFilter: ICustomFilter
    {
        public dynamic UserFilter(dynamic item)
        {
            ConsoleLogger.ShowInfo("Processing changes from custom library...");
            if (!item.ContainsKey("givenName") || string.IsNullOrEmpty(item["givenName"].ToString()))
                return null; // returns null will skip processing the user or delete if the user already exists
            if (!item.ContainsKey("surname") || string.IsNullOrEmpty(item["surname"].ToString()))
                return null;
            if (!item.ContainsKey("accountEnabled") || item["accountEnabled"].ToString().ToLower() != "true")
                return null;
            if (!item.ContainsKey("userType") || item["userType"].ToString().ToLower() != "member")
                return null;
            if (!item.ContainsKey("userPrincipalName") || item["userPrincipalName"].ToString().StartsWith("ADMIN."))
                return null;
            if (!item.ContainsKey("dirSyncEnabled") || item["dirSyncEnabled"].ToString().Equals("false", System.StringComparison.InvariantCultureIgnoreCase))
                return null;

            return item; // returns the item with updated property will process the item
        }

        public dynamic GroupFilter(dynamic item)
        {
            return item;
        }

    }
}