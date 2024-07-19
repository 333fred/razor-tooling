﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Diagnostics;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.Razor.Logging;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Razor.Workspaces;

internal static class SourceTextExtensions
{
    /// <summary>
    /// Gets the minimal range of text that changed between the two versions.
    /// </summary>
    public static TextChangeRange GetEncompassingTextChangeRange(this SourceText newText, SourceText oldText)
    {
        ArgHelper.ThrowIfNull(newText);
        ArgHelper.ThrowIfNull(oldText);

        var ranges = newText.GetChangeRanges(oldText);
        if (ranges.Count == 0)
        {
            return default;
        }

        // simple case.
        if (ranges.Count == 1)
        {
            return ranges[0];
        }

        return TextChangeRange.Collapse(ranges);
    }

    public static (int line, int offset) GetLineAndOffset(this SourceText text, int position)
    {
        ArgHelper.ThrowIfNull(text);

        return text.GetLineAndOffsetCore(position);
    }

    private static (int line, int offset) GetLineAndOffsetCore(this SourceText text, int position)
    {
        var line = text.Lines.GetLineFromPosition(position);

        return (line.LineNumber, position - line.Start);
    }

    public static ((int line, int offset) start, (int line, int offset) end) GetLinesAndOffsets(this SourceText text, TextSpan textSpan)
    {
        ArgHelper.ThrowIfNull(text);

        var start = text.GetLineAndOffsetCore(textSpan.Start);
        var end = text.GetLineAndOffsetCore(textSpan.End);

        return (start, end);
    }

    public static ((int line, int offset) start, (int line, int offset) end) GetLinesAndOffsets(this SourceText text, SourceSpan sourceSpan)
    {
        ArgHelper.ThrowIfNull(text);

        var start = text.GetLineAndOffsetCore(sourceSpan.AbsoluteIndex);
        var end = text.GetLineAndOffsetCore(sourceSpan.AbsoluteIndex + sourceSpan.Length);

        return (start, end);
    }

    public static string GetSubTextString(this SourceText text, TextSpan textSpan)
    {
        ArgHelper.ThrowIfNull(text);

        using var _ = ArrayPool<char>.Shared.GetPooledArray(textSpan.Length, out var charBuffer);

        text.CopyTo(textSpan.Start, charBuffer, 0, textSpan.Length);
        return new string(charBuffer, 0, textSpan.Length);
    }

    public static bool NonWhitespaceContentEquals(this SourceText text, SourceText other)
    {
        ArgHelper.ThrowIfNull(text);
        ArgHelper.ThrowIfNull(other);

        var i = 0;
        var j = 0;
        while (i < text.Length && j < other.Length)
        {
            if (char.IsWhiteSpace(text[i]))
            {
                i++;
                continue;
            }
            else if (char.IsWhiteSpace(other[j]))
            {
                j++;
                continue;
            }
            else if (text[i] != other[j])
            {
                return false;
            }

            i++;
            j++;
        }

        while (i < text.Length && char.IsWhiteSpace(text[i]))
        {
            i++;
        }

        while (j < other.Length && char.IsWhiteSpace(other[j]))
        {
            j++;
        }

        return i == text.Length && j == other.Length;
    }

    public static bool TryGetFirstNonWhitespaceOffset(this SourceText text, out int offset)
        => text.TryGetFirstNonWhitespaceOffset(new TextSpan(0, text.Length), out offset);

    public static bool TryGetFirstNonWhitespaceOffset(this SourceText text, TextSpan span, out int offset)
    {
        ArgHelper.ThrowIfNull(text);

        for (var i = span.Start; i < span.End; i++)
        {
            if (!char.IsWhiteSpace(text[i]))
            {
                offset = i - span.Start;
                return true;
            }
        }

        offset = -1;
        return false;
    }

    public static bool TryGetFirstNonWhitespaceOffset(this SourceText text, TextSpan span, out int offset, out int newLineCount)
    {
        ArgHelper.ThrowIfNull(text);

        newLineCount = 0;

        for (var i = span.Start; i < span.End; i++)
        {
            if (!char.IsWhiteSpace(text[i]))
            {
                offset = i - span.Start;
                return true;
            }
            else if (text[i] == '\n')
            {
                newLineCount++;
            }
        }

        offset = -1;
        newLineCount = -1;
        return false;
    }

    /// <summary>
    ///  <para>
    ///   Given the source text and the current span, we start at the ending span location and iterate towards the start
    ///   until we've reached a non-whitespace character.
    ///  </para>
    ///  <para>
    ///   For instance, "  abcdef  " would have a last non-whitespace offset of 7 to correspond to the character 'f'.
    ///  </para>
    /// </summary>
    public static bool TryGetLastNonWhitespaceOffset(this SourceText text, TextSpan span, out int offset)
    {
        ArgHelper.ThrowIfNull(text);

        // If the span is at the end of the document, it's common for the "End" to represent 1 past the end of the source
        var indexableSpanEnd = Math.Min(span.End, text.Length - 1);

        for (var i = indexableSpanEnd; i >= span.Start; i--)
        {
            if (!char.IsWhiteSpace(text[i]))
            {
                offset = i - span.Start;
                return true;
            }
        }

        offset = -1;
        return false;
    }

    public static bool TryGetAbsoluteIndex(this SourceText text, int line, int character, out int absoluteIndex)
    {
        return text.TryGetAbsoluteIndex(line, character, logger: null, out absoluteIndex);
    }

    public static bool TryGetAbsoluteIndex(this SourceText text, int line, int character, ILogger? logger, out int absoluteIndex)
    {
        absoluteIndex = 0;
        var lineCount = text.Lines.Count;
        if (line > lineCount ||
            (line == lineCount && character > 0))
        {
            if (logger != null)
            {
                logger?.Log(LogLevel.Error, SR.FormatPositionLine_Outside_Range(line, nameof(text), text.Lines.Count), exception: null);
                Debug.Fail(SR.FormatPositionLine_Outside_Range(line, nameof(text), text.Lines.Count));
            }

            return false;
        }

        // LSP spec allowed a Range to end one line past the end, and character 0. SourceText does not, so we adjust to the final char position
        if (line == lineCount)
        {
            absoluteIndex = text.Length;
        }
        else
        {
            var sourceLine = text.Lines[line];
            var lineLengthIncludingLineBreak = sourceLine.SpanIncludingLineBreak.Length;
            if (character > lineLengthIncludingLineBreak)
            {
                if (logger != null)
                {
                    var errorMessage = SR.FormatPositionCharacter_Outside_Range(character, nameof(text), lineLengthIncludingLineBreak);
                    logger?.Log(LogLevel.Error, errorMessage, exception: null);
                    Debug.Fail(errorMessage);
                }

                return false;
            }

            absoluteIndex = sourceLine.Start + character;
        }

        return true;
    }

    public static int GetRequiredAbsoluteIndex(this SourceText text, int line, int character, ILogger? logger = null)
    {
        if (!text.TryGetAbsoluteIndex(line, character, logger, out var absolutePosition))
        {
            throw new ArgumentOutOfRangeException($"({line},{character}) matches or exceeds SourceText boundary {text.Lines.Count}.");
        }

        return absolutePosition;
    }

    public static TextSpan GetTextSpan(this SourceText text, int startLine, int startCharacter, int endLine, int endCharacter)
    {
        ArgHelper.ThrowIfNull(text);

        var start = GetAbsoluteIndex(startLine, startCharacter, text, "Start");
        var end = GetAbsoluteIndex(endLine, endCharacter, text, "End");

        var length = end - start;
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException($"({startLine},{startCharacter})-({endLine},{endCharacter}) resolved to zero or negative length.");
        }

        return new TextSpan(start, length);

        static int GetAbsoluteIndex(int line, int character, SourceText sourceText, string argName)
        {
            if (!sourceText.TryGetAbsoluteIndex(line, character, out var absolutePosition))
            {
                throw new ArgumentOutOfRangeException($"{argName} ({line},{character}) matches or exceeds SourceText boundary {sourceText.Lines.Count}.");
            }

            return absolutePosition;
        }
    }
}
