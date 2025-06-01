using System.Text.RegularExpressions;

namespace SlugEnt.BWA.Common.Functions;

/// <summary>
/// Sets of funtions used to work with entity style fields.
/// </summary>
public static class FieldFunctions
{
#region "Email Functions"

    /// <summary>
    /// Sanitizes an email, by making sure there are no starting or ending spaces.  The email you store should be the result of this call!
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static string EmailSanitize(string email)
    {
        string trimmedEmail = email.Trim();
        return trimmedEmail;
    }



    /// <summary>
    /// Returns true if the email conforms to standards. Note:  This is the generally accepted standard for email validation.  It is not perfect and does not guaranter the email is valid,
    /// just that it conforms to the most common standards.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmail(string email)
    {
        if (email.EndsWith("."))
            return false;

        string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
        var    regex   = new Regex(pattern, RegexOptions.IgnoreCase);
        return regex.IsMatch(email);
    }

#endregion


#region "Zip Code Functions"

    /// <summary>
    /// Sanitizes a Zip Code by trimming any leading or trailing spaces
    /// </summary>
    /// <param name="zipCode"></param>
    /// <returns></returns>
    public static string ZipCodeSanitize(string zipCode)
    {
        return zipCode.Trim();
    }


    /// <summary>
    /// Confirms that the given string is a valid Zip Code.  Will accept both 5 and 9 digit zip codes, as well
    /// as zip codes with a dash in the middle (5 digit - 4 digit)
    /// </summary>
    /// <param name="zipCode"></param>
    /// <returns>True if valid zip code, false if not.</returns>
    public static bool IsValidZipCode(string zipCode,
                                      bool shouldSanitize = true)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
            return false;

        string sanitizedZipCode = shouldSanitize == false ? zipCode : ZipCodeSanitize(zipCode);

        // Pattern for 5-digit or 9-digit zip code with optional dash
        string pattern = @"^\d{5}(-?\d{4})?$";
        var    regex   = new Regex(pattern);

        return regex.IsMatch(sanitizedZipCode);
    }


    /// <summary>
    /// Makes assumption that this is a valid zip code and formats it to be 5 digits or 5 digits - 4 digits
    /// </summary>
    /// <param name="zipCode"></param>
    /// <returns></returns>
    public static string DisplayZipCode(string zipCode)
    {
        // TODO should we be returning a Result object?
        if (zipCode.Length == 5)
            return zipCode;
        else if (zipCode.Length == 9)
            return zipCode.Insert(5, "-");
        else if (zipCode.Length == 10 && zipCode.Contains("-"))
            return zipCode;

        return "";
    }


    /// <summary>
    /// Stores the minimized version of a zip code.  This will remove any dashes and return just the numbers.
    /// <para>Assumes that this is a valid zip code</para>
    /// </summary>
    /// <param name="zipCode"></param>
    /// <returns></returns>
    public static string ZipCodeMinimize(string zipCode)
    {
        // TODO should we be returning a Result object?
        if (zipCode.Length == 5 || zipCode.Length == 9)
            return zipCode;

        else if (zipCode.Length == 10)
        {
            string newZip = zipCode[0..5] + zipCode[6..10];
            return newZip;
        }
        return zipCode;
    }

    #endregion
}