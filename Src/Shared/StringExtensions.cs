namespace Sheakley.ICS.Template.Blazor.Shared;

public static class StringExtension
    {
        public static string PrintArg(this string str,
                                      string value)
        {
            return "[ " + value + " ] ";
        }


        public static string PrintArg(this string str,
                                      int? value)
        {
            if (value == null)
                return "[ null ] ";

            return "[ " + value.ToString() + " ] ";
        }


        public static string PrintArg(this string str,
                                      int value)
        {
            return "[ " + value.ToString() + " ] ";
        }


        public static string PrintArg(this string str,
                                      short value)
        {
            return "[ " + value.ToString() + " ] ";
        }


        public static string PrintArg(this string str,
                                      byte value)
        {
            return "[ " + value.ToString() + " ] ";
        }


        public static string PrintArg(this string str,
                                      decimal value)
        {
            return "[ " + value.ToString("##.000") + " ] ";
        }

    }
