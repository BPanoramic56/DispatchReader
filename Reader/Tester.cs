// using iText.Kernel.Pdf;
using iTextSharp.text.pdf;

// using iText.Layout.Element;
public class Tester
{
    static void Main(string[] args)
    {
        var reader = new PdfReader(File.ReadAllBytes(@"/Users/brunogomespascotto/Downloads/KSEAKJFK_PDF_1709363976.pdf"));

        List<string> AbsoluteTokenList = new();
        Dictionary<int, List<string>> PageIndexDict = new();
        for (var pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++)
        {
            // Get the page content and tokenize it.
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
                            stringsList.Add(s);
                            AbsoluteTokenList.Add(s);
                    }
                }
            }

            PageIndexDict.Add(pageNum, stringsList);
            // Print the set of string tokens, one on each line.
            for (int i = 0; i < stringsList.Count;i++){
                Console.WriteLine($"{pageNum}: {i}/{stringsList.Count} - {stringsList[i]}");
                Thread.Sleep(10);
            }
        }

    }
}