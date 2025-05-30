namespace AkinatorApi.Models

open System
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema
open Microsoft.EntityFrameworkCore
open System.Linq.Expressions
open System.Collections.Generic

[<CLIMutable>]
type DbCharacter = {
    [<Key>]
    Id: int
    Name: string
    Properties: ICollection<DbCharacterProperty>
    GuessedInSessions: ICollection<DbGameSession>
    AddedInSessions: ICollection<DbGameSession>
}
and [<CLIMutable>]
  DbCharacterProperty = {
    [<Key>]
    Id: int
    CharacterId: int
    PropertyName: string
    Value: string
    [<ForeignKey("CharacterId")>]
    Character: DbCharacter option
}
and [<CLIMutable>]
  DbQuestion = {
    [<Key>]
    Id: int
    Text: string
    CreatedBy: string
    CreatedAt: DateTime
    Answers: ICollection<DbAnswer>
}
and [<CLIMutable>]
  DbUser = {
    [<Key>]
    Id: string
    Username: string
    CreatedAt: DateTime
    GameSessions: ICollection<DbGameSession>
} 
and [<CLIMutable>]
  DbGameSession = {
    [<Key>]
    Id: Guid
    UserId: string
    StartTime: DateTime
    EndTime: DateTime option
    Questions: ICollection<DbQuestion>
    Answers: ICollection<DbAnswer>
    GuessedCharacter: DbCharacter option
    AddedCharacter: DbCharacter option
    [<ForeignKey("UserId")>]
    User: DbUser option
}
and [<CLIMutable>]
  DbAnswer = {
    [<Key>]
    Id: int
    QuestionId: int
    GameSessionId: Guid
    Value: string
    [<ForeignKey("QuestionId")>]
    Question: DbQuestion option
    [<ForeignKey("GameSessionId")>]
    GameSession: DbGameSession option
}

type AkinatorDbContext(options: DbContextOptions<AkinatorDbContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable private users: DbSet<DbUser>
    member this.Users
        with get() = this.users
        and set v = this.users <- v

    [<DefaultValue>]
    val mutable private gameSessions: DbSet<DbGameSession>
    member this.GameSessions
        with get() = this.gameSessions
        and set v = this.gameSessions <- v

    [<DefaultValue>]
    val mutable private questions: DbSet<DbQuestion>
    member this.Questions
        with get() = this.questions
        and set v = this.questions <- v

    [<DefaultValue>]
    val mutable private answers: DbSet<DbAnswer>
    member this.Answers
        with get() = this.answers
        and set v = this.answers <- v

    [<DefaultValue>]
    val mutable private characters: DbSet<DbCharacter>
    member this.Characters
        with get() = this.characters
        and set v = this.characters <- v

    [<DefaultValue>]
    val mutable private characterProperties: DbSet<DbCharacterProperty>
    member this.CharacterProperties
        with get() = this.characterProperties
        and set v = this.characterProperties <- v

    override this.OnModelCreating(modelBuilder: ModelBuilder) =
        base.OnModelCreating(modelBuilder)

        // Configure keys
        modelBuilder.Entity<DbUser>().HasKey(fun u -> u.Id :> obj) |> ignore
        modelBuilder.Entity<DbGameSession>().HasKey(fun g -> g.Id :> obj) |> ignore
        modelBuilder.Entity<DbQuestion>().HasKey(fun q -> q.Id :> obj) |> ignore
        modelBuilder.Entity<DbAnswer>().HasKey(fun a -> a.Id :> obj) |> ignore
        modelBuilder.Entity<DbCharacter>().HasKey(fun c -> c.Id :> obj) |> ignore
        modelBuilder.Entity<DbCharacterProperty>().HasKey(fun p -> p.Id :> obj) |> ignore

        // Configure relationships with proper expression types
        modelBuilder.Entity<DbUser>()
            .HasMany(fun u -> u.GameSessions :> IEnumerable<DbGameSession>)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade) |> ignore

        modelBuilder.Entity<DbQuestion>()
            .HasMany(fun q -> q.Answers :> IEnumerable<DbAnswer>)
            .WithOne()
            .HasForeignKey("QuestionId")
            .OnDelete(DeleteBehavior.Cascade) |> ignore

        modelBuilder.Entity<DbGameSession>()
            .HasMany(fun g -> g.Answers :> IEnumerable<DbAnswer>)
            .WithOne()
            .HasForeignKey("GameSessionId")
            .OnDelete(DeleteBehavior.Cascade) |> ignore

        modelBuilder.Entity<DbCharacter>()
            .HasMany(fun c -> c.Properties :> IEnumerable<DbCharacterProperty>)
            .WithOne()
            .HasForeignKey("CharacterId")
            .OnDelete(DeleteBehavior.Cascade) |> ignore

        // Configure many-to-many relationships
        modelBuilder.Entity<DbCharacter>()
            .HasMany(fun c -> c.GuessedInSessions :> System.Collections.Generic.IEnumerable<DbGameSession>)
            .WithMany("")
            .UsingEntity(fun (j: Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder) ->
                j.ToTable("CharacterGuessedInSessions") |> ignore
            ) |> ignore

        modelBuilder.Entity<DbCharacter>()
            .HasMany(fun c -> c.AddedInSessions :> System.Collections.Generic.IEnumerable<DbGameSession>)
            .WithMany("")
            .UsingEntity(fun (j: Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder) ->
                j.ToTable("CharacterAddedInSessions") |> ignore
            ) |> ignore

        modelBuilder.Entity<DbGameSession>()
            .HasMany(fun g -> g.Questions :> System.Collections.Generic.IEnumerable<DbQuestion>)
            .WithMany("")
            .UsingEntity(fun (j: Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder) ->
                j.ToTable("GameSessionQuestions") |> ignore
            ) |> ignore 
