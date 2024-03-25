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

        // T1.ConjoinedTest(new SpeedTester[]{T1,T2}, 1000);

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

            watch.Stop();
            TimeValues.Add(watch.ElapsedMilliseconds);

            Console.WriteLine($"{i}[{(i+1)*100/TestValue}%] - Execution Time: {watch.ElapsedMilliseconds}ms");
        }
        TimeValues.Sort();
        Console.WriteLine($"Average: {TimeValues.Average()}ms\nShortest: {TimeValues[0]}ms\nLongest: {TimeValues[^1]}ms");
    }

    public void SingleTest(){
        // Next section gets the biggest key and it's length, this is just made so that the printed column of information is alligned
        DReader ReaderParser = new(this.Filename);
        int maxKeyLength = 0;
        foreach (var key in ReaderParser.GetInfo("Tokens").Split(", "))
        {
            if (key.Length > maxKeyLength)
                maxKeyLength = key.Length;
        }
        maxKeyLength += 10;


        var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

        DReader reader = new(this.Filename);
        foreach (string s in reader.GetInfo("Tokens").Split(", "))
        {
            int spacesToAdd = maxKeyLength - s.Length + 1; // Calculate the number of spaces to add
            string spaces = new string(' ', spacesToAdd); // Create a string of spaces
            Console.WriteLine($"{s}:{spaces}{reader.GetInfo(s)}");
        } 

        watch.Stop();
        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds}ms\n");
    }
}