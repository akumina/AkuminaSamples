# Custom Machine Translator Service

This sample shows how to build your own machine translation provider for the Akumina platform. It uses **Amazon Translate** (AWS) in place of the built-in translator.

The sample is a .NET Framework 4.8 class library. It implements Akumina's `IMachineTranslatorService` interface and calls the Amazon Translate `TranslateText` API directly over HTTPS, signing each request with AWS Signature Version 4. It doesn't need the AWS SDK.

Use it as a starting point when you want Akumina to translate content with a translation service other than the default.

## Folder structure

```
Custom Machine Translator Service/
└── 4.8/
    └── AWS Translate/
        └── Akumina.Custom.MachineTranslatorService/
            ├── Akumina.Custom.MachineTranslatorService.sln
            └── Akumina.Custom.MachineTranslatorService/
                ├── AwsTranslate.cs                 # IMachineTranslatorService implementation
                ├── Akumina.Custom.MachineTranslatorService.csproj
                ├── packages.config                 # Newtonsoft.Json 13.0.2
                └── Properties/AssemblyInfo.cs
```

## Prerequisites

- Visual Studio 2019 or later, with the .NET Framework 4.8 developer pack
- An AWS account with Amazon Translate enabled
- An IAM user or role with the `translate:TranslateText` permission, plus its **Access Key ID** and **Secret Access Key**
- Access to an Akumina AppManager installation, to get the Akumina assemblies and to deploy the output

## Implemented members

`AwsTranslate` implements these `IMachineTranslatorService` members:

| Member | Behavior in this sample |
|---|---|
| `TranslateAsync(text, targetLanguageCode)` | Translates `text` into `targetLanguageCode`. The source language is sent as `en`. |
| `TranslateAsync(text, sourceLanguageCode, targetLanguageCode)` | Translates `text` from `sourceLanguageCode` into `targetLanguageCode`. |
| `DetectTextLanguage(text)` | Calls `TranslateText` and returns the `SourceLanguageCode` from the response. |
| `ValidateTranslatorKey(subscriptionKey)` | Always returns `true`. |
| `AnalyzeTextAsync(text, language)` | Not implemented. It throws `NotImplementedException`. |

Every method writes debug messages through `Akumina.Logging.TraceEvents`, so you can trace requests and responses in the Akumina logs.

## Setup

### 1. Add the Akumina assemblies

The project references two Akumina assemblies that aren't included in this repository:

- `Akumina.Interchange.Core.dll`
- `Akumina.Logging.dll`

Copy them from the `bin` folder of your AppManager website into this folder, next to the `.csproj` file:

```
Akumina.Custom.MachineTranslatorService\packages\akumina\
```

Use the assemblies from the same Akumina version you'll deploy to.

### 2. Restore NuGet packages

Open `Akumina.Custom.MachineTranslatorService.sln` in Visual Studio and restore NuGet packages. This installs `Newtonsoft.Json` 13.0.2 into the solution-level `packages` folder.

### 3. Configure AWS credentials and region

In `AwsTranslate.cs`, replace the placeholder values:

```csharp
private const string AccessKey = "<Access Key>";
private const string SecretKey = "<Secret Key>";
```

The region is set in `AwsTranslateText`. The default is `us-east-1`. Change it if your account uses a different region:

```csharp
const string regionName = "us-east-1";
```

> **Security:** Don't commit real AWS keys to source control. For production, read the keys from a secure location instead of hard-coding them. Examples are `web.config` app settings, environment variables, or a secrets store such as AWS Secrets Manager or Azure Key Vault. Grant the IAM identity only the permissions it needs, such as `translate:TranslateText`.

### 4. Build

Build the solution in the **Release** configuration. The output is:

```
bin\Release\Akumina.Custom.MachineTranslatorService.dll
```

## Deployment

1. Copy `Akumina.Custom.MachineTranslatorService.dll` to the `bin` folder of your AppManager website. Also copy `Newtonsoft.Json.dll` if that version isn't already there.
2. In `unity.config`, point the `IMachineTranslatorService` registration at the custom class:

   ```xml
   <register type="IMachineTranslatorService" mapTo="Akumina.Custom.MachineTranslatorService.AwsTranslate, Akumina.Custom.MachineTranslatorService" />
   ```

   The `mapTo` value has the form `<Namespace>.<ClassName>, <AssemblyName>`. If you rename the class or the assembly, update this value to match.
3. Recycle the AppManager application pool so the new registration is loaded.
4. Trigger a translation in Akumina and check the logs for entries that start with `Akumina.Custom.MachineTranslatorService.AwsTranslate`.

## Customizing the sample

Things to consider before you use this code in production:

- **Automatic source-language detection:** Amazon Translate detects the source language when `SourceLanguageCode` is set to `auto`. Right now, `TranslateAsync(text, targetLanguageCode)` and `DetectTextLanguage` send `en`. For real auto-detection, change `"en"` to `"auto"` in those methods. For language detection alone, you can call Amazon Comprehend `DetectDominantLanguage` instead.
- **Language code format:** Akumina may pass culture-style codes such as `es-es` or `en-us`. Amazon Translate expects codes such as `es`, `en`, `fr-CA`, or `zh-TW`. Add a mapping step if the codes you receive don't match what AWS supports.
- **Key validation:** `ValidateTranslatorKey` always returns `true`. You can make it send a small test request to AWS and return `false` if the request fails.
- **Text analytics:** `AnalyzeTextAsync` isn't implemented. If you need key phrases and named entities, implement it with a service such as Amazon Comprehend and return a `TextAnalyticsOutput`.
- **Request limits:** Amazon Translate limits the size of each request. Split long content into smaller chunks before you translate it.
- **Error handling:** If AWS returns a non-success status, `HttpWebRequest` throws a `WebException`. Add retry and back-off logic for throttling errors.
- **Other providers:** To use a different translation service, such as Google Cloud Translation or DeepL, create another class that implements `IMachineTranslatorService` and register that class in `unity.config`.
