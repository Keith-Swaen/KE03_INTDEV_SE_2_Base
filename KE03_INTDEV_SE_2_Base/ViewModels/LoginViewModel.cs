namespace KE03_INTDEV_SE_2_Base.Models
{
    /*
     * ViewModel voor het loginformulier.
     * Wordt gebruikt om gebruikersnaam en wachtwoord van de gebruiker op te slaan bij het inloggen.
     */
    public class LoginViewModel
    {
        // Gebruikersnaam die wordt ingevuld op het loginformulier
        public string Username { get; set; } = string.Empty;
        // Wachtwoord dat wordt ingevuld op het loginformulier
        public string Password { get; set; } = string.Empty;
    }
}
