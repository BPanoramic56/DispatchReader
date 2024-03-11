using iTextSharp.text.pdf;
public class Tester
{
    static void Main(string[] args)
    {
        DReader reader = new DReader(@"/Users/brunogomespascotto/Downloads/KSEAKJFK_PDF_1709363976.pdf");
        // Console.WriteLine(reader.GetFullDispatch());
        Console.WriteLine("\nAirline Name: \t\t"        + reader.GetInfo("AirlineName"));
        Console.WriteLine("Time: \t\t"                  + reader.GetInfo("Time"));
        Console.WriteLine("Day: \t\t"                   + reader.GetInfo("Day"));
        Console.WriteLine("Hour: \t\t"                  + reader.GetInfo("Hour"));
        Console.WriteLine("Departure: \t\t"             + reader.GetInfo("Departure"));
        Console.WriteLine("Arrival: \t\t"               + reader.GetInfo("Arrival"));
        Console.WriteLine("Release:\t\t"                + reader.GetInfo("Release"));
        Console.WriteLine("Taxi In:\t\t"                + reader.GetInfo("TaxiIn"));
        Console.WriteLine("Taxi Out:\t\t"               + reader.GetInfo("TaxiOut"));
        Console.WriteLine("Taxi:\t\t"                   + reader.GetInfo("Taxi"));
        Console.WriteLine("Departure Airport Name:\t\t"           + reader.GetInfo("DepartureAirportName"));
        Console.WriteLine("Departure Airport Acronym:\t\t"           + reader.GetInfo("DepartureAirportAcronym"));
        Console.WriteLine("Arrival Airport Name:\t\t"           + reader.GetInfo("ArrivalAirportName"));
        Console.WriteLine("Arrival Airport Acronym:\t\t"           + reader.GetInfo("ArrivalAirportAcronym"));
        Console.WriteLine("Help: \t\t"                  + reader.GetInfo("Help"));
    }
}