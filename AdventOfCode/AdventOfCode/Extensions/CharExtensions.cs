using System.Security.Cryptography;

namespace AdventOfCode;

public static class CharExtensions
{
    public static int ToCardValue(this char c, bool joker) => c switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => joker ? 1 : 11,
        'T' => 10,
        _ => c - '0'
    };
}
