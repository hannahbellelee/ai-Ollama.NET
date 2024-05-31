﻿namespace Ollama.Core;

public sealed partial class OllamaClient
{
    /// <summary>
    /// Create a model from a Modelfile.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="modelFileContent">Contents of the Modelfile.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the initial request or ongoing streaming operation.</param>
    /// <returns></returns>
    public async Task<CreateModel> CreateModelAsync(string name, string modelFileContent, CancellationToken cancellationToken = default)
    {
        return await this.CreateModelAsync(new CreateModelRequest
        {
            Name = name,
            ModelFileContent = modelFileContent
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a model from a Modelfile with streaming response.<br />
    /// It is recommended to set modelfile to the content of the Modelfile rather than just set path. 
    /// This is a requirement for remote create. 
    /// Remote model creation must also create any file blobs, fields such as FROM and ADAPTER, explicitly with the server using Create a Blob and the value to the path indicated in the response.<br />
    /// <list type="bullet">
    /// <item><see cref="https://github.com/ollama/ollama/blob/main/docs/api.md#create-a-model"/><br /></item>
    /// <item><see cref="https://github.com/ollama/ollama/blob/main/docs/modelfile.md"/></item>
    /// </list>
    /// </summary>
    /// <param name="request">The create model request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the initial request or ongoing streaming operation.</param>
    /// <returns></returns>
    public async Task<StreamingResponse<CreateModel>> CreateModelStreamingAsync(CreateModelRequest request, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(request, nameof(request));
        Argument.AssertNotNullOrEmpty(request.Name, nameof(request.Name));

        this._logger.LogDebug("Create model streaming: {name}", request.Name);

        try
        {
            HttpRequestMessage requestMessage = request.ToHttpRequestMessage();

            (HttpResponseMessage httpResponseMessage, string responseContent) = await this.ExecuteHttpRequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            this._logger.LogTrace("Create model response content: {responseContent}", responseContent);

            (HttpResponseMessage HttpResponseMessage, string ResponseContent) response = await this.ExecuteHttpRequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            return StreamingResponse<CreateModel>.CreateFromResponse(response.HttpResponseMessage, (responseMessage) => ServerSendEventAsyncEnumerator<CreateModel>.EnumerateFromSseStream(responseMessage, cancellationToken));
        }
        catch (HttpOperationException ex)
        {
            this._logger.LogError(ex, "Request for create model streaming faild. {Message}", ex.Message);

            throw;
        }
    }

    /// <summary>
    /// Create a model from a Modelfile.<br />
    /// It is recommended to set modelfile to the content of the Modelfile rather than just set path. 
    /// This is a requirement for remote create. 
    /// Remote model creation must also create any file blobs, fields such as FROM and ADAPTER, explicitly with the server using Create a Blob and the value to the path indicated in the response.<br />
    /// <list type="bullet">
    /// <item><see cref="https://github.com/ollama/ollama/blob/main/docs/api.md#create-a-model"/><br /></item>
    /// <item><see cref="https://github.com/ollama/ollama/blob/main/docs/modelfile.md"/></item>
    /// </list>
    /// </summary>
    /// <param name="request">The create model request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the initial request or ongoing streaming operation.</param>
    /// <returns></returns>
    public async Task<CreateModel> CreateModelAsync(CreateModelRequest request, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(request, nameof(request));
        Argument.AssertNotNullOrEmpty(request.Name, nameof(request.Name));

        this._logger.LogDebug("Create model: {name}", request.Name);

        try
        {
            HttpRequestMessage requestMessage = request.ToHttpRequestMessage();

            (HttpResponseMessage httpResponseMessage, string responseContent) = await this.ExecuteHttpRequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            this._logger.LogTrace("Create model response content: {responseContent}", responseContent);

            CreateModel? createModel = responseContent.FromJson<CreateModel>();

            return createModel is null || string.IsNullOrWhiteSpace(createModel.Status)
                ? throw new DeserializationException(responseContent, message: $"The create model response content: '{responseContent}' cannot be deserialize to an instance of {nameof(CreateModel)}.", innerException: null)
                : createModel;
        }
        catch (HttpOperationException ex)
        {
            this._logger.LogError(ex, "Request for create model faild. {Message}", ex.Message);

            throw;
        }
    }

    /// <summary>
    /// Load the speficied model into memory.
    /// </summary>
    /// <param name="model">The model name.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the initial request or ongoing streaming operation.</param>
    /// <returns>The model loaded response.</returns>
    /// <exception cref="DeserializationException">When deserialize the response is null.</exception>
    public async Task<LoadModel> LoadModelAsync(string model, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(model, nameof(model));

        this._logger.LogDebug("Load model: {model}", model);

        try
        {
            LoadModelRequest request = new()
            {
                Model = model
            };

            HttpRequestMessage requestMessage = request.ToHttpRequestMessage();

            (HttpResponseMessage httpResponseMessage, string responseContent) = await this.ExecuteHttpRequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            this._logger.LogTrace("Load model response content: {responseContent}", responseContent);

            LoadModel? loadModel = responseContent.FromJson<LoadModel>();

            return loadModel is null || string.IsNullOrWhiteSpace(loadModel.Model)
                ? throw new DeserializationException(responseContent, message: $"The load model response content: '{responseContent}' cannot be deserialize to an instance of {nameof(LoadModel)}.", innerException: null)
                : loadModel;
        }
        catch (HttpOperationException ex)
        {
            this._logger.LogError(ex, "Request for load model faild. {Message}", ex.Message);

            throw;
        }
    }
}