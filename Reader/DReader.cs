using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Text.RegularExpressions;
using Reader;
using System.Collections.Concurrent;
using System.Security.Cryptography.Xml;
using Org.BouncyCastle.Asn1.Cms;
using System.Text;
public class DReader
{
    private string                                  FilePath;
    private List<string>                            AbsoluteTokenList = new();
    private Dictionary<int, List<string>>           PageIndexDict = new();
    private ConcurrentDictionary<string,string>     InitialInfo = new();
    
    public AirportInformation AirportInfo = new();

    public DReader(string FilePathInit)
    {
        this.FilePath = FilePathInit;
        var reader = new PdfReader(File.ReadAllBytes(FilePath));
        for (var pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++)
        {
            var contentBytes = reader.GetPageContent(pageNum);
            var tokenizer = new PRTokeniser(new RandomAccessFileOrArray(contentBytes));
        
            var stringsList = new List<string>();
            while (tokenizer.NextToken())
            {
                if (tokenizer.TokenType == PRTokeniser.TokType.STRING)
                {
                    string newToken = tokenizer.StringValue;
                    foreach (string s in newToken.Split(" "))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            stringsList.Add(s);
                            AbsoluteTokenList.Add(s);
                            // Console.WriteLine(s);
                            // Thread.Sleep(1000);
                        }
                    }
                }
            }
            PageIndexDict.Add(pageNum, stringsList);
        }
        FirstPageDissection();
    }


    /// <summary>
    /// <para>
    ///     Returns the entire dispatch release, as a string, with each token joined by a space
    /// </para>
    /// </summary>
    /// <returns> A string representation of the release </returns>
    public string GetFullDispatch()
    {
        return string.Join(" ", this.AbsoluteTokenList.ToArray());
    }

    private void GetAirlineNameFromDispatch(List<string> firstPage)
    {
        List<string> AirlineName    = new();
        for (int i = 0; i < firstPage.Count; i++)
        {
            if (firstPage[i].Equals("IFR"))
            {
                InitialInfo["AirlineName"] = string.Join(" ", AirlineName);
                break;
            }
            AirlineName.Add(firstPage[i]);
        }
    }

    private void GetTaxiInformationFromDispatch(List<string> firstPage, int i)
    {
        if (firstPage[i+1].Equals("IN:"))
        {
            InitialInfo["TaxiIn"] = firstPage[i+2];
        }
        if (firstPage[i+1].Equals("OUT:"))
        {
            InitialInfo["TaxiOut"]                  = firstPage[i+2];
            InitialInfo["DepartureAirportName"]     = AirportInfo.GetAirportFullName(firstPage[i+3].Substring(0, firstPage[i+3].IndexOf('/')));
            InitialInfo["DepartureAirportAcronym"]  = firstPage[i+3].Substring(0, firstPage[i+3].IndexOf('/'));
            InitialInfo["ArrivalAirportName"]       = AirportInfo.GetAirportFullName(firstPage[i+5].Substring(0, firstPage[i+5].IndexOf('/')));
            InitialInfo["ArrivalAirportAcronym"]    = firstPage[i+5].Substring(0, firstPage[i+5].IndexOf('/'));
        }
        if (InitialInfo.ContainsKey("TaxiOut") && InitialInfo.ContainsKey("TaxiIn"))
        {
            InitialInfo["Taxi"] = "In: " + InitialInfo["TaxiIn"]  + "; Out:" + InitialInfo["TaxiOut"];
        }
    }
    private void FirstPageDissection()
    {
        List<string> FirstPage      = PageIndexDict[1];
        string HifenIntegersRegex   = @"(?<=-)\d+$";
        string DateFormatRegex      = @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.][0-9]{2}";

        new Thread(() => GetAirlineNameFromDispatch(PageIndexDict[1])).Start();
        // GetAirlineNameFromDispatch(PageIndexDict[1]);
        for (int i = 0; i < FirstPage.Count; i++)
        {
            // Console.WriteLine(firstPage[i]);
            string current = FirstPage[i];
            if (current.Equals("TAXI"))
            {
                Thread TaxiThread = new Thread(() => GetTaxiInformationFromDispatch(PageIndexDict[1], i));
                TaxiThread.Start();
                TaxiThread.Join();
            }
            else if (current.Equals("DEPART") || current.Equals("DEPART:"))
            {
                if (InitialInfo.ContainsKey("Departure"))
                    InitialInfo["Departure"] +=  " " + FirstPage[i+1];
                else
                    InitialInfo["Departure"] = FirstPage[i+1];
            }
            else if (current.Equals("ARRIVE") || current.Equals("ARRIVE:"))
            { 
                if (InitialInfo.ContainsKey("Arrival"))
                    InitialInfo["Arrival"] += " " + FirstPage[i+1];
                else
                    InitialInfo["Arrival"] = FirstPage[i+1];
            }
            else if (current.Contains("RELEASE"))
            {
                InitialInfo["Release"] = Regex.Match(current, HifenIntegersRegex).ToString();
            }
            else if (Regex.IsMatch(current, DateFormatRegex))
            {
                Match match = Regex.Match(current, DateFormatRegex);
                InitialInfo["Date"] = match.Value;
                InitialInfo["Hour"] = FirstPage[i+1];
                InitialInfo["Time"] = InitialInfo["Date"] + " " + InitialInfo["Hour"];
            }
            else if (current.Contains("TIME:"))
            {
                InitialInfo["ProposedDepartureTime"] = FirstPage[i+1];
            }
            // Console.WriteLine(current + " - " + Regex.IsMatch(current, DateFormatRegex));
            // Thread.Sleep(200);
        }
        TimeDifference();
    }

    private void TimeDifference(){
        string timeString1 = InitialInfo["Hour"];
        string timeString2 = InitialInfo["ProposedDepartureTime"];

        // Extract hour and minute from the strings
        int hours1 = int.Parse(timeString1.Substring(0, 2));
        int minutes1 = int.Parse(timeString1.Substring(2, 2));

        int hours2 = int.Parse(timeString2.Substring(0, 2));
        int minutes2 = int.Parse(timeString2.Substring(2, 2));

        // Create DateTime objects for comparison
        DateTime time1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours1, minutes1, 0);
        DateTime time2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours2, minutes2, 0);

        // Calculate the difference
        TimeSpan difference = time2 - time1;

        // Output the difference
        InitialInfo["ProposedDepartureTime"] = $"{InitialInfo["ProposedDepartureTime"]} [+{difference}]";
    }

   public string GetInfo(string request)
   {
        if (InitialInfo.ContainsKey(request))
        {
            return InitialInfo[request];
        }
        else if (request.Equals("Help") || request.Equals("All"))
        {
            StringBuilder builder = new StringBuilder( "The following information was taken from the dispatch. You can access them by calling this method (GetInfo) with the given information name:");
            List<string> keys = InitialInfo.Keys.ToList<string>();
            keys.Sort();
            foreach (string key in keys)
            {
                builder.Append("\n\t" + key);
            }
            return builder.ToString();
        }
        return "Information not available or invalid key";
   }
}