using BenchmarkDotNet.Attributes;

namespace vforteli.WordPieceTokenizer;

public class BenchmarkTokenizer
{
    private string? world192Text;
    private WordPieceTokenizer? tokenizer;


    [GlobalSetup]
    public async Task GlobalSetup()
    {
        world192Text = await File.ReadAllTextAsync("world192.txt");
        tokenizer = new WordPieceTokenizer(VocabStrings.Vocab);
    }

    [Benchmark]
    public void TokenizeLong()
    {
        _ = tokenizer!.Tokenize(world192Text).ToList();
    }
}