using Akumina.Portal.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Akumina.PeopleSync.Customization;

/// <summary>
/// Custom Filter Implementation to inherit the ICustomFilter interface.
/// User can use this structure to add addtional implementation for the User/Group processing.
/// Each item will be invoked with this Custom Filter Assembly to allow customer to apply their additional logics.
/// </summary>
/// <param name="log">ILogger Constructor passed from PeopleSync Serverless Process</param>
public class CustomFilter(ILogger log) : ICustomFilter
{
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
