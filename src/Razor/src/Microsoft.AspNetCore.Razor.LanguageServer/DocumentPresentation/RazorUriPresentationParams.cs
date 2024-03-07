﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System.Runtime.Serialization;
using Microsoft.CodeAnalysis.Razor.Workspaces;

namespace Microsoft.AspNetCore.Razor.LanguageServer.DocumentPresentation;

/// <summary>
/// Class representing the parameters sent for a textDocument/_vs_uriPresentation request, plus
/// a host document version.
/// </summary>
internal class RazorUriPresentationParams : UriPresentationParams, IRazorPresentationParams
{
    [DataMember]
    public RazorLanguageKind Kind { get; set; }

    [DataMember]
    public int HostDocumentVersion { get; set; }
}
