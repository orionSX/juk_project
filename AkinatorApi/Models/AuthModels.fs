namespace AkinatorApi.Models

open System

type LoginRequest = {
    Email: string
    Password: string
}

type RegisterRequest = {
    Name: string
    Email: string
    Password: string
}

type AuthResponse = {
    Success: bool
    Token: string option
    Message: string
}

type User = {
    Id: Guid
    Name: string
    Email: string
    PasswordHash: string
    CreatedAt: DateTime
} 
