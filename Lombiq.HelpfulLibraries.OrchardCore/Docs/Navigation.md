# Lombiq Helpful Libraries - Orchard Core Libraries - Navigation for Orchard Core



## NavigationProviderBase

An abstract base class for creating reducing boilerplate in `INavigationProvider` for checking the name parameter and if the user is authenticated. 


## MainMenuNavigationProviderBase

An abstract base class derived from `NavigationProviderBase` for creating home page menu structure using the `main` navigation name. If you use the _Lombiq.BaseTheme_ it automatically displays the generated menu as a widget in the _Navigation_ zone.


## Extensions

- `NavigationItemBuilderExtensions`: Adds extension methods for building menu [`TypedRoute`](https://github.com/Lombiq/Helpful-Libraries/blob/dev/Lombiq.HelpfulLibraries.OrchardCore/Mvc/TypedRoute.cs) style.

