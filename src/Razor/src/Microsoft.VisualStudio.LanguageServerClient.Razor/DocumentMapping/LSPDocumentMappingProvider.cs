﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Razor.Workspaces;

namespace Microsoft.VisualStudio.LanguageServerClient.Razor.DocumentMapping;

internal abstract class LSPDocumentMappingProvider
{
    public abstract Task<RazorMapToDocumentRangesResponse?> MapToDocumentRangesAsync(RazorLanguageKind languageKind, Uri razorDocumentUri, Range[] projectedRanges, CancellationToken cancellationToken);
}
