﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace LT.DigitalOffice.OfficeService
{
  public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
  {
    //TODO: move into kernel
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
      _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
      if (!_validators.Any())
      {
        return await next();
      }

      ValidationContext<TRequest> context = new(request);
      ValidationResult[] validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
      List<ValidationFailure> failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
      if (failures.Count != 0)
        throw new ValidationException(failures);
      return await next();
    }
  }
}