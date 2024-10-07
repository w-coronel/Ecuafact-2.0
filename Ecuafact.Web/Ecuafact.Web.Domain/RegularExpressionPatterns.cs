namespace Ecuafact.Web
{
    public static class RegularExpressionPatterns
    {
        public const string CorreoElectronicoSimplePattern = @"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)";
        public const string CorreoElectronicoPattern = @"^" + CorreoElectronicoSimplePattern + @"$";
        public const string CorreoElectronicoListaPattern = @"^" + CorreoElectronicoSimplePattern + "(;" + CorreoElectronicoSimplePattern + @")*$";
        public const string TelefonoPattern = @"^[0-9\- ]*$";
    }
}