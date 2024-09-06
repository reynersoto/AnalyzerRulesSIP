using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;

public class MyRewriter : CSharpSyntaxRewriter
{
    public MyRewriter() : base(true)
    {
    }
    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
        var trimChars = " .+-";
        if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
            trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
            trivia.IsKind(SyntaxKind.DisabledTextTrivia))
        {
            string xml = trivia.ToFullString();
            var newLine = "";
            char[] delim = { ' ', '\n', '\t', '\r' };
            var ar = xml.Split(delim, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < ar.Length; i++)
            {
                if (i == 1) ar[i] = FirstCharToUpper(ar[i]);
                newLine = newLine + " " + ar[i];
            }

            return SyntaxFactory.Comment(newLine.TrimStart(trimChars.ToCharArray()));
        }
        return base.VisitTrivia(trivia);
    }
    private static string FirstCharToUpper(string input)
    {
        var firstLetter = input[0].ToString().ToUpper();
        var nextLetters = input.AsSpan(1).ToString();
        switch (input)
        {
            case null:
                throw new ArgumentNullException(nameof(input));
            case "":
                throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default:
                return string.Concat(firstLetter, nextLetters);
        }
    }
}