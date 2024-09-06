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
        if (IsAnyKindComment(trivia))
            return SyntaxTrivia(trivia, trimChars);
        
        return base.VisitTrivia(trivia);
    }

    private static SyntaxTrivia SyntaxTrivia(SyntaxTrivia trivia, string trimChars)
    {
        string xml = trivia.ToFullString();
        var newLine = "";
        var argsToSplitString = SetArgsToSplitString(xml);

        newLine = MakesFirstLetterUpperCase(argsToSplitString, newLine);

        return SyntaxFactory.Comment(newLine.TrimStart(trimChars.ToCharArray()));
    }

    private static string[] SetArgsToSplitString(string xml)
    {
        char[] delim = { ' ', '\n', '\t', '\r' };
        var ar = xml.Split(delim, StringSplitOptions.RemoveEmptyEntries);
        return ar;
    }

    private static string MakesFirstLetterUpperCase(string[] ar, string newLine)
    {
        for (int i = 0; i < ar.Length; i++)
        {
            if (i == 1) ar[i] = FirstCharToUpper(ar[i]);
            newLine = newLine + " " + ar[i];
        }
        return newLine;
    }

    private static bool IsAnyKindComment(SyntaxTrivia trivia)
    {
        return trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
               trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
               trivia.IsKind(SyntaxKind.DisabledTextTrivia);
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