namespace AkinatorApi.Controllers

open Microsoft.AspNetCore.Mvc
open AkinatorApi.Models
open AkinatorApi.Services

[<ApiController>]
[<Route("api/[controller]")>]
type AuthController(authService: AuthService) =
    inherit ControllerBase()

    [<HttpPost("register")>]
    member _.Register([<FromBody>] request: RegisterRequest) =
        async {
            let! result = authService.RegisterUser request
            return Ok result
        }

    [<HttpPost("login")>]
    member _.Login([<FromBody>] request: LoginRequest) =
        async {
            let! result = authService.LoginUser request
            return Ok result
        } 
