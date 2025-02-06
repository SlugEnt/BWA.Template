# SlugEnt Blazor Web App Template

This template serves as a starting point for building a .Net compatible Blazor Web Assembly app with Auto Rendering.

It has Authentication and Authorization built in out of the box.

It has a Database Project and an Entities project which you may or may not need depending on your requirements.  If you do not need a database
then just remove these projects and remove the code in the ProgramCustom that references them.

## Some Key Features
You should never need to update anything in the Program.cs file.  All customizations should be done in the ProgramCustom.cs file in the 
appropriate methods.  Methods are clearly documented and named as to where they go and what they do.

If you find yourself needing to modify the Program.cs file, please let me know so I can add a customization point for you.

You should not really need to rename the Client or the Server Apps.  The only thing you should need to change is the Server's Assembly Name.

# # Getting Started
1. Reset the BWA.Server projects UserSecretsId!  Failure to do so will cause issues later with this template's running and your app.
        1. Open the BWA's Server project's .csproj file and delete the UserSecretsId line.  
        2. Open the BWA's Server project's Properties/launchSettings.json file and delete the UserSecretsId line.
        3.  Open the Developer Powershell window and run the following command:
            * dotnet user-secrets init
            * dotnet user-secrets set "AzureAd:ClientSecret" "your secret from Azure"

## Azure Registration
You will need to register your app in Azure AD.  
* You need to set the Redirect URI to https://localhost:/signin-oidc
* You need to update the Appsettings.json with the Tenant and ClientId from Azure.
* You will also need to add the Client Secret via the Dotnet User Secrets manager
        * dotnet user-secrets set "AzureAd:ClientSecret" "your secret code"

## Database
The database is a SQL Server database.  All database updates can be done thru the Database project due to it including Design Time objects.

### Setting up the Database.
The database connection string file is currently hard coded in the Database-->DesignTimebContextFactory.  Edit it appropriately.
* You will need to delete any existing migrations since you are starting your project fresh.
* Set up your first Entities.  
* Go to the Developer Powershell and run:
    * Dotnet ef migrations add InitialCreate
    * Dotnet ef database update
    




