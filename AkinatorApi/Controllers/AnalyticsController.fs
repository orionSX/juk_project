namespace AkinatorApi.Controllers

open System
open Microsoft.AspNetCore.Mvc
open AkinatorApi.Services
open AkinatorApi.Models

[<ApiController>]
[<Route("[controller]")>]
type AnalyticsController(databaseService: DatabaseService) =
    inherit ControllerBase()

    [<HttpGet("user/{userId}")>]
    member this.GetUserStats(userId: string) =
        let stats = databaseService.GetUserStats(userId)
        Ok(stats)

    [<HttpGet("characters/most-guessed")>]
    member this.GetMostGuessedCharacters() =
        let characters = databaseService.GetMostGuessedCharacters()
        Ok(characters)

    [<HttpGet("characters/most-added")>]
    member this.GetMostAddedCharacters() =
        let characters = databaseService.GetMostAddedCharacters()
        Ok(characters)

    [<HttpGet("questions/most-used")>]
    member this.GetMostUsedQuestions() =
        let questions = databaseService.GetMostUsedQuestions()
        Ok(questions)

    [<HttpGet("tree")>]
    member this.GetQuestionTree() =        
        Ok({ Question = None; Character = None; IsComplete = false; Message = "Question tree visualization will be implemented" }) 
