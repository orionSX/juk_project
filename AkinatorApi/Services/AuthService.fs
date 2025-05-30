namespace AkinatorApi.Services

open System
open System.Security.Cryptography
open System.Text
open AkinatorApi.Models
open Microsoft.AspNetCore.Cryptography.KeyDerivation

type AuthService() =
    let mutable users: User list = []

    let hashPassword (password: string) =
        let salt = RandomNumberGenerator.GetBytes(128 / 8)
        let hash = KeyDerivation.Pbkdf2(
            password = password,
            salt = salt,
            prf = KeyDerivationPrf.HMACSHA256,
            iterationCount = 10000,
            numBytesRequested = 256 / 8)
        Convert.ToBase64String(hash)

    let verifyPassword (password: string) (hash: string) =
        let computedHash = hashPassword password
        computedHash = hash

    member _.RegisterUser(request: RegisterRequest) =
        async {
            if users |> List.exists (fun u -> u.Email = request.Email) then
                return {
                    Success = false
                    Token = None
                    Message = "User with this email already exists"
                }
            else
                let newUser = {
                    Id = Guid.NewGuid()
                    Name = request.Name
                    Email = request.Email
                    PasswordHash = hashPassword request.Password
                    CreatedAt = DateTime.UtcNow
                }
                users <- newUser :: users
                return {
                    Success = true
                    Token = Some "dummy-token"
                    Message = "Registration successful"
                }
        }

    member _.LoginUser(request: LoginRequest) =
        async {
            match users |> List.tryFind (fun u -> u.Email = request.Email) with
            | Some user ->
                if verifyPassword request.Password user.PasswordHash then
                    return {
                        Success = true
                        Token = Some "dummy-token" 
                        Message = "Login successful"
                    }
                else
                    return {
                        Success = false
                        Token = None
                        Message = "Invalid password"
                    }
            | None ->
                return {
                    Success = false
                    Token = None
                    Message = "User not found"
                }
        } 
