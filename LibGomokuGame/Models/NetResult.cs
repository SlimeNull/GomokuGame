using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibGomokuGame.Models
{
    public class NetResult<T> where T : class
    {
        [JsonConstructor]
        public NetResult(bool isSuccess, int errorCode, string? message, T? data)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Message = message;
            Data = data;

            if (data is not default(T))
                IsSuccess = true;
        }

        [MemberNotNullWhen(true, nameof(Data))]
        public bool IsSuccess { get; }
        public int ErrorCode { get; } = -1;
        public string? Message { get; }

        public T? Data { get; }

        public static NetResult<T> Ok(T data)
        {
            return new NetResult<T>(true, -1, null, data);
        }

        public static NetResult<T> Err(int code, string? message)
        {
            return new NetResult<T>(false, code, message, default);
        }
    }
}
