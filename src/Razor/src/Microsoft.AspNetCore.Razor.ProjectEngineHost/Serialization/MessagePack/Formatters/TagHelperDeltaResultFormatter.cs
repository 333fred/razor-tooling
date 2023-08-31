﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using MessagePack;
using Microsoft.AspNetCore.Razor.Language;
using Checksum = Microsoft.AspNetCore.Razor.Utilities.Checksum;

namespace Microsoft.AspNetCore.Razor.Serialization.MessagePack.Formatters;

internal sealed class TagHelperDeltaResultFormatter : TopLevelFormatter<TagHelperDeltaResult>
{
    public static readonly TopLevelFormatter<TagHelperDeltaResult> Instance = new TagHelperDeltaResultFormatter();

    private TagHelperDeltaResultFormatter()
    {
    }

    protected override TagHelperDeltaResult Deserialize(ref MessagePackReader reader, SerializerCachingOptions options)
    {
        reader.ReadArrayHeaderAndVerify(4);

        var delta = reader.ReadBoolean();
        var resultId = reader.ReadInt32();
        var added = reader.Deserialize<ImmutableArray<TagHelperDescriptor>>(options);
        var removed = reader.Deserialize<ImmutableArray<Checksum>>(options);

        return new(delta, resultId, added, removed);
    }

    protected override void Serialize(ref MessagePackWriter writer, TagHelperDeltaResult value, SerializerCachingOptions options)
    {
        writer.WriteArrayHeader(4);

        writer.Write(value.Delta);
        writer.Write(value.ResultId);
        writer.SerializeObject(value.Added, options);
        writer.SerializeObject(value.Removed, options);
    }
}
