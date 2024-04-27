using vforteli.WordPieceTokenizer;

Console.WriteLine("Tokenizing text...");

var text = await File.ReadAllTextAsync("world192.txt");
var tokenizer = new WordPieceTokenizer(VocabStrings.Vocab);


var count = 10;


var allocations = GC.GetTotalAllocatedBytes();

for (int i = 0; i < count; i++)
{
    _ = tokenizer.Tokenize(text).Take(512).ToList();
}


Console.WriteLine(GC.GetTotalAllocatedBytes() - allocations);


Console.WriteLine("Done...");