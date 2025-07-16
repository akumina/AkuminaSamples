using Akumina.PeopleSync.Core;
using Akumina.PeopleSync.Core.Entities;
using Akumina.PeopleSync.Core.Interfaces;

namespace Akumina.PeopleSync.Customization;

/// <summary>
/// Custom Filter Implementation to inherit the ICustomFilter interface.
/// User can use this structure to add addtional implementation for the User/Group processing.
/// Each item will be invoked with this Custom Filter Assembly to allow customer to apply their additional logics.
/// </summary>
public class CustomFilter: ICustomFilter
{
    private readonly string credentialId = "Workday_Credential";

    /// <summary>
    /// V1 version of Custom Filter uses this empty constructor
    /// </summary>
    public CustomFilter() { }

    /// <summary>
    /// V2 version of Custom Filter uses this constructor. It will provide the TenantInfo to the Custom Filter.
    /// TenantInfo will be used to fetch the Credential Id from the Akumina database.
    /// </summary>
    /// <param name="tenant">Tenant Info passed and used for Tenant Credential Fetch</param>
    public CustomFilter(TenantInfo tenant)
    {
        // This Sample will allow the Custom Filter to access the Workday Credential for the specific tenant.
        CredentialResponse workdayCredential = Utility.GetCredential(tenant, credentialId);
        ConsoleLogger.ShowInfo(workdayCredential.IsError.ToString());

        // If the credential is not found, log an error and throw an exception. Data will provide the credential value (whatever the format its stored in).
        string strValue = Convert.ToString(workdayCredential.Data);
        ConsoleLogger.ShowInfo(strValue);
    }

    /// <summary>
	/// Implement logic to filter or skip specific user.
	/// </summary>
	/// <param name="item">User property object is passed. This can be used to implement user filter logic </param>
	/// <returns>If validation passes then returns User property object else returns null</returns>
	public dynamic? UserFilter(dynamic item)
    {
        ConsoleLogger.ShowInfo("Processing changes from custom library for Akuminadev tenant...");

        // Implement user filter validation logic. If validation passes then return item else return null

        return item;
    }

    /// <summary>
    /// Implement logic to filter or skip specific group.
    /// </summary>
    /// <param name="item">Group property object is passed. This can be used to implement user filter logic</param>
    /// <returns>If validation passes then returns Group property object else returns null</returns>
    public dynamic? GroupFilter(dynamic item)
    {
        ConsoleLogger.ShowInfo("New Custom Filter for Function App in Group Filter for Akuminadev tenant");

        // Implement user filter validation logic. If validation passes then return item else return null

        return item;
    }
}
