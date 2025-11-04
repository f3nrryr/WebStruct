using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersRoles.Services.DTOs
{
    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ServiceResult Success() => new() { Succeeded = true };
        public static ServiceResult Failure(List<string> errors) => new() { Succeeded = false, Errors = errors };
        public static ServiceResult Failure(string error) => new() { Succeeded = false, Errors = new List<string> { error } };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; }

        public static ServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };
        public static new ServiceResult<T> Failure(List<string> errors) => new() { Succeeded = false, Errors = errors };
        public static new ServiceResult<T> Failure(string error) => new() { Succeeded = false, Errors = new List<string> { error } };
    }
}
