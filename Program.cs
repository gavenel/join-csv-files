using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;

const string mergedCsv = "fusion.csv";

using var syntheseParser = new TextFieldParser(@"./synthese.csv", Encoding.UTF8);
using var catalogueParser = new TextFieldParser(@"./catalogue.csv", Encoding.UTF8);
InitCsvParser(syntheseParser);
InitCsvParser(catalogueParser);

// Write merged headers.
var headerSynthese = syntheseParser.ReadFields().ToList();
var headerCatalogue = catalogueParser.ReadFields().ToList();
headerSynthese.AddRange(headerCatalogue);
var dir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
using StreamWriter outputFile = new StreamWriter(Path.Combine(dir, mergedCsv));
outputFile.WriteLine(string.Join(';', headerSynthese));

// Copy Catalogue to List.
var catalogueLines = new List<string[]>();
while (!catalogueParser.EndOfData)
{
    var fields = catalogueParser.ReadFields();
    catalogueLines.Add(fields);
}

// Iterate synthese file.
while (!syntheseParser.EndOfData)
{
    var fields = syntheseParser.ReadFields();
    var weight = fields[6]; // 9
    var designation = fields[1]; // 9
    var fieldsToAdd = SearchInCatalogue(weight, designation, catalogueLines);

    // Write result to new file.
    var line = fields.ToList();
    line.AddRange(fieldsToAdd);
    outputFile.WriteLine(string.Join(';', line));
}


string[] SearchInCatalogue(string weight, string designation, List<string[]> catalogueLines)
{
    foreach (var fields in catalogueLines)
    {
        var cataloguePoids = fields[9];
        var catalogueDesignation = fields[7];
        // ex: 00001,3000 00001,0000 00101,3010
        // (?<![1-9]) = only 0 and not other figure before or after weight and even between 0.
        var pattern = $"^0*(?<![1-9]){weight}(,0+|0*|(?<![1-9]))\0$";
        if (catalogueDesignation.Contains(designation) && Regex.IsMatch(cataloguePoids, pattern))
        {
            return fields;
        }
    }

    return new[] { "" };
    throw new Exception($"not found {weight} {designation}");
}


void InitCsvParser(TextFieldParser textFieldParser)
{
    textFieldParser.TextFieldType = FieldType.Delimited;
    textFieldParser.SetDelimiters(";");
}


void Parse(TextFieldParser textFieldParser)
{
    while (!textFieldParser.EndOfData)
    {
        //Process row
        var fields = textFieldParser.ReadFields();
        foreach (string field in fields)
        {
            Console.WriteLine(field);
            //TODO: Process field
        }
    }
}