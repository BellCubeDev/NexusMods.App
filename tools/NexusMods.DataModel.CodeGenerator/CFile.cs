using System.Text;

namespace NexusMods.DataModel.CodeGenerator;

public class CFile
{
    private readonly StringBuilder _sb;
    private int _indent;

    public CFile()
    {
        _sb = new StringBuilder();
        Code("// THIS FILE IS AUTOGENERATED DO NOT EDIT BY HAND");
        Code("using System;");
        Code("using System.Text.Json;");
        Code("using System.Text.Json.Serialization;");
        Code("using Microsoft.Extensions.DependencyInjection;");
        Code("using NexusMods.DataModel.Abstractions;");
        Code("using NexusMods.DataModel.Loadouts;");
        Code("using NexusMods.DataModel.Loadouts.ModFiles;");
        Code("using NexusMods.Hashing.xxHash64;");
        Code("using NexusMods.Interfaces;");
        Code("using NexusMods.Paths;");
        Code("using NexusMods.DataModel.ArchiveContents;");
        Code("using NexusMods.FileExtractor.FileSignatures;");
        Code("");
    }

    public void Write(string path)
    {
        File.WriteAllText(path, _sb.ToString());
    }

    public void Code(string c)
    {
        if (c.EndsWith("}"))
            _indent--;

        for (var i = 0; i < _indent; i++) _sb.Append("  ");
        _sb.AppendLine(c);

        if (c.EndsWith("{"))
            _indent++;
    }
}