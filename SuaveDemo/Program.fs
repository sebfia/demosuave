open Suave
open Suave.Async
open Suave.Session
open Suave.Web
open Suave.Session
open Suave.Types
open Suave.Http
open Suave.Http.Successful
open Suave.Http.Response
open Suave.Http.Writers
open Suave.Http.Applicatives
open Suave.Http.Authentication
open Suave.Http.Files
open Suave.Log

// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
[<EntryPoint>]
let main argv = 
    let basic_auth : WebPart =
        Authentication.authenticate_basic ( fun (user_name,password) -> user_name.Equals("Testuser") && password.Equals("secret#password1"))
    let readContent (path: string) =
        System.IO.File.ReadAllText(path)
    choose [
            GET >>= url "/" >>= OK (readContent "Home Page.html")
            GET >>= url "/hello" >>= OK "Hello from public space"
            basic_auth
            GET >>= url "/protected" >>= context (fun x -> OK "Hello from secret space")
            ]
            |> web_server 
                {
                    bindings = [HttpBinding.Create(HTTP, "0.0.0.0", 8080)]; //necessary to have public access from outside the container
                    error_handler = default_error_handler;
                    listen_timeout = System.TimeSpan.FromSeconds(5.);
                    ct = Async.DefaultCancellationToken;
                    buffer_size = 2048;
                    max_ops = 1000;
                    mime_types_map = default_mime_types_map;
                    home_folder = None;
                    compressed_files_folder = None;
                    logger = Loggers.sane_defaults_for Debug;
                    session_provider = new DefaultSessionProvider()
                }
    System.Console.ReadKey(false) |> ignore
    0 // return an integer exit code

