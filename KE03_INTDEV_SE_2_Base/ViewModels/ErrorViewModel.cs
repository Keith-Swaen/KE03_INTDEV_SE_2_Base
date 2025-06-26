namespace KE03_INTDEV_SE_2_Base.Models
{
    /*
     * ViewModel voor het tonen van foutmeldingen.
     * Wordt gebruikt om het requestId en de zichtbaarheid daarvan te tonen op de errorpagina.
     */
    public class ErrorViewModel
    {
        // Id van het request waarbij de fout optrad
        public string? RequestId { get; set; }
        // Geeft aan of het requestId getoond moet worden
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
