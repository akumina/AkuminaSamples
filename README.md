This sample provides customer to write custom token store for storing graph/sharepoint tokens.  
To run this sample download or clone repository compile the source then copy the assemly Akumina.Samples.TokenStore.dll to your AppManager web site bin folder


If you run the sample as it is then modify the following
UNITY.CONFIG

 &lt;register type="IRepository[UserToken]" mapTo="AzureTableTokenStore" /&gt;
 TO
 &lt;register type="IRepository[UserToken]" mapTo="Akumina.Samples.TokenStore.NtFileTokenStore, Akumina.Samples.TokenStore" /&gt;
 
 NOTE:  This sample has path hard coded to c:\\Temp\\ , change this path to your desired path before compilation.
