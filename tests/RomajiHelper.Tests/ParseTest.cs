namespace RomajiHelper.Tests;

public class ParseTest
{
    [Theory]
    [ClassData(typeof(ParseTestData))]
    public void Test_Parse(string input, string[] patterns)
    {
        var node = RomajiNode.Parse(input);
        Assert.Equal(patterns.ToHashSet(), node.Patterns().ToHashSet());
        AssertNoDuplicateSiblingCharacters(node);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("あ🙂")]
    public void Test_Parse_InvalidCharacterThrows(string input)
    {
        Assert.Throws<ArgumentException>(() => RomajiNode.Parse(input));
    }

    static void AssertNoDuplicateSiblingCharacters(RomajiNode node)
    {
        Assert.Equal(node.Nodes.Length, node.Nodes.Select(x => x.Character).Distinct().Count());

        foreach (var child in node.Nodes)
        {
            AssertNoDuplicateSiblingCharacters(child);
        }
    }
}

public class ParseTestData : TheoryData<string, string[]>
{
    public ParseTestData()
    {
        Add("あいうえお", ["aiueo"]);
        Add("りんご", ["ringo", "rinngo", "rixngo"]);
        Add("り ん\tご", ["ringo", "rinngo", "rixngo"]);
        Add("ちょう", ["chou", "tyou", "cyou", "chixyou", "chilyou", "tixyou", "tilyou"]);
        Add("がっこう", ["gakkou", "gaccou", "gaxtsukou", "gaxtsucou", "gaxtukou", "gaxtucou", "galtukou", "galtucou"]);
        Add("きゃく", ["kyaku", "kyacu", "kixyaku", "kixyacu", "kilyaku", "kilyacu"]);
        Add("き ゃ\nく", ["kyaku", "kyacu", "kixyaku", "kixyacu", "kilyaku", "kilyacu"]);
        Add("じょうほう", ["zyouhou", "jouhou", "jyouhou", "zixyouhou", "zilyouhou", "jixyouhou", "jilyouhou"]);
        Add("きょう", ["kyou", "kixyou", "kilyou"]);
        Add("しょくどう", ["syokudou", "syocudou", "shokudou", "shocudou", "shixyokudou", "shixyocudou", "shilyokudou", "shilyocudou", "sixyokudou", "sixyocudou", "silyokudou", "silyocudou", "cixyokudou", "cixyocudou", "cilyokudou", "cilyocudou"]);
        Add("しょ", ["syo", "sho", "shixyo", "shilyo", "sixyo", "silyo", "cixyo", "cilyo"]);
        Add("じゃ", ["ja", "jya", "zya", "jixya", "jilya", "zixya", "zilya"]);
        Add("んあ", ["nna", "xna"]);
        Add("んな", ["nnna", "xnna"]);
        Add("んや", ["nnya", "xnya"]);
        Add("わんこ", ["wannko", "wannco", "wanko", "wanco", "waxnko", "waxnco"]);
        Add("わん こ", ["wannko", "wannco", "wanko", "wanco", "waxnko", "waxnco"]);
        Add("ぱぴぷぺぽ", ["papipupepo"]);
        Add("ー", ["-"]);
        Add("ヶ", ["xke", "lke"]);
        Add("ん", ["nn", "xn"]);
        Add(" \t\n", []);
    }
}
