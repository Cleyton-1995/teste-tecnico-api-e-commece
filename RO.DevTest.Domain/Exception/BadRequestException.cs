using System.Net;
using System.Linq;
using System.Collections.Generic;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace RO.DevTest.Domain.Exception;

/// <summary>
/// Returns a <see cref="HttpStatusCode.BadRequest"/> to the request.
/// Used to standardize error responses for bad requests.
/// </summary>
public class BadRequestException : ApiException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public IEnumerable<string> Errors { get; }

    public BadRequestException(string error) : base(error)
    {
        Errors = new List<string> { error };
    }

    public BadRequestException(IEnumerable<string> errors) : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public BadRequestException(ValidationResult validationResult) 
        : this(validationResult.Errors.Select(e => e.ErrorMessage))
    {
    }

    public BadRequestException(IdentityResult result)
        : this(result.Errors.Select(e => e.Description))
    {
    }
}
