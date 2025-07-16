using Akumina.PeopleSync.Customization;
using Microsoft.Extensions.Logging;

// Create a simple logger using the ConsoleLogger
ILogger<Program> logger = LoggerFactory
    .Create(builder => builder.AddConsole())
    .CreateLogger<Program>();

// Create an instance of CustomFilter with the logger
var customFilter = new CustomFilter(logger);
var user = new
{
    mobilePhone = "555-123-4567",
    officelocation = "Chicago Office",
    objectId = "a3b9e8f1-42f5-4a1e-9380-34b8d12d89af",
    accountEnabled = "True",
    businessphones = "[\"555-123-4567\"]",
    city = "Chicago",
    companyName = "Contoso Ltd.",
    country = "USA",
    department = "Engineering",
    dirSyncEnabled = "true",
    displayName = "Jane Doe",
    givenName = "Jane",
    jobTitle = "Software Engineer",
    mail = "jdoe@contosodev.onmicrosoft.com",
    mailNickname = "jdoe",
    postalCode = "60601",
    preferredLanguage = "en-US",
    state = "IL",
    streetAddress = "456 Dev Lane",
    surname = "Doe",
    userPrincipalName = "jdoe@contosodev.onmicrosoft.com",
    UserType = "Member",
    proxyAddresses = "[\"SMTP:jdoe@contosodev.onmicrosoft.com\"]",
    department_keyword = "Engineering",
    jobTitle_keyword = "Software Engineer",
    givenName_keyword = "Jane",
    surname_keyword = "Doe",
    userId = "a3b9e8f1-42f5-4a1e-9380-34b8d12d89af",
    ageGroup = "Adult",
    consentProvidedForMinor = "",
    userIdentities = "[{ \"signInType\": \"userPrincipalName\", \"issuer\": \"contosodev.onmicrosoft.com\", \"issuerAssignedId\": \"jdoe@contosodev.onmicrosoft.com\" }]",
    employeeId = "EMP123456",
    legalAgeGroupClassification = "Adult",
    OnPremisesDistinguishedName = "CN=Jane Doe,OU=Engineering,DC=contosodev,DC=onmicrosoft,DC=com",
    showInAddressList = "true",
    createdDateTime = "2023-03-10T10:20:30",
    aboutMe = "Software engineer passionate about clean code.",
    birthday = "5/10/1990 12:00:00 AM",
    hireDate = "6/1/2022 9:00:00 AM",
    interests = "[\"Coding\",\"Gaming\"]",
    schools = "[\"University of Illinois\"]",
    skills = "[\"C#\",\"Azure\",\"React\"]",
    faxNumber = "555-987-6543",
    AssignedLicenses = "[{ \"disabledPlans\": [], \"skuId\": \"abcd1234-5678-90ab-cdef-1234567890ab\" }]",
    NumberOfLicenses = "1",
    DirectReports = "[]",
    manager = "{\"businessPhones\":[],\"displayName\":\"John Smith\",\"givenName\":\"John\",\"jobTitle\":\"Engineering Manager\",\"mobilePhone\":null,\"officeLocation\":\"Chicago Office\",\"surname\":\"Smith\",\"userPrincipalName\":\"jsmith@contosodev.onmicrosoft.com\",\"id\":\"b1a5d001-9a12-4c6c-9f3b-0c5345a3c2d1\",\"mail\":\"jsmith@contosodev.onmicrosoft.com\",\"mailNickname\":\"jsmith\",\"country\":\"USA\",\"department\":\"Engineering\",\"objectId\":\"b1a5d001-9a12-4c6c-9f3b-0c5345a3c2d1\"}",
    aklanguageid = "1033",
    IsExternalUser = "false",
    TenantId = "9c88df62-a123-4e1c-a8a1-5b3f7f0cd15f"
};

var group = new
{
    objectId = "d7e5a3bc-b3e9-4a76-a9d4-5e67fba9b01a",
    createdDateTime = "2023-11-01T15:30:00",
    description = "ENG_DEV_TEAM",
    displayName = "Engineering Development Team",
    visibility = "Private",
    objectType = "Group",
    TenantId = "9c88df62-a123-4e1c-a8a1-5b3f7f0cd15f"
};

// Apply filters
var filteredUser = customFilter.UserFilter(user);
var filteredGroup = customFilter.GroupFilter(group);

// Output results
Console.WriteLine($"Filtered User: {(filteredUser != null ? filteredUser.displayName : "None")}");
Console.WriteLine($"Filtered Group: {(filteredGroup != null ? filteredGroup.displayName : "None")}");

Console.ReadLine();
