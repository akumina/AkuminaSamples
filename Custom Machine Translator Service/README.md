# Custom Machine Translator Service

This sample shows how to build your own machine translation provider for the Akumina platform, in place of the built-in translator. It's a single .NET Framework 4.8 class library, `Akumina.Custom.MachineTranslatorService`, with two implementations of Akumina's `IMachineTranslatorService` interface:

| Provider | Translation service | Class |
|---|---|---|
| [AWS Translate](#aws-translate-provider) | **Amazon Translate**. It calls the `TranslateText` API directly over HTTPS and signs each request with AWS Signature Version 4. It doesn't need the AWS SDK. | `AwsTranslate` |
| [LLM Translate](#llm-translate-provider-microsoft-agent-framework) | **A Large Language Model** (OpenAI, Azure OpenAI / Azure AI Foundry, or any OpenAI-compatible endpoint) through [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/overview/agent-framework-overview). | `LlmTranslate` |

Use it as a starting point when you want Akumina to translate content with a translation service other than the default. Both classes are built into the same DLL, and `unity.config` chooses which one Akumina uses.

## Folder structure

```
Custom Machine Translator Service/
└── 4.8/
    ├── Akumina.Custom.MachineTranslatorService.sln
    └── Akumina.Custom.MachineTranslatorService/
        ├── AwsTranslate.cs                 # Amazon Translate implementation
        ├── LlmTranslate.cs                 # LLM (Microsoft Agent Framework) implementation
        ├── Akumina.Custom.MachineTranslatorService.csproj
        ├── packages.config                 # Newtonsoft.Json, Microsoft.Agents.AI.OpenAI 1.22.0 and its dependencies
        ├── packages/akumina/               # Akumina.Interchange.Core.dll, Akumina.Logging.dll
        └── Properties/AssemblyInfo.cs
```

## Comparing the providers

| Member | AWS Translate | LLM Translate |
|---|---|---|
| `TranslateAsync(text, targetLanguageCode)` | Translates `text` into `targetLanguageCode`. The source language is sent as `en`. | The model detects the source language and translates `text` into `targetLanguageCode`. |
| `TranslateAsync(text, sourceLanguageCode, targetLanguageCode)` | Translates `text` from `sourceLanguageCode` into `targetLanguageCode`. | Same. A source of `auto` or empty means detect. |
| `DetectTextLanguage(text)` | Calls `TranslateText` and returns the `SourceLanguageCode` from the response. | Returns the ISO 639-1 code of the dominant language in lowercase, for example `fr`. |
| `AnalyzeTextAsync(text, language)` | Not implemented. It throws `NotImplementedException`. | Returns up to 20 key phrases and the named entities (people, organizations, locations, products, events). |
| `ValidateTranslatorKey(subscriptionKey)` | Always returns `true`. | Always returns `true`. The LLM key comes from configuration, not from the tenant subscription key. |

Both classes log every method through `Akumina.Logging.TraceEvents`, so you can trace requests and responses in the Akumina logs.

## Prerequisites

- Visual Studio 2019 or later, with the .NET Framework 4.8 developer pack
- Access to an Akumina AppManager installation (AppManager 7 for the LLM provider), to get the Akumina assemblies and to deploy the output
- For AWS Translate: an AWS account with Amazon Translate enabled, and an IAM user or role with the `translate:TranslateText` permission, plus its **Access Key ID** and **Secret Access Key**
- For LLM Translate: an API key and a chat model on an OpenAI-compatible endpoint. For Azure OpenAI, create a model deployment and use its deployment name as the model.

## Build

1. **Add the Akumina assemblies.** Copy `Akumina.Interchange.Core.dll` and `Akumina.Logging.dll` from the `bin` folder of your AppManager website into `4.8\Akumina.Custom.MachineTranslatorService\packages kumina\`. Use the assemblies from the same Akumina version you'll deploy to.
2. **Restore NuGet packages.** Open `4.8\Akumina.Custom.MachineTranslatorService.sln` in Visual Studio and restore NuGet packages. This installs the 42 packages listed in `packages.config` into the solution-level `4.8\packages` folder: `Newtonsoft.Json` 13.0.2 for AWS Translate, plus `Microsoft.Agents.AI.OpenAI` 1.22.0 and all of its dependencies for LLM Translate, each pinned to an exact version. From the command line, run `msbuild Akumina.Custom.MachineTranslatorService.sln -t:restore -p:RestorePackagesConfig=true` or `nuget restore Akumina.Custom.MachineTranslatorService.sln`.

   > `packages.config` doesn't resolve dependencies on its own, so every transitive package is listed. To upgrade Agent Framework, update `Microsoft.Agents.AI.OpenAI` with the Visual Studio NuGet Package Manager, which also updates its dependencies, the `<Reference>` hint paths, and the `System.ValueTuple` targets import in the `.csproj`.

3. **Build** the solution in the **Release** configuration. The output folder `bin\Release\` contains:

   - `Akumina.Custom.MachineTranslatorService.dll`, with both `AwsTranslate` and `LlmTranslate`
   - `Newtonsoft.Json.dll`
   - About 40 Agent Framework dependency assemblies (`Microsoft.Extensions.AI`, the OpenAI SDK, `System.Text.Json`, and so on), which only `LlmTranslate` needs
   - `Akumina.Custom.MachineTranslatorService.dll.config`, which lists the binding redirects that the LLM dependencies need

> Package paths are long, up to about 150 characters below the `4.8` folder, and Windows limits paths to 260 characters unless long paths are enabled. Clone the repository into a short folder, such as `C:\src`, to leave extra room.

## Register the class

To tell Akumina which provider to use, point the `IMachineTranslatorService` registration in `unity.config` at one of the classes:

```xml
<!-- AWS Translate -->
<register type="IMachineTranslatorService" mapTo="Akumina.Custom.MachineTranslatorService.AwsTranslate, Akumina.Custom.MachineTranslatorService" />

<!-- LLM Translate -->
<register type="IMachineTranslatorService" mapTo="Akumina.Custom.MachineTranslatorService.LlmTranslate, Akumina.Custom.MachineTranslatorService" />
```

The `mapTo` value has the form `<Namespace>.<ClassName>, <AssemblyName>`. If you rename the class or the assembly, update this value to match. After any change, recycle the AppManager application pool so the new registration is loaded.

---

## AWS Translate provider

### Configure

1. In `AwsTranslate.cs`, replace the placeholder credentials:

   ```csharp
   private const string AccessKey = "<Access Key>";
   private const string SecretKey = "<Secret Key>";
   ```

   The region is set in `AwsTranslateText`. The default is `us-east-1`. Change it if your account uses a different region:

   ```csharp
   const string regionName = "us-east-1";
   ```

   > **Security:** Don't commit real AWS keys to source control. For production, read the keys from a secure location instead of hard-coding them. Examples are `web.config` app settings, environment variables, or a secrets store such as AWS Secrets Manager or Azure Key Vault. Grant the IAM identity only the permissions it needs, such as `translate:TranslateText`.

2. [Build](#build) the solution.

### Deployment

1. Copy `Akumina.Custom.MachineTranslatorService.dll` to the `bin` folder of your AppManager website. Also copy `Newtonsoft.Json.dll` if that version isn't already there.

   AWS Translate doesn't use the Agent Framework assemblies. .NET loads a referenced assembly only when code that uses it runs, so you don't need to deploy them, or change any binding redirects, while `AwsTranslate` is the registered class.
2. [Register `AwsTranslate` in `unity.config`](#register-the-class) and recycle the application pool.
3. Trigger a translation in Akumina and check the logs for entries that start with `Akumina.Custom.MachineTranslatorService.AwsTranslate`.

### Customizing

Things to consider before you use this code in production:

- **Automatic source-language detection:** Amazon Translate detects the source language when `SourceLanguageCode` is set to `auto`. Right now, `TranslateAsync(text, targetLanguageCode)` and `DetectTextLanguage` send `en`. For real auto-detection, change `"en"` to `"auto"` in those methods. For language detection alone, you can call Amazon Comprehend `DetectDominantLanguage` instead.
- **Language code format:** Akumina may pass culture-style codes such as `es-es` or `en-us`. Amazon Translate expects codes such as `es`, `en`, `fr-CA`, or `zh-TW`. Add a mapping step if the codes you receive don't match what AWS supports.
- **Key validation:** `ValidateTranslatorKey` always returns `true`. You can make it send a small test request to AWS and return `false` if the request fails.
- **Text analytics:** `AnalyzeTextAsync` isn't implemented. If you need key phrases and named entities, implement it with a service such as Amazon Comprehend and return a `TextAnalyticsOutput`.
- **Request limits:** Amazon Translate limits the size of each request. Split long content into smaller chunks before you translate it.
- **Error handling:** If AWS returns a non-success status, `HttpWebRequest` throws a `WebException`. Add retry and back-off logic for throttling errors.

---

## LLM Translate provider (Microsoft Agent Framework)

This provider translates with a Large Language Model instead of a dedicated translation API. It works with any endpoint that supports the OpenAI Chat Completions API:

- **OpenAI** (`https://api.openai.com/v1`)
- **Azure OpenAI / Azure AI Foundry**, through the v1 endpoint (`https://<resource>.openai.azure.com/openai/v1/`)
- **Self-hosted or third-party models** that expose an OpenAI-compatible API, such as Ollama, vLLM, or LM Studio

Compared with Amazon Translate, an LLM:

- Understands Akumina culture codes such as `es-es` and `fr-ca` directly, so there's no language-code mapping to maintain.
- Preserves HTML markup, placeholders, and brand names when you tell it to in the instructions.
- Implements all of `IMachineTranslatorService`, including language detection and `AnalyzeTextAsync`.

### How it works

`LlmTranslate` creates three stateless Agent Framework agents (`AIAgent`). Each agent is an OpenAI `ChatClient` with its own system instructions:

| Agent | Used by | Output |
|---|---|---|
| `AkuminaTranslator` | `TranslateAsync` (both overloads) | Plain translated text |
| `AkuminaLanguageDetector` | `DetectTextLanguage` | Structured output (`LanguageDetectionResult`), which uses `RunAsync<T>` and a JSON schema |
| `AkuminaTextAnalyzer` | `AnalyzeTextAsync` | Structured output (`TextAnalysisResult`), mapped to `TextAnalyticsOutput` |

```csharp
var client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
AIAgent agent = client.GetChatClient(model).AsAIAgent(instructions: TranslatorInstructions, name: "AkuminaTranslator");

AgentResponse response = await agent.RunAsync(prompt).ConfigureAwait(false);
```

The agents are created once, on first use, and shared. Every call runs without an `AgentSession`, so calls are independent and no chat history builds up between translations. Empty or whitespace input is returned as is, without calling the model. Failures are logged with `TraceEvents.Log.Error` and then rethrown.

### Configure

1. [Build](#build) the solution. Agent Framework ships a `net472` build that runs on .NET Framework 4.8.
2. Add these keys to `<appSettings>` in the AppManager `web.config`:

   ```xml
   <!-- OpenAI -->
   <add key="LlmTranslator:Endpoint" value="https://api.openai.com/v1" />
   <add key="LlmTranslator:ApiKey" value="<API Key>" />
   <add key="LlmTranslator:Model" value="gpt-4.1-mini" />

   <!-- Azure OpenAI / Azure AI Foundry: use the v1 endpoint and the deployment name -->
   <add key="LlmTranslator:Endpoint" value="https://<resource>.openai.azure.com/openai/v1/" />
   <add key="LlmTranslator:ApiKey" value="<Azure OpenAI Key>" />
   <add key="LlmTranslator:Model" value="<deployment name>" />
   ```

   If a key is missing, the default constant at the top of `LlmTranslate.cs` is used.

   > **Security:** Don't commit real API keys to source control. For production, keep the key in a secrets store such as Azure Key Vault, or in encrypted configuration, and give the key only the access it needs.

### Deployment

> **Important:** Agent Framework depends on current versions of shared assemblies such as `System.Text.Json` (10.x), `Microsoft.Bcl.AsyncInterfaces` (10.x), `System.Diagnostics.DiagnosticSource` (10.x), and `Microsoft.Extensions.*.Abstractions` (10.x). The AppManager `bin` folder already has older versions of about 20 of these, for example `System.Text.Json` 6.0 and `Microsoft.Extensions.*` 2.2 in AppManager 7. Deploying the LLM provider upgrades them for the whole AppManager site. Try it in a non-production environment first, and keep a backup of `bin` and `web.config`.

1. Back up the AppManager `bin` folder and `web.config`.
2. Copy every `.dll` from `bin\Release\` into the AppManager `bin` folder, **except** `Akumina.Interchange.Core.dll` and `Akumina.Logging.dll`. Only overwrite an existing DLL when the new file's assembly version is the same or higher. Never downgrade an assembly that AppManager already has.
3. Update the binding redirects in `web.config`. Every assembly you replaced needs its `<bindingRedirect>` set to the version you deployed, and every assembly listed in `Akumina.Custom.MachineTranslatorService.dll.config` needs a redirect. Edit the existing `<dependentAssembly>` entries instead of adding duplicates. For example:

   ```xml
   <dependentAssembly>
     <assemblyIdentity name="System.Text.Json" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
     <bindingRedirect oldVersion="0.0.0.0-10.0.0.12" newVersion="10.0.0.12" />
   </dependentAssembly>
   ```

4. [Register `LlmTranslate` in `unity.config`](#register-the-class) and recycle the application pool.
5. Trigger a translation and check the logs for entries that start with `Akumina.Custom.MachineTranslatorService.LlmTranslate`. Also check that the rest of AppManager still works, because the shared assemblies were upgraded.

If the shared-assembly upgrade isn't acceptable for your site, move the Agent Framework code into a separate service, such as an Azure Function or a small ASP.NET Core API. Then keep only a thin `IMachineTranslatorService` class in AppManager that calls that service over HTTP. The thin class doesn't need any new assemblies in AppManager.

### Customizing

- **Instructions:** The agent behavior is set by `TranslatorInstructions`, `DetectorInstructions`, and `AnalyzerInstructions` in `LlmTranslate.cs`. Add your company glossary, tone of voice, or terms that must never be translated here.
- **Temperature and other model options:** To make output more deterministic, pass a `ChatClientAgentOptions` with `ChatOptions = new ChatOptions { Temperature = 0 }` to `AsAIAgent`. Some reasoning models reject `Temperature`, so check what your model supports.
- **Structured output support:** `DetectTextLanguage` and `AnalyzeTextAsync` send a JSON schema (`response_format: json_schema`). If your OpenAI-compatible server doesn't support that, change them to plain-text prompts and parse the reply yourself.
- **Prompt injection:** The text to translate is wrapped in `<text>` tags, and the instructions tell the model to treat it as data. This lowers the risk that content in a page changes what the model does, but it doesn't remove it. Don't give these agents tools that can take actions.
- **Long content:** Every model has a context window and an output token limit. Split very long pages into sections, for example by paragraph or HTML block, before you translate them.
- **Cost and latency:** Each call is a model request. Pick a small, fast model for translation, and consider caching translations that Akumina asks for more than once.
- **Retries:** The OpenAI SDK retries transient failures, such as HTTP 429 and 5xx, with exponential back-off. To change this, set `RetryPolicy` and `NetworkTimeout` on `OpenAIClientOptions` in `CreateAgent`.
- **Other LLM providers:** Agent Framework can also build agents on top of any `Microsoft.Extensions.AI.IChatClient`. To use a provider that has no OpenAI-compatible endpoint, create its `IChatClient` and call `chatClient.AsAIAgent(...)` in `CreateAgent`.

---

## Adding another provider

To use a different translation service, such as Google Cloud Translation or DeepL, add another class to the project that implements `IMachineTranslatorService` and [register that class in `unity.config`](#register-the-class).
