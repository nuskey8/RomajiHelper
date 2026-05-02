using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RomajiHelper;

public sealed class RomajiNode
{
    public required char Character { get; init; }
    public required RomajiNode[] Nodes { get; init; }
    public bool IsTerminal { get; init; }

    public static RomajiNode Parse(ReadOnlySpan<char> input)
    {
        var nodes = ParseAt(input, 0);

        return new RomajiNode
        {
            Character = default,
            Nodes = nodes,
        };
    }

    public bool IsRoot => Character == default;

    ref struct Romanization
    {
        string? field0;
        string? field1;
        string? field2;

        public Romanization(scoped ReadOnlySpan<string> patterns)
        {
            Debug.Assert(patterns.Length <= 3);

            if (patterns.Length > 0) this.field0 = patterns[0];
            if (patterns.Length > 1) this.field1 = patterns[1];
            if (patterns.Length > 2) this.field2 = patterns[2];
        }

        public ReadOnlySpan<string> AsSpan()
        {
            if (field0 is null) return [];
            if (field1 is null) return MemoryMarshal.CreateReadOnlySpan(ref field0, 1);
            if (field2 is null) return MemoryMarshal.CreateReadOnlySpan(ref field0, 2);
            return MemoryMarshal.CreateReadOnlySpan(ref field0, 3);
        }

        public int Length => AsSpan().Length;
    }

    static bool TryGetRomanization(ReadOnlySpan<char> kana, out Romanization romanization)
    {
        ReadOnlySpan<string> result = kana switch
        {
            "あ" or "ア" => ["a"],
            "い" or "イ" => ["i"],
            "う" or "ウ" => ["u"],
            "え" or "エ" => ["e"],
            "お" or "オ" => ["o"],
            "か" or "カ" => ["ka", "ca"],
            "き" or "キ" => ["ki"],
            "く" or "ク" => ["ku", "cu"],
            "け" or "ケ" => ["ke"],
            "こ" or "コ" => ["ko", "co"],
            "が" or "ガ" => ["ga"],
            "ぎ" or "ギ" => ["gi"],
            "ぐ" or "グ" => ["gu"],
            "げ" or "ゲ" => ["ge"],
            "ご" or "ゴ" => ["go"],
            "さ" or "サ" => ["sa"],
            "し" or "シ" => ["si", "shi", "ci"],
            "す" or "ス" => ["su"],
            "せ" or "セ" => ["se", "ce"],
            "そ" or "ソ" => ["so"],
            "ざ" or "ザ" => ["za"],
            "じ" or "ジ" => ["zi", "ji"],
            "ず" or "ズ" => ["zu"],
            "ぜ" or "ゼ" => ["ze"],
            "ぞ" or "ゾ" => ["zo"],
            "た" or "タ" => ["ta"],
            "ち" or "チ" => ["ti", "chi",],
            "つ" or "ツ" => ["tu", "tsu",],
            "て" or "テ" => ["te"],
            "と" or "ト" => ["to"],
            "だ" or "ダ" => ["da"],
            "ぢ" or "ヂ" => ["di"],
            "づ" or "ヅ" => ["du"],
            "で" or "デ" => ["de"],
            "ど" or "ド" => ["do"],
            "な" or "ナ" => ["na"],
            "に" or "ニ" => ["ni"],
            "ぬ" or "ヌ" => ["nu"],
            "ね" or "ネ" => ["ne"],
            "の" or "ノ" => ["no"],
            "は" or "ハ" => ["ha"],
            "ひ" or "ヒ" => ["hi"],
            "ふ" or "フ" => ["hu", "fu"],
            "へ" or "ヘ" => ["he"],
            "ほ" or "ホ" => ["ho"],
            "ば" or "バ" => ["ba"],
            "び" or "ビ" => ["bi"],
            "ぶ" or "ブ" => ["bu"],
            "べ" or "ベ" => ["be"],
            "ぼ" or "ボ" => ["bo"],
            "ぱ" or "パ" => ["pa"],
            "ぴ" or "ピ" => ["pi"],
            "ぷ" or "プ" => ["pu"],
            "ぺ" or "ペ" => ["pe"],
            "ぽ" or "ポ" => ["po"],
            "ま" or "マ" => ["ma"],
            "み" or "ミ" => ["mi"],
            "む" or "ム" => ["mu"],
            "め" or "メ" => ["me"],
            "も" or "モ" => ["mo"],
            "や" or "ヤ" => ["ya"],
            "ゆ" or "ユ" => ["yu"],
            "よ" or "ヨ" => ["yo"],
            "ら" or "ラ" => ["ra"],
            "り" or "リ" => ["ri"],
            "る" or "ル" => ["ru"],
            "れ" or "レ" => ["re"],
            "ろ" or "ロ" => ["ro"],
            "わ" or "ワ" => ["wa"],
            "ゐ" or "ヰ" => ["wyi"],
            "ゑ" or "ヱ" => ["wye"],
            "を" or "ヲ" => ["wo"],
            "ゔ" or "ヴ" => ["vu"],
            "ー" or "ｰ" => ["-"],
            "きゃ" or "キャ" => ["kya"],
            "きゅ" or "キュ" => ["kyu"],
            "きょ" or "キョ" => ["kyo"],
            "ぎゃ" or "ギャ" => ["gya"],
            "ぎゅ" or "ギュ" => ["gyu"],
            "ぎょ" or "ギョ" => ["gyo"],
            "しゃ" or "シャ" => ["sya", "sha"],
            "しゅ" or "シュ" => ["syu", "shu"],
            "しょ" or "ショ" => ["syo", "sho"],
            "じゃ" or "ジャ" => ["zya", "ja", "jya"],
            "じゅ" or "ジュ" => ["zyu", "ju", "jyu"],
            "じょ" or "ジョ" => ["zyo", "jo", "jyo"],
            "ちゃ" or "チャ" => ["tya", "cha", "cya"],
            "ちゅ" or "チュ" => ["tyu", "chu", "cyu"],
            "ちょ" or "チョ" => ["tyo", "cho", "cyo"],
            "ぢゃ" or "ヂャ" => ["dya"],
            "ぢゅ" or "ヂュ" => ["dyu"],
            "ぢょ" or "ヂョ" => ["dyo"],
            "にゃ" or "ニャ" => ["nya"],
            "にゅ" or "ニュ" => ["nyu"],
            "にょ" or "ニョ" => ["nyo"],
            "ひゃ" or "ヒャ" => ["hya"],
            "ひゅ" or "ヒュ" => ["hyu"],
            "ひょ" or "ヒョ" => ["hyo"],
            "びゃ" or "ビャ" => ["bya"],
            "びゅ" or "ビュ" => ["byu"],
            "びょ" or "ビョ" => ["byo"],
            "ぴゃ" or "ピャ" => ["pya"],
            "ぴゅ" or "ピュ" => ["pyu"],
            "ぴょ" or "ピョ" => ["pyo"],
            "しぇ" or "シェ" => ["she", "sye"],
            "じぇ" or "ジェ" => ["je", "zye"],
            "ちぇ" or "チェ" => ["che", "tye"],
            "つぁ" or "ツァ" => ["tsa"],
            "つぃ" or "ツィ" => ["tsi"],
            "つぇ" or "ツェ" => ["tse"],
            "つぉ" or "ツォ" => ["tso"],
            "てぃ" or "ティ" => ["thi"],
            "でぃ" or "ディ" => ["dhi"],
            "てゅ" or "テュ" => ["thu"],
            "でゅ" or "デュ" => ["dhu"],
            "とぅ" or "トゥ" => ["twu"],
            "どぅ" or "ドゥ" => ["dwu"],
            "ふぁ" or "ファ" => ["fa", "hwa"],
            "ふぃ" or "フィ" => ["fi", "hwi"],
            "ふぇ" or "フェ" => ["fe", "hwe"],
            "ふぉ" or "フォ" => ["fo", "hwo"],
            "ふゅ" or "フュ" => ["fyu", "hwyu"],
            "いぇ" or "イェ" => ["ye"],
            "うぃ" or "ウィ" => ["wi", "whi"],
            "うぇ" or "ウェ" => ["we", "whe"],
            "うぉ" or "ウォ" => ["who"],
            "ゔぁ" or "ヴァ" => ["va"],
            "ゔぃ" or "ヴィ" => ["vi"],
            "ゔぇ" or "ヴェ" => ["ve"],
            "ゔぉ" or "ヴォ" => ["vo"],
            "ゔゅ" or "ヴュ" => ["vyu"],
            "くぁ" or "クァ" => ["kwa", "qa"],
            "くぃ" or "クィ" => ["kwi", "qi"],
            "くぇ" or "クェ" => ["kwe", "qe"],
            "くぉ" or "クォ" => ["kwo", "qo"],
            "ぐぁ" or "グァ" => ["gwa"],
            "ぁ" or "ァ" => ["xa", "la"],
            "ぃ" or "ィ" => ["xi", "li"],
            "ぅ" or "ゥ" => ["xu", "lu"],
            "ぇ" or "ェ" => ["xe", "le"],
            "ぉ" or "ォ" => ["xo", "lo"],
            "ゃ" or "ャ" => ["xya", "lya"],
            "ゅ" or "ュ" => ["xyu", "lyu"],
            "ょ" or "ョ" => ["xyo", "lyo"],
            "ゎ" or "ヮ" => ["xwa", "lwa"],
            "ゕ" or "ヵ" => ["xka", "lka"],
            "ゖ" or "ヶ" => ["xke", "lke"],
            _ => [],
        };

        romanization = new Romanization(result);
        return romanization.Length > 0;
    }

    internal static RomajiNode[] ParseAt(ReadOnlySpan<char> input, int index)
    {
        if (index == input.Length)
        {
            return [];
        }

        if (input[index] is 'ん' or 'ン')
        {
            return ParseN(input, index);
        }

        if (input[index] is 'っ' or 'ッ')
        {
            return ParseSmallTsu(input, index);
        }

        var nodes = new PooledList<RomajiNode>(2);
        try
        {
            foreach (var length in new[] { 2, 1 })
            {
                if (index + length > input.Length)
                {
                    continue;
                }

                var kana = input.Slice(index, length);
                if (!TryGetRomanization(kana, out var patterns))
                {
                    continue;
                }

                var tail = ParseAt(input, index + length);
                foreach (var pattern in patterns.AsSpan())
                {
                    Add(ref nodes, tail, pattern);
                }
            }

            if (nodes.Count == 0)
            {
                ThrowInvalidCharacter(input[index], index);
            }

            return Merge(nodes.AsSpan().ToArray());
        }
        finally
        {
            nodes.Dispose();
        }
    }

    static RomajiNode[] ParseN(ReadOnlySpan<char> input, int index)
    {
        var tail = ParseAt(input, index + 1);
        var nodes = new PooledList<RomajiNode>(tail.Length + 2);
        try
        {
            Add(ref nodes, tail, "nn");
            Add(ref nodes, tail, "xn");

            var singleNTail = GetSingleNTail(tail);
            if (CanUseSingleN(input, index, singleNTail))
            {
                Add(ref nodes, singleNTail, "n");
            }

            return Merge(nodes.AsSpan().ToArray());
        }
        finally
        {
            nodes.Dispose();
        }
    }

    static RomajiNode[] GetSingleNTail(RomajiNode[] tail)
    {
        var nodes = new PooledList<RomajiNode>(tail.Length);
        try
        {
            foreach (var node in tail)
            {
                if (IsConsonant(node.Character) && node.Character is not ('y' or 'n'))
                {
                    nodes.Add(node);
                }
            }

            return nodes.AsSpan().ToArray();
        }
        finally
        {
            nodes.Dispose();
        }
    }

    static bool CanUseSingleN(ReadOnlySpan<char> input, int index, RomajiNode[] singleNTail)
    {
        if (singleNTail.Length == 0 || index + 1 >= input.Length || input[index + 1] is 'ん' or 'ン')
        {
            return false;
        }

        ReadOnlySpan<int> l = [2, 1];
        foreach (var length in l)
        {
            if (index + 1 + length > input.Length)
            {
                continue;
            }

            if (!TryGetRomanization(input.Slice(index + 1, length), out var romanization))
            {
                continue;
            }

            foreach (var pattern in romanization.AsSpan())
            {
                if (IsVowel(pattern[0]) || pattern[0] is 'y' or 'n')
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    static RomajiNode[] ParseSmallTsu(ReadOnlySpan<char> input, int index)
    {
        var tail = ParseAt(input, index + 1);
        var nodes = new PooledList<RomajiNode>(tail.Length);
        try
        {
            foreach (var node in tail)
            {
                if (IsConsonant(node.Character) && node.Character != 'n')
                {
                    nodes.Add(new RomajiNode
                    {
                        Character = node.Character,
                        Nodes = [node],
                    });
                }
            }

            ReadOnlySpan<string> patterns = ["xtsu", "xtu", "ltu"];
            foreach (var p in patterns)
            {
                Add(ref nodes, tail, p);
            }

            return Merge(nodes.AsSpan().ToArray());
        }
        finally
        {
            nodes.Dispose();
        }
    }

    static void Add(ref PooledList<RomajiNode> nodes, RomajiNode[] tail, string text)
    {
        var next = tail;

        for (var i = text.Length - 1; i > 0; i--)
        {
            next =
            [
                new RomajiNode
                {
                    Character = text[i],
                    Nodes = next,
                    IsTerminal = next.Length == 0,
                },
            ];
        }

        nodes.Add(new RomajiNode
        {
            Character = text[0],
            Nodes = next,
            IsTerminal = next.Length == 0,
        });
    }

    static RomajiNode[] Merge(RomajiNode[] nodes)
    {
        if (nodes.Length <= 1)
        {
            return nodes.Select(MergeChildren).ToArray();
        }

        var result = new PooledList<RomajiNode>(nodes.Length);
        try
        {
            foreach (var node in nodes)
            {
                var mergedNode = MergeChildren(node);
                var index = result.FindIndex(n => n.Character == mergedNode.Character);
                if (index < 0)
                {
                    result.Add(mergedNode);
                    continue;
                }

                var current = result[index];
                result[index] = new RomajiNode
                {
                    Character = current.Character,
                    Nodes = Merge([.. current.Nodes, .. mergedNode.Nodes]),
                    IsTerminal = current.IsTerminal || mergedNode.IsTerminal,
                };
            }

            return result.AsSpan().ToArray();
        }
        finally
        {
            result.Dispose();
        }
    }

    static RomajiNode MergeChildren(RomajiNode node)
    {
        return new()
        {
            Character = node.Character,
            Nodes = Merge(node.Nodes),
            IsTerminal = node.IsTerminal,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsVowel(char c) => c is 'a' or 'i' or 'u' or 'e' or 'o';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsConsonant(char c) => c is >= 'a' and <= 'z' && !IsVowel(c);

    static void ThrowInvalidCharacter(char c, int index)
        => throw new ArgumentException($"Invalid character '{c}' at index {index}.");

    public IEnumerable<string> Patterns()
    {
        // Root node
        if (IsRoot)
        {
            foreach (var node in Nodes)
            {
                foreach (var pattern in node.Patterns())
                {
                    yield return pattern;
                }
            }

            yield break;
        }

        // Terminal node
        if (IsTerminal)
        {
            yield return Character.ToString();
        }

        // Internal node
        foreach (var node in Nodes)
        {
            foreach (var pattern in node.Patterns())
            {
                yield return Character + pattern;
            }
        }
    }

    public string? FirstPattern()
    {
        if (IsRoot)
        {
            return Nodes[0].FirstPattern();
        }

        if (IsTerminal)
        {
            return Character.ToString();
        }

        return Character + Nodes[0].FirstPattern();
    }
}
