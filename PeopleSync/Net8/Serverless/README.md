# Akumina.PeopleSync.Customization: Enhance User and Group Data

This sample project in .NET 8.0 empowers you to customize Akumina PeopleSync with tenant-specific logic. You can:

- **Enrich data**: Include additional properties like Workday data for users and groups.
- **Filter data**: Apply custom rules to filter users and groups based on your specific needs.

## How It Works:

1. **Custom Filter Assembly**: During PeopleSync, each user and group object triggers the `CustomFilter.GroupFilter` and `CustomFilter.UserFilter` methods.
2. **Tenant-Specific Logic**: Implement your custom logic within these methods to:
   - Fetch additional data from external sources like Workday.
   - Apply conditions to filter data based on your criteria.
3. **Vendor Folder**: This folder contains the `Akumina.Portal.Common` assembly, which provides the `ICustomFilter` interface for Serverless version of PeopleSync. Use this interface to inherit the CustomFilter class and implement your custom logic.

## Benefits:
* **Flexible and tailored data**: Adapt PeopleSync behavior to your specific data needs and user base.
* **Enhanced data accuracy**: Ensure your users and groups have the most relevant and complete information.
* **Streamlined workflows**: Eliminate unnecessary data and focus on what matters most.

## Get Started:
1. Clone this repository and explore the sample code.
2. Implement your custom logic in the CustomFilter methods.
3. Configure PeopleSync to use your custom assembly.

**Note**: This sample provides a starting point. You can adapt and expand it to fit your specific requirements and data sources.

**Ready to take control of your PeopleSync data? Dive into this sample and unlock the power of customization!**