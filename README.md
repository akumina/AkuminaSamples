<b>PeopleSyncCustomization</b>

To customize the data source provide implementation to customsource.cs, compile and copy the output to where peoplesync exe is running.  

Open the config.json modify the property "SyncSourceAssemblyType": "PeopleSync.Customization,PeopleSync.Customization.CustomSource",

Based on your custom processor you may need to adjust the following properties within config.json

 "FetchOptions": {
    "SyncUsers": true,
    "SyncGroups": false,
    "SyncTags": false,
    "SyncTypes": false,
    "TagExtension": "",
    "TypeExtension": "",
    "UserProperties": "",
    "UserExtendedProperties": "",
    "GroupProperties": "",
    "SkipUsers": "",
    "SkipGroups": ""
  },


<b>Token Store</b>

This sample provides customer to write custom token store for storing graph/SharePoint tokens.

To run this sample download or clone repository, compile the source then copy the assembly Akumina.Samples.TokenStore.dll to your AppManager website bin folder

If you run the sample as it is, then update the UNITY.CONFIG as shown below

 &lt;register type="IRepository[UserToken]" mapTo="AzureTableTokenStore" /&gt;
 
 TO
 
 &lt;register type="IRepository[UserToken]" mapTo="Akumina.Samples.TokenStore.NtFileTokenStore, Akumina.Samples.TokenStore" /&gt;
 
 NOTE:  This sample has path hard coded to c:\\Temp\\ , change this path to your desired path before compilation.
