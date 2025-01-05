using System.Runtime.CompilerServices;

namespace GameInfoFetcher.Models;

public static
class MoreGameInfo
{
    public static bool IsDefault(this GameInfo gi) => gi.Equals(default(GameInfo));
    public static bool IsNotDefault(this GameInfo gi) => !gi.IsDefault();
}
