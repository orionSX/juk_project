namespace AkinatorApi.Controllers

open System
open Microsoft.AspNetCore.Mvc
open AkinatorApi.Models
open AkinatorApi.Services

[<ApiController>]
[<Route("[controller]")>]
type GameController(prologService: PrologService) =
    inherit ControllerBase()

    [<HttpPost("start")>]
    member this.StartGame([<FromBody>] userId: string) =
        let session = {
            Id = Guid.NewGuid()
            UserId = userId
            StartTime = DateTime.UtcNow
            EndTime = None
            Questions = []
            Answers = Map.empty
            GuessedCharacter = None
            AddedCharacter = None
        }
        
        let firstQuestion = prologService.GetNextQuestion(Map.empty)
        {
            Question = firstQuestion
            Character = None
            IsComplete = false
            Message = "Game started"
        }

    [<HttpPost("answer")>]
    member this.SubmitAnswer([<FromBody>] answer: Map<int, Answer>) =
        match prologService.GuessCharacter(answer) with
        | Some character ->
            {
                Question = None
                Character = Some character
                IsComplete = true
                Message = "Character guessed!"
            }
        | None ->
            let nextQuestion = prologService.GetNextQuestion(answer)
            {
                Question = nextQuestion
                Character = None
                IsComplete = false
                Message = "Next question"
            }

    [<HttpPost("add-character")>]
    member this.AddCharacter([<FromBody>] request: AddCharacterRequest) =
        let success = prologService.AddCharacter(request.Name, request.Properties)
        if success then
            Ok({ Question = None; Character = None; IsComplete = false; Message = "Character added successfully" })
        else
            Error({ Question = None; Character = None; IsComplete = false; Message = "Failed to add character" })

    [<HttpPost("add-question")>]
    member this.AddQuestion([<FromBody>] request: AddQuestionRequest) =
        Ok({ Question = None; Character = None; IsComplete = false; Message = "Question added successfully" }) 
