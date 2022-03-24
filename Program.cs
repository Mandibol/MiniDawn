//Ladda in Data från CSV
//Då behöver vi en klass att spara datan i

using LiteDB;


List<Svar> listaSvar = new List<Svar>();
using (StreamReader reader = new("svar.csv"))
{
    Console.WriteLine("Läser in Svar");
    //Skippa Rubriken
    reader.ReadLine();


    string line;
    while ((line = reader.ReadLine()) != null)
    {
        string[] columns = line.Split('\u002C');
        
        //Ta bort allt utom sifror från datum
        columns[0] = columns[0].Replace(".",":");
        columns[4] = columns[4].ToLower();


        Svar svar = new(
            DateTime.Parse(columns[0]),
            int.Parse(columns[1]),
            int.Parse(columns[2]),
            int.Parse(columns[3]),
            columns[4]);
        listaSvar.Add(svar);
    }
}

//Nu behöver vi lägga in dem i Databasen

using (var db = new LiteDatabase("Filename = SvarDB.db; Connection = shared; Password = abc123"))
{
    Console.WriteLine("Öpnnar Data Bas");
    var collection = db.GetCollection<Svar>("SvarDB");

    foreach (Svar svar in listaSvar)
    {
        //Skapa Index
        collection.EnsureIndex("Datum");
        collection.EnsureIndex("Erfarenhet");

        //Kolla om svaret redan finns genom att jämföra datum
        var exists = collection.FindOne(x => x.Datum == svar.Datum);
        //Om Svaret inte finns lägg till det i Databasen
        if(exists is null)
        {
            collection.Insert(svar);
            Console.WriteLine("Svar Tillagd");
        }
        else
        {
            Console.WriteLine("Svar Ej tillagd");
        }

    }
    //Ta fram Statistik
    //Antal Svar
    var antalSvar = collection.Count();
    Console.WriteLine("Antal Svar: " + antalSvar + "\n");

    //Vi vill veta Average värde på frågorna
    Console.WriteLine("=== Average Svar ===");
    //Average 
    var avgErfarenhet = collection.Query().ToList().Average(x => x.Erfarenhet);
    var avgSvårighet = collection.Query().ToList().Average(x => x.Svårighet);
    var avgUppskattning = collection.Query().ToList().Average(x => x.Uppskattning);
    Console.WriteLine("Average Erfarenhet: " + avgErfarenhet);
    Console.WriteLine("Average Svårighetsgrad: " + avgSvårighet);
    Console.WriteLine("Average Uppskattning c# :" + avgUppskattning);
    Console.WriteLine();

    //Vi vill veta topp tre populäraste språken
    Console.WriteLine("=== Top tre populäraste språken ===");
    var språktLista = collection.Query().Select(x => x.Språk).ToList();
    var språkGrupp = GrupperaData(språktLista).OrderByDescending(x => x.Count).ToList();

    for (int i = 0; i < 3; i++)
    {
        Console.WriteLine("Språk: " + språkGrupp[i].Value + " Antal: " + språkGrupp[i].Count);
    }
    Console.WriteLine();

    //Vi vill veta fördelningen av svaren
    Console.WriteLine("=== Fördelning av svaren ===");
    //Gruppera Erfarenhet
    var erfarenhetLista = collection.Query().Select(x => x.Erfarenhet).ToList();
    var erfarenhetGrupp = GrupperaData(erfarenhetLista);
    erfarenhetGrupp.ForEach(x => Console.WriteLine("Erfarenhetsnivå: " + x.Value + " Antal: " + x.Count));
    Console.WriteLine();

    //Gruppera Svårighet
    var svårighetLista = collection.Query().Select(x => x.Svårighet).ToList();
    var svårighetGrupp = GrupperaData(svårighetLista);
    erfarenhetGrupp.ForEach(x => Console.WriteLine("Hur Svårt är C#: " + x.Value + " Antal: " + x.Count));
    Console.WriteLine();

    //Gruppera Svårighet
    var UppskattningLista = collection.Query().Select(x => x.Uppskattning).ToList();
    var UppskattningGrupp = GrupperaData(UppskattningLista);
    erfarenhetGrupp.ForEach(x => Console.WriteLine("Hur mycket Uppskattar du C#: " + x.Value + " Antal: " + x.Count));
    Console.WriteLine();
}

/// <summary>
/// Generisc Metod för att gruppera Data.
/// </summary>
List<Stat<T>> GrupperaData<T>(List<T> lista)
{
    return lista
        .GroupBy(b => b)
        .Select(x => new Stat<T>(x.Key, x.Count()))
        .OrderBy(x => x.Value).ToList();
}

