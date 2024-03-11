using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Text.RegularExpressions;
using Reader;
public class DReader
{
    private string                          FilePath;
    private List<string>                    AbsoluteTokenList = new();
    private Dictionary<int, List<string>>   PageIndexDict = new();
    private Dictionary<string,string>       InitialInfo = new();
    
    public AirportInformation DepartureAirportInfo;
    public AirportInformation ArrivalAirportInfo;

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
    ///     Returns the entire dispatch release, as a string, with each token joined by new lines (\n)
    /// </para>
    /// </summary>
    /// <returns> A string representation of the release </returns>
    public string GetFullDispatch()
    {
        return string.Join("\n", this.AbsoluteTokenList.ToArray());
    }

    private void FirstPageDissection()
    {
        List<string> firstPage      = PageIndexDict[1];
        List<string> AirlineName    = new();
        string HifenIntegersRegex   = @"(?<=-)\d+$";
        string DateFormatRegex      = @"\d{2}/\d{2}/\d{4}";

        for (int i = 0; i < firstPage.Count; i++)
        {
            if (firstPage[i].Equals("IFR"))
            {
                InitialInfo["AirlineName"] = string.Join(" ", AirlineName);
                break;
            }
            AirlineName.Add(firstPage[i]);
        }
        for (int i = 0; i < firstPage.Count; i++)
        {
            string current = firstPage[i];
            if (current.Equals("TAXI"))
            {
                if (firstPage[i+1].Equals("IN:"))
                {
                    InitialInfo["TaxiIn"] = firstPage[i+2];
                }
                if (firstPage[i+1].Equals("OUT:"))
                {
                    InitialInfo["TaxiOut"] = firstPage[i+2];
                    DepartureAirportInfo = new AirportInformation(firstPage[i+3].Substring(0, firstPage[i+3].IndexOf('/')));
                    ArrivalAirportInfo = new AirportInformation(firstPage[i+5].Substring(0, firstPage[i+5].IndexOf('/')));
                }
                if (InitialInfo.ContainsKey("TaxiIn") && InitialInfo.ContainsKey("TaxiOut"))
                {
                    InitialInfo["Taxi"] = "In: " + InitialInfo["TaxiIn"]  + "; Out:" + InitialInfo["TaxiOut"];
                }
            }
            else if (current.Equals("DEPART") || current.Equals("DEPART:"))
            {
                if (InitialInfo.ContainsKey("Departure"))
                    InitialInfo["Departure"] +=  " " + firstPage[i+1];
                else
                    InitialInfo["Departure"] = firstPage[i+1];
            }
            else if (current.Equals("ARRIVE") || current.Equals("ARRIVE:"))
            { 
                if (InitialInfo.ContainsKey("Arrival"))
                    InitialInfo["Arrival"] += " " + firstPage[i+1];
                else
                    InitialInfo["Arrival"] = firstPage[i+1];
            }
            else if (current.Contains("RELEASE"))
            {

                Match match = Regex.Match(current, HifenIntegersRegex);
                InitialInfo["Release"] = match.Value;
            }
            else if (Regex.IsMatch(current, DateFormatRegex))
            {
                Match match = Regex.Match(current, DateFormatRegex);
                InitialInfo["Date"] = match.Value;
                InitialInfo["Hour"] = firstPage[i+1];
                InitialInfo["Time"] = InitialInfo["Date"] + " " + InitialInfo["Hour"];
            }
            Console.WriteLine(current + " - " + Regex.IsMatch(current, DateFormatRegex));
            // Thread.Sleep(400);
        }
        InitialInfo["DepartureAirportName"] = DepartureAirportInfo.AirportName;
        InitialInfo["DepartureAirportAcronym"] = DepartureAirportInfo.AirportAcronym;
        InitialInfo["ArrivalAirportName"] = ArrivalAirportInfo.AirportName;
        InitialInfo["ArrivalAirportAcronym"] = ArrivalAirportInfo.AirportAcronym;
    }

   public string GetInfo(string request)
   {
        if (request.Equals("Help") || request.Equals("All"))
        {
            string builder = ""; // TODO: Make into StringBuilder
            foreach (string key in InitialInfo.Keys)
            {
                // builder.Append(key);
                builder += key + ", ";
            }
            return builder.ToString(); // TODO: Delete last comma
        }
        else if (InitialInfo.ContainsKey(request))
        {
            return InitialInfo[request];
        }
        return "Information not available or invalid key";
   }
}