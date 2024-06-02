﻿namespace Ollama.Core.Models;

/// <summary>
/// <see cref="https://github.com/ollama/ollama/blob/main/docs/api.md#push-a-model"/>
/// </summary>
internal abstract class PushModelRequestBase
{
    /// <summary>
    /// name of the model to push in the form of <namespace>/<model>:<tag>
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Allow insecure connections to the library. 
    /// Only use this if you are pulling from your own library during development.
    /// </summary>
    [JsonPropertyName("insecure")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Insecure { get; set; }

    /// <summary>
    /// To the <see cref="HttpRequestMessage"/>  for send a http request.
    /// </summary>
    /// <returns></returns>
    public HttpRequestMessage ToHttpRequestMessage()
    {
        return HttpRequest.CreatePostRequest("/api/push", this);
    }
}