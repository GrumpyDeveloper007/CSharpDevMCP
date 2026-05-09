using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace CSharpDevMCP.Services
{
    /// <summary>
    /// Provide an interface to remote/local AI models
    /// </summary>
    internal class AiService
    {
        private readonly ChatClient _lmAgent;
        private readonly string _modelName;

        public const float DefaultTemperature = 0.1f;
        public const int DefaultMaxOutputTokenCount = 10000;

        /// <summary>
        /// Opens the connection to our local AI
        /// </summary>
        public AiService(string apiKey, string endpoint, string clientName, int maxTokens, TimeSpan timeout)
        {
            _modelName = clientName;

            // Create a client for your local server
            var client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
            {
                Endpoint = new Uri(endpoint + "/v1"),
                NetworkTimeout = new TimeSpan(0, 20, 0),
                RetryPolicy = new ClientRetryPolicy(20)
            });

            // Get the chat client for the specified model
            _lmAgent = client.GetChatClient(clientName);
        }

        /// <summary>
        /// Ask a question about an image
        /// </summary>
        public async Task<ChatCompletion?> SendImageMessage(string question, byte[] imageBytes, string mimeType)
        {
            var systemPrompt = "";

            var chatOptions = new ChatCompletionOptions()
            {
                Temperature = DefaultTemperature,
                MaxOutputTokenCount = DefaultMaxOutputTokenCount
            };
            List<ChatMessage> messages = [
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(
                    [
                        ChatMessageContentPart.CreateTextPart(question),
                        ChatMessageContentPart.CreateImagePart(BinaryData.FromBytes( imageBytes),mimeType)
                    ])
                ];

            var response = await _lmAgent.CompleteChatAsync(messages, chatOptions);
            return response;
        }


        /// <summary>
        /// Sends a message to the AI LLM, prefers remote models (as they are quicker and free)
        /// </summary>
        public ChatCompletion? SendMessage(string messageText)
        {
            try
            {
                var systemPrompt = "";
                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(messageText)
                };

                var chatOptions = new ChatCompletionOptions()
                {
                    Temperature = DefaultTemperature,
                    MaxOutputTokenCount = DefaultMaxOutputTokenCount
                };

                ChatCompletion response = _lmAgent.CompleteChat(messages, chatOptions);

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
