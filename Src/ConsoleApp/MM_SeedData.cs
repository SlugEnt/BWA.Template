namespace ConsoleApp;
public partial class MainMenu
{
    /// <summary>
    ///     Seeds the database with some preliminary values
    /// </summary>
    /// <returns></returns>
    public async Task SeedDataAsync()
    {
        //await SeedXyZAsync();

        await SeedSomeAppDataAsync();

    }


    /// <summary>
    ///     Seed Some Info
    /// </summary>
    /// <returns></returns>
    public async Task SeedSomeAppDataAsync()
    {
        try
        {

            await Db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            AppRuntime.Logger!.Error("SeedSomeAppDataAsync:  Error: {Error}  InnerError: {InnerError}",
                             ex.Message,
                             ex.InnerException != null ? ex.InnerException.Message : "N/A");
        }
    }

}