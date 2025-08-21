namespace BaitaHora.Application.Common;

public static class ResultCodes
{
    public static class Generic
    {
        public const string Ok = "generic.ok";
        public const string Created = "generic.created";
        public const string NoContent = "generic.no_content";
        public const string BadRequest = "generic.bad_request"; 
        public const string ServerError = "generic.server_error";
    }

    public static class Validation
    {
        public const string Invalid = "validation.invalid";            
        public const string MissingField = "validation.missing_field";
        public const string OutOfRange = "validation.out_of_range";
    }

    public static class Auth
    {
        public const string Unauthorized = "auth.unauthorized";               
        public const string InvalidCredentials = "auth.invalid_credentials";  
        public const string Forbidden = "auth.forbidden";                    
        public const string AccountDisabled = "auth.account_disabled";
        public const string TokenInvalid = "auth.token_invalid";
        public const string TokenExpired = "auth.token_expired";
    }

    public static class NotFound
    {
        public const string Generic = "not_found.generic";
        public const string User = "users.not_found";
        public const string Company = "company.not_found";
        public const string Member = "company.member_not_found";
        public const string Position = "company.position_not_found";
    }

    public static class Conflict
    {
        public const string Generic = "conflict.generic";
        public const string UniqueViolation = "conflict.unique_violation";     
        public const string AlreadyExists = "conflict.already_exists";
        public const string BusinessRule = "conflict.business_rule";
        public const string Concurrency = "conflict.concurrency";             
    }

    public static class Infra
    {
        public const string Timeout = "infra.timeout";
        public const string ServiceUnavailable = "infra.service_unavailable";
        public const string ExternalDependency = "infra.external_dependency_error";
    }

    public static class RateLimit
    {
        public const string TooManyRequests = "rate_limit.too_many_requests";
    }
}