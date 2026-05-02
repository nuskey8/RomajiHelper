using RomajiHelper;

var node = RomajiNode.Parse("んんん");
foreach (var pattern in node.Patterns())
{
    Console.WriteLine(pattern);
}