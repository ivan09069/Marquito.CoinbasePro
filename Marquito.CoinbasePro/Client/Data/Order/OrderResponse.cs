using Marquito.CoinbasePro.Client.Data.Common;
using Newtonsoft.Json;

namespace Marquito.CoinbasePro.Client.Data.Order
{
    /// <summary>
    /// Represents the response to an order request, including success status, order details, and error information.
    /// </summary>
    /// <remarks>This class encapsulates the result of an order operation. It provides information about
    /// whether the operation  was successful, the details of the order if successful, or error details if the operation
    /// failed.</remarks>
    public class OrderResponse
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful
        /// </summary>
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// Gets the order associated with the success response
        /// </summary>
        [JsonProperty("success_response")]
        public Order Order { get; set; }
        /// <summary>
        /// Gets the error response associated with the current operation
        /// </summary>
        [JsonProperty("error_response")]
        public ErrorResponse Error { get; set; }
    }
}
