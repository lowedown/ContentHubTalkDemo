# ContentHubTalkDemo
This repository contains some demo code for reference. 
It shows how to extend Sitecore Connect for Content Hub 4.0 to

* fetch custom metadata into the image field
* use a .net core facade service to communicate with ContentHub

*Disclaimer*: This code is purely for demo purposes and is not intended for production use.

## Setup

1. Set up a Sitecore 10.2 instance
1. Compile and deploy `src\ContentHubTalk\ContentHubTalk.sln` to your 10.2 instance
1. In Core database, change the `/sitecore/system/Field types/Simple Types/Image` item. Set the `Assembly` field to `ContentHubTalk` and the `Class` field to `ContentHubTalk.ImageField.MetadataEnabledImageField` 
1. Go to `src\ContentHubFacade\ContentHubFacade.sln` and compile (you'll need a reference to `Sitecore.Connector.ContentHub.DAM.dll` from the Sitecore Connect for ContentHub module)
1. Modify `appsettings.json` and set up your ContentHub endpoint and credentials
1. Launch the facade service through `bin\Debug\netcoreapp3.1\ContentHubFacade.exe` 


# Usage

You can call the facade service through
`https://localhost:5001/api/assets/<assetId>`
