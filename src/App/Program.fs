open App
open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Elmish

type MainWindow() as this =
    inherit HostWindow()

    do
        base.Title <- "Hehe"
        base.Width <- 1280.0
        base.Height <- 720.0
        
        let itemStore = ItemStore.Observer()
        let subscriptions (state: Main.State) = Subscriptions.registerItemStore itemStore state

        Program.mkProgram Main.init (Main.update itemStore) Main.view
        |> Program.withHost this
        |> Program.withSubscription subscriptions
        |> Program.withConsoleTrace
        |> Program.run

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme ())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow ()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main (args : string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime (args)
