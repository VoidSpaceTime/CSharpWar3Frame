using System;

namespace FastMDX;

internal class ParsingException : Exception
{
    public override string Message => "Parsing error.";
}