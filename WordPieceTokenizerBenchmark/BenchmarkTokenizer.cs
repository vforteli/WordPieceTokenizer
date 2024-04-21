using BenchmarkDotNet.Attributes;

namespace vforteli.WordPieceTokenizer;

public class BenchmarkTokenizer
{
    private string? world192Text;


    [GlobalSetup]
    public async Task GlobalSetup()
    {
        world192Text = await File.ReadAllTextAsync("world192.txt");
    }

    [Benchmark]
    public void TokenizeLong()
    {
        var tokenizer = new WordPieceTokenizer(VocabStrings.Vocab);

        _ = tokenizer.Tokenize(world192Text);
    }
}