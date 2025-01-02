using BepInEx;
using Utilla;

namespace MysticClient
{
    [BepInDependency("org.legoandmars.gorillatag.utilla")]
    [BepInPlugin("mystic.gamemodes", "Custom GameMods", "1.0.0")]
    [ModdedGamemode("MODDED_MCASUAL", "MCasual", Utilla.Models.BaseGamemode.Casual)]
    [ModdedGamemode("MODDED_MINFECTION", "MInfection", Utilla.Models.BaseGamemode.Infection)]
    [ModdedGamemode("MODDED_MHUNT", "MHunt", Utilla.Models.BaseGamemode.Hunt)]
    [ModdedGamemode("MODDED_MBATTLE", "MBattle", Utilla.Models.BaseGamemode.Paintbrawl)]
    public class CustomGameModes : BaseUnityPlugin { }
}