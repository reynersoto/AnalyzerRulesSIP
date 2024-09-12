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

    public SyntaxTrivia SkipTrivia(SyntaxTrivia trivia, string ruleNumber, string originalText)
    {
        return AddPreCommentToTrivia(trivia, ruleNumber, originalText);
    }
    public SyntaxTrivia AddPeriod(SyntaxTrivia trivia, string ruleNumber, string originalText)
    {
        string formatedText = AddPeriodToCommentIfItsNeeded(trivia, originalText);
        return SyntaxFactory.Comment(formatedText);
    }

    private static SyntaxTrivia SyntaxTrivia(SyntaxTrivia trivia, string trimChars)
    {
        string xml = trivia.ToFullString();
        string newLine = "", openers = "", closers = "";
        var argsToSplitString = SetArgsToSplitString(xml);

        newLine = MakesFirstLetterUpperCase(argsToSplitString, newLine);

        if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.DisabledTextTrivia))
            openers = "//";
        if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
        {
            openers = "/*";
            closers = "*/";
        }
             
        return SyntaxFactory.Comment(openers + newLine + closers);
    }
    private static SyntaxTrivia AddPreCommentToTrivia(SyntaxTrivia trivia, string ruleNumber, string originalText)
    {
            var trimChars = @" +.-";
            originalText = originalText.TrimStart(trimChars.ToCharArray());
            int insertPosition = 2;
            string formatedText = InsertStringAtPosition(originalText, ruleNumber, insertPosition);
            
            formatedText = AddPeriodToCommentIfItsNeeded(trivia, formatedText);

        return SyntaxFactory.Comment(formatedText);
    }
    

    private static string AddPeriodToCommentIfItsNeeded(SyntaxTrivia trivia, string formatedText)
    {
        if (!IsLastCharacterPeriod(formatedText) &&
                (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.DisabledTextTrivia)))
            formatedText = formatedText + ".";
        if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
            formatedText = InsertCharAtLastPosition(formatedText, ".", formatedText.Length-2);
        return formatedText;
    }

    private static string InsertStringAtPosition(string originalComment, string textToInsert, int position)
    {
        // Verifica que la posición es válida
        if (position < 0 || position > originalComment.Length)
            throw new ArgumentOutOfRangeException(nameof(position), "La posición debe estar dentro del rango del string originalComment.");

        // Divide el string originalComment en dos partes: antes y después de la posición de inserción
        string beforeInsertion = originalComment.Substring(0, position);
        string afterInsertion = originalComment.Substring(position);

        // Concatena las partes y el string a insertar
        string result = beforeInsertion + textToInsert + " " + afterInsertion;

        return result;
    }
    private static string InsertCharAtLastPosition(string originalComment, string textToInsert, int position)
    {
        if (position < 0 || position > originalComment.Length)
            throw new ArgumentOutOfRangeException(nameof(position), "La posición debe estar dentro del rango del string originalComment.");

        string beforeInsertion = originalComment.Substring(0, position);
        string afterInsertion = originalComment.Substring(position);

        if(!IsLastCharacterPeriod(beforeInsertion))
            return beforeInsertion + textToInsert + afterInsertion;

        return originalComment;
    }

    private static bool IsLastCharacterPeriod(string lastCharInput)
    {
        if (string.IsNullOrEmpty(lastCharInput))
            return false;
        char lastChar = lastCharInput[lastCharInput.Length - 1];
        return lastChar == '.';
    }
    private static string[] SetArgsToSplitString(string xml)
    {
        char[] delim = { ' ','/','*' };
        var ar = xml.Split(delim, StringSplitOptions.RemoveEmptyEntries);
        return ar;
    }

    private static string MakesFirstLetterUpperCase(string[] ar, string newLine)
    {
        for (int i = 0; i < ar.Length; i++)
        {
            if (i == 0) ar[i] = FirstCharToUpper(ar[i]);
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