using System.Reflection.Metadata;
using System.Security.Cryptography;
using iTextSharp.text.pdf;

/// <summary>
/// /Users/brunogomespascotto/Downloads/KSEAKJFK_PDF_1709363976.pdf"
/// /Users/brunogomespascotto/Downloads/KLASKPHX_PDF_1710018399.pdf
/// </summary>
public class Tester
{
    static void Main(string[] args)
    {
        SpeedTester T1 = new(10, @"/Users/brunogomespascotto/Downloads/KSEAKJFK_PDF_1709363976.pdf");
        SpeedTester T2 = new(10, @"/Users/brunogomespascotto/Downloads/KLASKPHX_PDF_1710018399.pdf");

        T1.ConjoinedTest(new SpeedTester[]{T1,T2}, 1);

        T2.SingleTest();
        T1.SingleTest();
    }
}

class SpeedTester{
    int TestValue;
    string Filename;
    public SpeedTester(int TestvalueInit, string FilenameInit){
        this.TestValue = TestvalueInit;
        this.Filename = FilenameInit;
    }

    public void ConjoinedTest(SpeedTester[] TestSuites, int TestQuantity){
        for (int i = 0; i < TestQuantity; i++)
            foreach (SpeedTester T in TestSuites)
                T.Test();
    }

    public void Test(){
        List<long> TimeValues = new();
        for (int i = 0; i < TestValue; i++){
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            DReader reader = new(this.Filename);
            // Console.WriteLine(reader.GetFullDispatch());
            // Console.WriteLine("\nAirline Name: \t\t"            + reader.GetInfo("AirlineName"));
            // Console.WriteLine("Time: \t\t"                      + reader.GetInfo("Time"));
            // Console.WriteLine("Day: \t\t"                       + reader.GetInfo("Day"));
            // Console.WriteLine("Hour: \t\t"                      + reader.GetInfo("Hour"));
            // Console.WriteLine("Departure: \t\t"                 + reader.GetInfo("Departure"));
            // Console.WriteLine("Arrival: \t\t"                   + reader.GetInfo("Arrival"));
            // Console.WriteLine("Release:\t\t"                    + reader.GetInfo("Release"));
            // Console.WriteLine("Taxi In:\t\t"                    + reader.GetInfo("TaxiIn"));
            // Console.WriteLine("Taxi Out:\t\t"                   + reader.GetInfo("TaxiOut"));
            // Console.WriteLine("Taxi:\t\t"                       + reader.GetInfo("Taxi"));
            // Console.WriteLine("Departure Airport Name:\t\t"     + reader.GetInfo("DepartureAirportName"));
            // Console.WriteLine("Departure Airport Acronym:\t\t"  + reader.GetInfo("DepartureAirportAcronym"));
            // Console.WriteLine("Arrival Airport Name:\t\t"       + reader.GetInfo("ArrivalAirportName"));
            // Console.WriteLine("Arrival Airport Acronym:\t\t"    + reader.GetInfo("ArrivalAirportAcronym"));
            // Console.WriteLine("Help: \t\t"                      + reader.GetInfo("All"));

            watch.Stop();
            TimeValues.Add(watch.ElapsedMilliseconds);

            Console.WriteLine($"{i}[{(i+1)*100/TestValue}%] - Execution Time: {watch.ElapsedMilliseconds}ms");
        }
        TimeValues.Sort();
        Console.WriteLine($"Average: {TimeValues.Average()}ms\nShortest: {TimeValues[0]}ms\nLongest: {TimeValues[^1]}ms");
    }

    public void SingleTest(){
         var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            DReader reader = new(this.Filename);
            // Console.WriteLine(reader.GetFullDispatch());
            Console.WriteLine("\nAirline Name: \t\t"            + reader.GetInfo("AirlineName"));
            Console.WriteLine("Time: \t\t"                      + reader.GetInfo("Time"));
            Console.WriteLine("Day: \t\t"                       + reader.GetInfo("Day"));
            Console.WriteLine("Hour: \t\t"                      + reader.GetInfo("Hour"));
            Console.WriteLine("Departure: \t\t"                 + reader.GetInfo("Departure"));
            Console.WriteLine("Arrival: \t\t"                   + reader.GetInfo("Arrival"));
            Console.WriteLine("Release:\t\t"                    + reader.GetInfo("Release"));
            Console.WriteLine("Taxi In:\t\t"                    + reader.GetInfo("TaxiIn"));
            Console.WriteLine("Taxi Out:\t\t"                   + reader.GetInfo("TaxiOut"));
            Console.WriteLine("Taxi:\t\t"                       + reader.GetInfo("Taxi"));
            Console.WriteLine("Departure Airport Name:\t\t"     + reader.GetInfo("DepartureAirportName"));
            Console.WriteLine("Departure Airport Acronym:\t\t"  + reader.GetInfo("DepartureAirportAcronym"));
            Console.WriteLine("Arrival Airport Name:\t\t"       + reader.GetInfo("ArrivalAirportName"));
            Console.WriteLine("Arrival Airport Acronym:\t\t"    + reader.GetInfo("ArrivalAirportAcronym"));
            Console.WriteLine("Help: \t\t"                      + reader.GetInfo("All"));

            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds}ms");
    }
}