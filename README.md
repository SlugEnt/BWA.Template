# SlugEnt Blazor Web App Template

This template serves as a starting point for building a .Net compatible Blazor Web Assembly app with Auto Rendering.

It has a number of key things to get you jump started into just building your app:
* Authentication and Authorization are built in out of the box using Microsoft MSAL
* Heavy use of Generica to reduce the amount of code you need to write.
* Uses Microsoft OData to provide query parameters for the data models.  Note, it does not fully provide an OData interface, but that is rarely needed and prevents you from 
* being able to build outside of the pure Rest model.  I take the best of OData and leave the rest out.
* It has a complete database Entity model that works throughout the entire app to build a modern app that means just a few lines of code for an entity will provide:
    * An entity with auditing entries (Created/Modified) by Id and DateTimeUTC.  
    * Name Property for the entity that is used in the UI.
    * IsActive flags for deactiving entities if deletion is not wanted
    * A Can Delete property that allows the entity to tell components whether it can be permanently deleted or not*
    * It has built in base entity classes that provide Id types for Int, Long, String, Guid and ULID!
    * Database Repositories that support each of the above entity types and provide the normal CRUD operations with full logging built in
    * Controllers that provide the typical CRUD operations for each of the Entity Types
    * Blazor components that provide Service objects for working with the entities in Blazor
    * Sample Drop down controls for the Entity types that make it easy to standup these entities in related entities
    * Full unit tests of the entities, the repositories and the controllers
* Has an imnproved database transaction model that allows multiple entities or repositories to use transactions without complicated logic to determine if already in one or not.*

With the above, you can create a basic new entity in less than 5 minutes that provides a Repository, Controller, Service, Unit tests for each, and copy paste a sample blazor component and unit test that  you can immediately begin working with!

All of the repository, controller and services are built such that you can override any of their methods for a given entity if you need to do something different than the base implementation.  


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
    




