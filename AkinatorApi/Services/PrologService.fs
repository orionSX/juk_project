namespace AkinatorApi.Services

open System
open System.Diagnostics
open AkinatorApi.Models

type PrologService() =
    let prologProcess = new Process()
    
    do
        prologProcess.StartInfo.FileName <- "swipl"
        prologProcess.StartInfo.Arguments <- "-s PrologKnowledgeBase/characters.pl"
        prologProcess.StartInfo.UseShellExecute <- false
        prologProcess.StartInfo.RedirectStandardInput <- true
        prologProcess.StartInfo.RedirectStandardOutput <- true
        prologProcess.StartInfo.CreateNoWindow <- true
        prologProcess.Start() |> ignore

    member this.ExecuteQuery(query: string) =
        prologProcess.StandardInput.WriteLine(query)
        prologProcess.StandardInput.Flush()
        let response = prologProcess.StandardOutput.ReadLine()
        response

    member this.GetNextQuestion(answers: Map<int, Answer>) =
        let answerString = 
            answers
            |> Map.toList
            |> List.map (fun (id, ans) -> 
                let answerStr = 
                    match ans with
                    | Yes -> "yes"
                    | No -> "no"
                    | Unknown -> "unknown"
                sprintf "(%d, %s)" id answerStr)
            |> String.concat ", "
        
        let query = sprintf "get_next_question([%s], Question)." answerString
        let response = this.ExecuteQuery(query)
        
        match response with
        | "false" -> None
        | questionText -> 
            Some { 
                Id = answers.Count + 1
                Text = questionText 
            }

    member this.GuessCharacter(answers: Map<int, Answer>) =
        let answerString = 
            answers
            |> Map.toList
            |> List.map (fun (id, ans) -> 
                let answerStr = 
                    match ans with
                    | Yes -> "yes"
                    | No -> "no"
                    | Unknown -> "unknown"
                sprintf "(%d, %s)" id answerStr)
            |> String.concat ", "
        
        let query = sprintf "can_identify_character([%s], Character)." answerString
        let response = this.ExecuteQuery(query)
        
        match response with
        | "false" -> None
        | characterName -> 
            let propertiesQuery = sprintf "findall(Property-Value, has_property(%s, Property, Value), Properties)." characterName
            let propertiesResponse = this.ExecuteQuery(propertiesQuery)
            
            let properties = 
                propertiesResponse.Split(',')
                |> Array.map (fun prop -> 
                    let [|propName; value|] = prop.Split('-')
                    let answer = 
                        match value.Trim() with
                        | "yes" -> Yes
                        | "no" -> No
                        | _ -> Unknown
                    (propName.Trim(), answer))
                |> Map.ofArray
            
            Some { 
                Id = 1
                Name = characterName
                Properties = properties 
            }

    member this.AddCharacter(name: string, properties: Map<string, Answer>) =
        let propertiesString = 
            properties
            |> Map.toList
            |> List.map (fun (prop, ans) -> 
                let answerStr = 
                    match ans with
                    | Yes -> "yes"
                    | No -> "no"
                    | Unknown -> "unknown"
                sprintf "(%s, %s)" prop answerStr)
            |> String.concat ", "
        
        let query = sprintf "add_character(%s, [%s])." name propertiesString
        let response = this.ExecuteQuery(query)
        response = "true"

    member this.GetMatchingCharacters(answers: Map<int, Answer>) =
        let answerString = 
            answers
            |> Map.toList
            |> List.map (fun (id, ans) -> 
                let answerStr = 
                    match ans with
                    | Yes -> "yes"
                    | No -> "no"
                    | Unknown -> "unknown"
                sprintf "(%d, %s)" id answerStr)
            |> String.concat ", "
        
        let query = sprintf "get_matching_characters([%s], Characters)." answerString
        let response = this.ExecuteQuery(query)
        
        match response with
        | "false" -> []
        | characters -> 
            characters.Split(',')
            |> Array.map (fun name -> 
                { 
                    Id = 1
                    Name = name.Trim()
                    Properties = Map.empty
                })
            |> Array.toList

    interface IDisposable with
        member this.Dispose() =
            prologProcess.Dispose() 
