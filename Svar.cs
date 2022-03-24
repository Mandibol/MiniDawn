/// <summary>
/// Denna Klassen används till att ta emot och spara svarsinformation i
/// för att enkelt kunna lägga till i en databas
/// </summary>
public class Svar
{
    public DateTime Datum { get; set; }
    public int Erfarenhet { get; set; }
    public int Svårighet { get; set; }
    public int Uppskattning { get; set; }
    public string Språk { get; set; }
    
    public Svar( DateTime Datum, int Erfarenhet, int Svårighet, int Uppskattning, string Språk)
    {
        this.Datum = Datum;
        this.Erfarenhet = Erfarenhet;
        this.Svårighet = Svårighet; 
        this.Uppskattning = Uppskattning;   
        this.Språk = Språk;
    }
}

