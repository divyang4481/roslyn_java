﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Formatting
{
    internal abstract partial class AbstractTriviaDataFactory
    {
        protected abstract class AbstractComplexTrivia<TToken, TTrivia> : TriviaDataWithList<TTrivia>
        {
            private readonly SyntaxToken token1;
            private readonly SyntaxToken token2;

            public TreeData TreeInfo { get; private set; }
            public string OriginalString { get; private set; }

            private readonly bool treatAsElastic;

            public AbstractComplexTrivia(OptionSet optionSet, TreeData treeInfo, SyntaxToken token1, SyntaxToken token2) :
                base(optionSet, token1.Language)
            {
                Contract.ThrowIfNull(treeInfo);

                this.token1 = token1;
                this.token2 = token2;

                this.treatAsElastic = CommonFormattingHelpers.HasAnyWhitespaceElasticTrivia(token1, token2);

                this.TreeInfo = treeInfo;
                this.OriginalString = this.TreeInfo.GetTextBetween(token1, token2);

                int lineBreaks;
                int spaces;
                ExtractLineAndSpace(this.OriginalString, out lineBreaks, out spaces);

                this.LineBreaks = lineBreaks;
                this.Spaces = spaces;
            }

            protected abstract TToken ConvertToken(SyntaxToken token);
            protected abstract SyntaxTrivia ConvertTrivia(TTrivia trivia);

            protected abstract void ExtractLineAndSpace(string text, out int lines, out int spaces);
            protected abstract TriviaData CreateComplexTrivia(int line, int space);
            protected abstract TriviaDataWithList<TTrivia> Format(FormattingContext context, ChainedFormattingRules formattingRules, int lines, int spaces, CancellationToken cancellationToken);
            protected abstract bool ContainsSkippedTokensOrText(TriviaList list);

            public TToken Token1
            {
                get
                {
                    return ConvertToken(this.token1);
                }
            }

            public TToken Token2
            {
                get
                {
                    return ConvertToken(this.token2);
                }
            }

            public override bool TreatAsElastic
            {
                get
                {
                    return this.treatAsElastic;
                }
            }

            public override bool IsWhitespaceOnlyTrivia
            {
                get
                {
                    return false;
                }
            }

            public override bool ContainsChanges
            {
                get
                {
                    return false;
                }
            }

            public override TriviaData WithSpace(int space, FormattingContext context, ChainedFormattingRules formattingRules)
            {
                // two tokens are on a single line, we don't allow changing spaces between two
                // tokens that contain noisy characters between them.
                if (!this.SecondTokenIsFirstTokenOnLine)
                {
                    return this;
                }

                // okay, two tokens are on different lines, we are basically asked to remove line breaks between them
                // and make them to be on a single line. well, that is not allowed when there are noisy chars between them
                if (this.SecondTokenIsFirstTokenOnLine)
                {
                    return this;
                }

                return Contract.FailWithReturn<TriviaData>("Can not reach here");
            }

            public override TriviaData WithLine(
                int line, int indentation, FormattingContext context, ChainedFormattingRules formattingRules, CancellationToken cancellationToken)
            {
                Contract.ThrowIfFalse(line > 0);

                // if we have elastic trivia, always let it be modified
                if (this.TreatAsElastic)
                {
                    return CreateComplexTrivia(line, indentation);
                }

                // two tokens are on a single line, it is always allowed to put those two tokens on a different lines
                if (!this.SecondTokenIsFirstTokenOnLine)
                {
                    return CreateComplexTrivia(line, indentation);
                }

                // okay, two tokens are on different lines, now we need to see whether we can add more lines or not
                if (this.SecondTokenIsFirstTokenOnLine)
                {
                    // we are asked to add more lines. sure, no problem
                    if (this.LineBreaks < line)
                    {
                        return CreateComplexTrivia(line, indentation);
                    }

                    // we already has same number of lines, but it is asking changing indentation
                    if (this.LineBreaks == line)
                    {
                        return WithIndentation(indentation, context, formattingRules, cancellationToken);
                    }

                    // sorry, we can't reduce lines if it contains noisy chars
                    if (this.LineBreaks > line)
                    {
                        return this;
                    }
                }

                return Contract.FailWithReturn<TriviaData>("Can not reach here");
            }

            public override TriviaData WithIndentation(
                int indentation, FormattingContext context, ChainedFormattingRules formattingRules, CancellationToken cancellationToken)
            {
                // if tokens are not in different line, there is nothing we can do here
                if (!this.SecondTokenIsFirstTokenOnLine)
                {
                    return this;
                }

                // well, we are already in a desired format, nothing to do. return as it is.
                if (this.Spaces == indentation)
                {
                    return this;
                }

                // do expansive check
                // we need to actually format here to find out indentation
                var list = new TriviaList(token1.TrailingTrivia, token2.LeadingTrivia);
                Contract.ThrowIfFalse(list.Count > 0);

                if (ContainsSkippedTokensOrText(list))
                {
                    // we can't format
                    return this;
                }

                // okay, we need to do expansive calculation to find out actual space between two tokens
                var trivia = Format(context, formattingRules, this.LineBreaks, indentation, cancellationToken);
                var triviaString = CreateString(trivia, cancellationToken);

                int lineBreaks;
                int spaces;
                ExtractLineAndSpace(triviaString, out lineBreaks, out spaces);

                // if given indentation is negative, and actual formatting shows that we can touch the last line (space == 0)
                // then, keep the negative indentation.
                return CreateComplexTrivia(lineBreaks, (spaces == 0 && indentation < 0) ? indentation : spaces);
            }

            private string CreateString(TriviaDataWithList<TTrivia> triviaData, CancellationToken cancellationToken)
            {
                // create string from given trivia data
                var sb = StringBuilderPool.Allocate();

                foreach (var trivia in triviaData.GetTriviaList(cancellationToken))
                {
                    var commonTrivia = ConvertTrivia(trivia);
                    sb.Append(commonTrivia.ToFullString());
                }

                return StringBuilderPool.ReturnAndFree(sb);
            }
        }
    }
}