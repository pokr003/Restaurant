using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Restaurant.API.Models.Customer;
using Restaurant.API.Models.User;
using Restaurant.API.Security.Configurations;
using Restaurant.API.Security.Models;
using Restaurant.API.Services;

namespace Restaurant.API.Controllers;

[ApiController]
[Route("customers")]
public sealed class CustomerController(
      ICustomerService customerService,
      IAuthService authService,
      IOptions<JwtOptions> jwtOptions
) : ControllerBase
{
      private readonly ICustomerService _customerService = customerService;
      private readonly IAuthService _authService = authService;
      private readonly JwtOptions _jwtOptions = jwtOptions.Value;

      [TranslateResultToActionResult]
      [ExpectedFailures(ResultStatus.NotFound)]
      [HttpGet("{id:guid}")]
      public async Task<Result<CustomerResponse>> GetCustomerById([FromRoute(Name = "id")] Guid id) =>
            await _customerService.GetCustomerByIdAsync(id);

      [TranslateResultToActionResult]
      [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
      [HttpGet()]
      public async Task<Result<CustomerResponse>> GetCustomerByEmail([FromQuery(Name = "email")] string email) =>
            await _customerService.GetCustomerByEmailAsync(email);

      [TranslateResultToActionResult]
      [ExpectedFailures(ResultStatus.Conflict, ResultStatus.Invalid)]
      [HttpPost]
      public async Task<Result<CustomerResponse>> CreateCustomer([FromBody] CreateCustomerModel createCustomerModel) =>
          await _customerService.CreateCustomerAsync(createCustomerModel);

      [TranslateResultToActionResult]
      [Authorize(AuthorizationPolicies.RequireCustomer)]
      [ExpectedFailures(ResultStatus.Invalid, ResultStatus.NotFound, ResultStatus.Error)]
      [HttpPatch("{id:guid}")]
      public async Task<Result<CustomerResponse>> UpdateCustomer(
         [FromRoute(Name = "id")] Guid id,
         [FromBody] UpdateCustomerModel updateCustomerModel) =>
            await _customerService.UpdateCustomerAsync(id, User.Adapt<AuthenticatedUser>(), updateCustomerModel);

      [TranslateResultToActionResult]
      [Authorize(AuthorizationPolicies.RequireCustomer)]
      [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Error)]
      [HttpDelete("{id:guid}")]
      public async Task<Result> RemoveCustomer([FromRoute(Name = "id")] Guid id) =>
             await _customerService.RemoveCustomerAsync(id, User.Adapt<AuthenticatedUser>());

      [TranslateResultToActionResult]
      [ExpectedFailures(ResultStatus.Invalid, ResultStatus.Error, ResultStatus.NotFound)]
      [HttpPost("authentication")]
      public async Task<Result<LoginCustomerResponse>> LoginCustomerAsync([FromBody] LoginUserModel loginUserModel)
      {
            string? audience = Request.Headers.FirstOrDefault(h => h.Key == "Audience").Value;

            if (string.IsNullOrEmpty(audience))
                  return Result.Error("audience header not set or value is empty");

            if (!_jwtOptions.Audiences.Contains(audience))
                  return Result.Error("incorrect audience value in header");

            return await _authService.LoginCustomerAsync(audience, loginUserModel);
      }
}
