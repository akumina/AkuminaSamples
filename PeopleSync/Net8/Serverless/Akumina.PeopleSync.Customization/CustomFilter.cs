using Akumina.Portal.Common.Entities.PeopleSync;
using Akumina.Portal.Common.Interfaces;
using Akumina.Portal.Common.Utilities;
using Microsoft.Extensions.Logging;

namespace Akumina.PeopleSync.Customization;

/// <summary>
/// Custom Filter Implementation to inherit the ICustomFilter interface.
/// User can use this structure to add addtional implementation for the User/Group processing.
/// Each item will be invoked with this Custom Filter Assembly to allow customer to apply their additional logics.
/// </summary>
public class CustomFilter: ICustomFilter
{
    private readonly ILogger log;
    private readonly string credentialId = "Workday_Credential";

    /// <summary>
    /// V1 version of Custom Filter uses this constructor. ILogger Constructor to log information in AppInsights. 
    /// </summary>
    /// <param name="log">ILogger Constructor passed from PeopleSync Serverless Process</param>
    public CustomFilter(ILogger log)
    {
        this.log = log;
    }

    /// <summary>
    /// V2 version of Custom Filter uses this constructor. It will provide the TenantInfo and ILogger to the Custom Filter.
    /// TenantInfo will be used to fetch the Credential Id from the Akumina database.
    /// ILogger Constructor to log information in AppInsights.
    /// </summary>
    /// <param name="tenant">Tenant Info passed to used for Tenant Credential Fetch</param>
    /// <param name="log">ILogger Constructor passed from PeopleSync Serverless Process</param>
    public CustomFilter(TenantInfo tenant, ILogger log)
    {
        // This Sample will allow the Custom Filter to access the Workday Credential for the specific tenant.
        CredentialResponse workdayCredential = Utility.GetCredential(tenant, credentialId);

        // If the credential is not found, log an error and throw an exception. Data will provide the credential value (whatever the format its stored in).
        string strValue = Convert.ToString(workdayCredential.Data);
        this.log = log;
    }

    /// <summary>
	/// Implement logic to filter or skip specific user.
	/// </summary>
	/// <param name="item">User property object is passed. This can be used to implement user filter logic </param>
	/// <returns>If validation passes then returns User property object else returns null</returns>
	public dynamic? UserFilter(dynamic item)
    {
        log.LogInformation("Processing changes from custom library for Akuminadev tenant...");

        //Implement user filter validation logic. If validation passes then return item else return null

        return item;
    }

    /// <summary>
    /// Implement logic to filter or skip specific group.
    /// </summary>
    /// <param name="item">Group property object is passed. This can be used to implement user filter logic</param>
    /// <returns>If validation passes then returns Group property object else returns null</returns>
    public dynamic? GroupFilter(dynamic item)
    {
        log.LogInformation("New Custom Filter for Function App in Group Filter for Akuminadev tenant");

        //Implement user filter validation logic. If validation passes then return item else return null

        return item;
    }
}
