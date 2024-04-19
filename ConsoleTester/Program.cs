using vforteli.WordPieceTokenizer;

Console.WriteLine("Tokenizing text...");

var text = await File.ReadAllTextAsync("test_files/world192.txt");
var tokenizer = new WordPieceTokenizer(VocabStrings.Vocab);


var count = 100;

for (int i = 0; i < count; i++)
{
    var foo = tokenizer.Tokenize(text);
    Console.WriteLine(i);
}


Console.WriteLine("Done...");