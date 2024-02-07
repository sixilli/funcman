open App
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "Hehe"
        base.Width <- 1280.0
        base.Height <- 720.0
        this.Content <- Main.view
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
