namespace AkinatorApi.Services

open System
open System.Linq
open Microsoft.EntityFrameworkCore
open AkinatorApi.Models
open System.Collections.Generic

type UserStats = {
    TotalGames: int
    SuccessfulGuesses: int
    AverageQuestionsPerGame: float
}

type DatabaseService(dbContext: AkinatorDbContext) =
    
   
    member this.CreateUser(username: string) =
        let user = {
            Id = Guid.NewGuid().ToString()
            Username = username
            CreatedAt = DateTime.UtcNow
            GameSessions = List<DbGameSession>() :> ICollection<DbGameSession>
        }
        dbContext.Users.Add(user) |> ignore
        dbContext.SaveChanges() |> ignore
        Some user

    member this.GetUser(userId: string) =
        dbContext.Users
            .Include(fun u -> u.GameSessions)
            .Where(fun u -> u.Id = userId)
            .AsEnumerable()
            |> Seq.tryHead

    
    member this.CreateGameSession(userId: string) =
        match this.GetUser(userId) with
        | None -> None
        | Some user ->
            let session = {
                Id = Guid.NewGuid()
                UserId = userId
                StartTime = DateTime.UtcNow
                EndTime = None
                Questions = List<DbQuestion>() :> ICollection<DbQuestion>
                Answers = List<DbAnswer>() :> ICollection<DbAnswer>
                GuessedCharacter = None
                AddedCharacter = None
                User = Some user
            }
            dbContext.GameSessions.Add(session) |> ignore
            dbContext.SaveChanges() |> ignore
            Some session

    member this.UpdateGameSession(sessionId: Guid, endTime: DateTime option, guessedCharacter: DbCharacter option, addedCharacter: DbCharacter option) =
        dbContext.GameSessions
            .Where(fun g -> g.Id = sessionId)
            .AsEnumerable()
            |> Seq.tryHead
        |> Option.map (fun session ->
            let updatedSession = {
                session with
                    EndTime = endTime
                    GuessedCharacter = guessedCharacter
                    AddedCharacter = addedCharacter
            }
            dbContext.GameSessions.Update(updatedSession) |> ignore
            dbContext.SaveChanges() |> ignore
            updatedSession
        )

   
    member this.CreateQuestion(text: string, createdBy: string) =
        let question = {
            Id = 0 
            Text = text
            CreatedBy = createdBy
            CreatedAt = DateTime.UtcNow
            Answers = List<DbAnswer>() :> ICollection<DbAnswer>
        }
        dbContext.Questions.Add(question) |> ignore
        dbContext.SaveChanges() |> ignore
        Some question

    member this.GetQuestion(questionId: int) =
        dbContext.Questions
            .Include(fun q -> q.Answers)
            .Where(fun q -> q.Id = questionId)
            .AsEnumerable()
            |> Seq.tryHead

   
    member this.AddAnswer(questionId: int, gameSessionId: Guid, value: string) =
        match this.GetQuestion(questionId),
              dbContext.GameSessions.Where(fun g -> g.Id = gameSessionId).AsEnumerable() |> Seq.tryHead with
        | Some question, Some gameSession ->
            let answer = {
                Id = 0 
                QuestionId = questionId
                GameSessionId = gameSessionId
                Value = value
                Question = Some question
                GameSession = Some gameSession
            }
            dbContext.Answers.Add(answer) |> ignore
            dbContext.SaveChanges() |> ignore
            Some answer
        | _ -> None

   
    member this.CreateCharacter(name: string, properties: (string * string) list) =
        let character = {
            Id = 0 
            Name = name
            Properties = List<DbCharacterProperty>() :> ICollection<DbCharacterProperty>
            GuessedInSessions = List<DbGameSession>() :> ICollection<DbGameSession>
            AddedInSessions = List<DbGameSession>() :> ICollection<DbGameSession>
        }
        dbContext.Characters.Add(character) |> ignore
        dbContext.SaveChanges() |> ignore

       
        for (propName, value) in properties do
            let property = {
                Id = 0 
                CharacterId = character.Id
                PropertyName = propName
                Value = value
                Character = Some character
            }
            dbContext.CharacterProperties.Add(property) |> ignore
        dbContext.SaveChanges() |> ignore
        Some character

    member this.GetCharacter(characterId: int) =
        dbContext.Characters
            .Include(fun c -> c.Properties)
            .Where(fun c -> c.Id = characterId)
            .AsEnumerable()
            |> Seq.tryHead

   
    member this.GetUserStats(userId: string) =
        let sessions =
            dbContext.GameSessions
                .Include(fun s -> s.Answers)
                .Include(fun s -> s.GuessedCharacter)
                .Where(fun s -> s.UserId = userId)
                .ToList()
        let totalGames = sessions.Count
        let successfulGuesses = 
            sessions
            |> Seq.filter (fun s -> s.GuessedCharacter.IsSome)
            |> Seq.length
        let averageQuestionsPerGame = 
            if totalGames > 0 then
                float (sessions |> Seq.sumBy (fun s -> s.Answers.Count)) / float totalGames
            else 0.0
        { TotalGames = totalGames
          SuccessfulGuesses = successfulGuesses
          AverageQuestionsPerGame = averageQuestionsPerGame }

    member this.GetMostGuessedCharacters() =
        dbContext.Characters
            .Include(fun c -> c.GuessedInSessions)
            .ToList()
            |> Seq.sortByDescending (fun c -> c.GuessedInSessions.Count)
            |> Seq.truncate 10
            |> Seq.map (fun c -> (c.Name, c.GuessedInSessions.Count))
            |> Seq.toList

    member this.GetMostAddedCharacters() =
        dbContext.Characters
            .Include(fun c -> c.AddedInSessions)
            .ToList()
            |> Seq.sortByDescending (fun c -> c.AddedInSessions.Count)
            |> Seq.truncate 10
            |> Seq.map (fun c -> (c.Name, c.AddedInSessions.Count))
            |> Seq.toList

    member this.GetMostUsedQuestions() =
        dbContext.Questions
            .Include(fun q -> q.Answers)
            .ToList()
            |> Seq.sortByDescending (fun q -> q.Answers.Count)
            |> Seq.truncate 10
            |> Seq.map (fun q -> (q.Text, q.Answers.Count))
            |> Seq.toList 
