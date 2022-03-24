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
    Console.WriteLine("=== Erfarenhet ===");
    var erfarenhetLista = collection.Query().Select(x => x.Erfarenhet).ToList();
    var erfarenhetGrupp = GrupperaData(erfarenhetLista);
    //Ta Fram Flest, Högst och Lägst
    var erfarenhetHögst = erfarenhetGrupp.Find(x => x.Value == erfarenhetGrupp.Max(x => x.Value));
    var erfarenhetLägst = erfarenhetGrupp.Find(x => x.Value == erfarenhetGrupp.Min(x => x.Value));
    var erfarenhetFlest = erfarenhetGrupp.Find(x => x.Count == erfarenhetGrupp.Max(x => x.Count));
    Console.WriteLine("Flest: " + erfarenhetFlest.Value + " Antal: " + erfarenhetFlest.Count);
    Console.WriteLine("Högst: " + erfarenhetHögst.Value + " Antal: " + erfarenhetHögst.Count);
    Console.WriteLine("Lägst: " + erfarenhetLägst.Value + " Antal: " + erfarenhetLägst.Count);
    Console.WriteLine();
    erfarenhetGrupp.ForEach(x => Console.WriteLine("Erfarenhetsnivå: " + x.Value + " Antal: " + x.Count));
    Console.WriteLine();

    //Gruppera Svårighet
    Console.WriteLine("=== Svårighet ===");
    var svårighetLista = collection.Query().Select(x => x.Svårighet).ToList();
    var svårighetGrupp = GrupperaData(svårighetLista);
    //Ta Fram Flest, Högst och Lägst
    var svårighetHögst = svårighetGrupp.Find(x => x.Value == svårighetGrupp.Max(x => x.Value));
    var svårighetLägst = svårighetGrupp.Find(x => x.Value == svårighetGrupp.Min(x => x.Value));
    var svårighetFlest = svårighetGrupp.Find(x => x.Count == svårighetGrupp.Max(x => x.Count));
    Console.WriteLine("Flest: " + svårighetFlest.Value + " Antal: " + svårighetFlest.Count);
    Console.WriteLine("Högst: " + svårighetHögst.Value + " Antal: " + svårighetHögst.Count);
    Console.WriteLine("Lägst: " + svårighetLägst.Value + " Antal: " + svårighetLägst.Count);
    Console.WriteLine();
    svårighetGrupp.ForEach(x => Console.WriteLine("Hur Svårt är C#: " + x.Value + " Antal: " + x.Count));
    Console.WriteLine();

    //Gruppera Uppskattning
    Console.WriteLine("=== Uppskattning ===");
    var uppskattningLista = collection.Query().Select(x => x.Uppskattning).ToList();
    var uppskattningGrupp = GrupperaData(uppskattningLista);
    
    //Ta Fram Flest, Högst och Lägst
    var uppskattningHögst = uppskattningGrupp.Find(x => x.Value == uppskattningGrupp.Max(x => x.Value));
    var uppskattningLägst = uppskattningGrupp.Find(x => x.Value == uppskattningGrupp.Min(x => x.Value));
    var uppskattningFlest = uppskattningGrupp.Find(x => x.Count == uppskattningGrupp.Max(x => x.Count));
    Console.WriteLine("Flest: " + uppskattningFlest.Value + " Antal: " + uppskattningFlest.Count);
    Console.WriteLine("Högst: " + uppskattningHögst.Value + " Antal: " + uppskattningHögst.Count);
    Console.WriteLine("Lägst: " + uppskattningLägst.Value + " Antal: " + uppskattningLägst.Count);
    Console.WriteLine();
    uppskattningGrupp.ForEach(x => Console.WriteLine("Hur mycket Uppskattar du C#: " + x.Value + " Antal: " + x.Count));
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


//Sammanfattning
/* Beroende på vart du laddar ner csv filen ifrån får den olika formatering.
 * 
 * laddar du direkt ifrån sidan får du en formatering där varje vaärde har citat-tecken runt sig
 * vilket behöver tas bort.
 * 
 * Laddar du ner via google spreadsheet så slipper du detta.
 * Dock måste datum formateras så att DateTime Parsern kan fösrtå värdet.
 * 
 * Det populäraste språket var C#
 * 
 * Top 3 är:
 * C# med 10
 * C++ med 3
 * Html med 2
 * 
 * En insikt är att folk svarade enegelska och svenska och inte höll sig inom ramarna
 * desutom svarade vissa två språk c++/c#. Så en svårighet med text input är att det kan vara svårt att tolka svar
 * vilket leder till att vissa svar kanske räknas om folk stavat tokigt fel.
 * Ett bättre alternativ hadde varit en stor lista på alla språk alternativ. Men då har man risken att missa något
 * speciellt eller obskyrt språk.
 * 
 * - Erfarenhet 1-10, ingen till expert -
 * Flest svarade  3 med 8 svar. 
 * Högst var 10 med 1 svar.
 * Lägst var 1 med 2 svar.
 * 
 * -C# svårighet 1-10, lätt till svårt -
 * Flest svarade  7 med 8 svar. 
 * Högst var 8 med 4 svar.
 * Lägst var 1 med 2 svar.
 * 
 * - C# Uppskattning 1-10, sämst till bäst-
 * De flesta svarade 3 med 8 svar. En svarade 10 och
 * Högst 10 med 7 svar
 * lägst 1 med 1 svar
 * 
 * De flesta uppskattar C# med flest svar på 10 och ett average 7,52 
 * De flesta tycker också att det är relativt svårt med flest svar 7 och ett average på 5,52
 * De flesta uppskattar sin erfarenhet till 3 med ett average på 4,36
 * 
 * Så vi har en grupp med nybörjare som tycker att C# är svårt men de uppskattar verkligen språket :)
 * 
 * 
 * Grupp Deltagare:
 * Alexander Wallberg
 * Jonathan Johansson
 * Diala Kul
 * Qabas Alzaidi
 */
