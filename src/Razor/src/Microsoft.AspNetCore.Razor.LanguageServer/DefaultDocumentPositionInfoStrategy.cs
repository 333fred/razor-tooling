﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Razor.DocumentMapping;
using Microsoft.CodeAnalysis.Razor.Logging;
using Microsoft.CodeAnalysis.Razor.ProjectSystem;
using Microsoft.CodeAnalysis.Razor.Workspaces;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace Microsoft.AspNetCore.Razor.LanguageServer;

internal class DefaultDocumentPositionInfoStrategy : IDocumentPositionInfoStrategy
{
    public static IDocumentPositionInfoStrategy Instance { get; } = new DefaultDocumentPositionInfoStrategy();

    public async Task<DocumentPositionInfo?> TryGetPositionInfoAsync(IRazorDocumentMappingService documentMappingService, DocumentContext documentContext, Position position, ILogger logger, CancellationToken cancellationToken)
    {
        var sourceText = await documentContext.GetSourceTextAsync(cancellationToken).ConfigureAwait(false);
        if (!sourceText.TryGetAbsoluteIndex(position, logger, out var absoluteIndex))
        {
            return null;
        }

        return await documentMappingService.GetPositionInfoAsync(documentContext, absoluteIndex, cancellationToken).ConfigureAwait(false);
    }
}
