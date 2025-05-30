namespace AkinatorApi.Models

open System

type Answer = Yes | No | Unknown

type Question = {
    Id: int
    Text: string
}

type Character = {
    Id: int
    Name: string
    Properties: Map<string, Answer>
}

type GameSession = {
    Id: Guid
    UserId: string
    StartTime: DateTime
    EndTime: DateTime option
    Questions: Question list
    Answers: Map<int, Answer>
    GuessedCharacter: Character option
    AddedCharacter: Character option
}

type User = {
    Id: string
    Username: string
    CreatedAt: DateTime
}

type GameResponse = {
    Question: Question option
    Character: Character option
    IsComplete: bool
    Message: string
}

type AddCharacterRequest = {
    Name: string
    Properties: Map<string, Answer>
}

type AddQuestionRequest = {
    Text: string
    CharacterId: int
    Answer: Answer
} 
