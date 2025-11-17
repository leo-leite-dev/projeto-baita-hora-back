namespace BaitaHora.Application.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Code { get; }
        public string? Title { get; }
        public string? Error { get; }

        public IReadOnlyDictionary<string, object?> Metadata { get; }

        protected Result(bool isSuccess, string code, string? title, string? error, IDictionary<string, object?>? meta = null)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Result.Code é obrigatório.");
            if (isSuccess && error is not null)
                throw new InvalidOperationException("Success result cannot have an error.");
            if (!isSuccess && string.IsNullOrWhiteSpace(error))
                throw new InvalidOperationException("Failure result must have an error message.");

            IsSuccess = isSuccess;
            Code = code;
            Title = title;
            Error = error;
            Metadata = meta is null ? new Dictionary<string, object?>() : new Dictionary<string, object?>(meta);
        }      

        // ===================== FALHA =====================
        public static Result BadRequest(string message, string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Generic.BadRequest, title, message, meta);

        public static Result Unauthorized(string message = "Não autorizado.", string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Auth.Unauthorized, title, message, meta);

        public static Result Forbidden(string message = "Acesso negado.", string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Auth.Forbidden, title, message, meta);

        public static Result NotFound(string message = "Recurso não encontrado.", string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.NotFound.Generic, title, message, meta);

        public static Result Conflict(string message, string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Conflict.Generic, title, message, meta);

        public static Result ServerError(string message, string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Generic.ServerError, title, message, meta);

        // ===================== HELPERS =====================
        public Result WithMeta(string key, object? value)
        {
            var dict = new Dictionary<string, object?>(Metadata) { [key] = value };
            return new Result(IsSuccess, Code, Title, Error, dict);
        }
    }

    public sealed class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool ok, string code, string? title, string? error, T? value, IDictionary<string, object?>? meta = null)
            : base(ok, code, title, error, meta)
        {
            Value = value;
        }

        // ===================== SUCESSO =====================
        public static Result<T> Ok(T? value = default, string? title = null)
            => new(true, ResultCodes.Generic.Ok, title, null, value);
        public static Result<T> Created(T? value = default, string? title = null)
            => new(true, ResultCodes.Generic.Created, title, null, value);

        public static Result<T> NoContent(T? value = default, string? title = null)
            => new(true, ResultCodes.Generic.NoContent, title, null, value);

        // ===================== FALHA =====================
        public new static Result<T> Fail(string code, string message, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code, title, message, default, meta);

        public new static Result<T> BadRequest(string message, string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Generic.BadRequest, title, message, default, meta);

        public new static Result<T> Unauthorized(string message = "Não autorizado.", string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Auth.Unauthorized, title, message, default, meta);

        public new static Result<T> Forbidden(string message = "Acesso negado.", string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Auth.Forbidden, title, message, default, meta);

        public new static Result<T> NotFound(string message = "Recurso não encontrado.", string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.NotFound.Generic, title, message, default, meta);

        public new static Result<T> Conflict(string message, string? code = null, string? title = null, IDictionary<string, object?>? meta = null)
            => new(false, code ?? ResultCodes.Conflict.Generic, title, message, default, meta);

        // ===================== CONVERSÕES / COMPOSIÇÃO =====================
        public static Result<T> FromError(Result src)
            => Fail(src.Code, src.Error ?? "Erro", src.Title, new Dictionary<string, object?>(src.Metadata));

        public Result<TOut> MapError<TOut>()
        {
            if (IsSuccess)
                throw new InvalidOperationException("MapError não deve ser usado em resultados de sucesso.");

            return Result<TOut>.Fail(Code, Error ?? "Erro", Title, new Dictionary<string, object?>(Metadata));
        }
    }
}