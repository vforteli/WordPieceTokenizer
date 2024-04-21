using vforteli.WordPieceTokenizer;

Console.WriteLine("Tokenizing text...");

var text = await File.ReadAllTextAsync("world192.txt");
var tokenizer = new WordPieceTokenizer(VocabStrings.Vocab);


var count = 10;

for (int i = 0; i < count; i++)
{
    _ = tokenizer.Tokenize(text).ToList();
    Console.WriteLine(i);
}


Console.WriteLine(GC.GetTotalAllocatedBytes() / 1024 / 1024);


Console.WriteLine("Done...");