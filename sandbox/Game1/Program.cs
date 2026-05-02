using Kokuban;
using RomajiHelper;

string[] items = [
    "あいうえお",
    "りんご",
    "ちょう",
    "しんぶん",
    "がっこう",
    "きゃく",
    "じょうほう",
    "きょう",
    "ファイル",
    "ティー",
    "ヴァイオリン",
    "フィッシュ",
];

Console.Clear();

NEXT_WORD:
var item = items[Random.Shared.Next(items.Length)];

Console.Clear();
Console.WriteLine(item);

var node = RomajiNode.Parse(item);
var buffer = "";

NEXT_CHAR:
Console.Write(Chalk.Gray[node.FirstPattern()!]);
Console.SetCursorPosition(0, 1);
Console.Write(buffer);

RETRY:
var key = Console.ReadKey(true).KeyChar;
foreach (var nextNode in node.Nodes)
{
    if (nextNode.Character == key)
    {
        buffer += key;
        node = nextNode;
        if (nextNode.IsTerminal)
            goto NEXT_WORD;
        else
            goto NEXT_CHAR;
    }
}
goto RETRY;
