namespace GamaEdtech.Backend.Common.Data
{
    using System.Collections.Generic;

    using static GamaEdtech.Backend.Common.Core.Constants;

    public struct ResultData<T>(OperationResult operationResult)
    {
        public T? Data { get; set; }

        public OperationResult OperationResult { get; set; } = operationResult;

        public IEnumerable<Error>? Errors { get; set; }
    }
}
