using Newtonsoft.Json;

namespace FieldDataTest
{
    public enum ServiceResultStatus
    {
        Success = 0,
        Failure = 1,
        Warning = 2,
        Error = 3,
        Conflict = 4,
        Offline = 5
    }

    /// <summary>
    /// This is a copy of PE.Framework.Models.ServiceResult with a few small changes
    /// Needed so that Web and Mobile can share
    /// 
    /// The purpose of this class is to provide a unified response type for all service calls that do not return a HTTP response code < 500
    /// </summary>
    public class ServiceResult
    {
        #region Constructors

        public ServiceResult()
        {
            Status = ServiceResultStatus.Success;
            Message = string.Empty;
        }

        #endregion Constructors

        #region Properties

        [JsonProperty("status")]
        public ServiceResultStatus Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        #endregion Properties
    }

    public class ServiceResult<ResultType> : ServiceResult
    {
        #region Constructors

        /// <summary>
        /// Create a new generic ServiceResult
        /// </summary>
        public ServiceResult()
            : base()
        {
        }
        /// <summary>
        /// Create a new generic ServiceResult with a payload and successful status
        /// </summary>
        /// <param name="payload">Generic Payload</param>
        public ServiceResult(ResultType payload)
            : base()
        {
            Payload = payload;
        }

        #endregion Constructors

        #region Properties

        [JsonProperty("payload")]
        public ResultType Payload { get; set; }

        #endregion Properties
    }
}
