<b>PeopleSync Customization</b>

To customize the data source, implement your custom logic in `CustomSource.cs`. Compile the project, and then copy the output assembly to the directory where the PeopleSync executable is running.

Open `config.json` and update the following property:

```json
"SyncSourceAssemblyType": "PeopleSync.Customization,PeopleSync.Customization.CustomSource"
```

Depending on your custom processor, you may also need to adjust the following properties in `config.json`:

```json
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
}
```

<b>Token Store</b>

This sample demonstrates how customers can create a custom token store for storing Microsoft Graph and SharePoint tokens.

To run this sample, download or clone the repository, compile the source code, and then copy the `Akumina.Samples.TokenStore.dll` assembly to the `bin` folder of your AppManager website.

To run the sample without additional modifications, update `UNITY.CONFIG` as shown below.

Replace:

```xml
<register type="IRepository[UserToken]" mapTo="AzureTableTokenStore" />
```

With:

```xml
<register type="IRepository[UserToken]" mapTo="Akumina.Samples.TokenStore.NtFileTokenStore, Akumina.Samples.TokenStore" />
```

<b>Note:</b> This sample has the path `C:\Temp\` hard-coded. Change this path to your preferred location before compiling the project.

<b>Web API Sample</b>

Copy the required `Akumina.*.dll` files from your website’s `bin` directory and the `packages/akumina` folder.

Refer to the document [Adding optional claims and custom scope in Access token.pdf](https://github.com/adansari/AkuminaSamples/blob/8b637eb8af2eb2c9b63bf5c995430fd93dbc07e5/Adding%20optional%20claims%20and%20custom%20scope%20in%20Access%20token.pdf) for instructions on configuring additional claims and scopes.
