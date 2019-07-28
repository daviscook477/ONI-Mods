using STRINGS;

namespace CrystalBiome.Geysers
{
    public class MineralWaterGeyser
    {
        public const string Id = "mineral_water";
        public static string Name = UI.FormatAsLink("Mineral Water Geyser", $"GeyserGeneric_{Id.ToUpper()}");
        public static string Description = $"A highly pressurized geyser that periodically erupts with {UI.FormatAsLink("Mineral Water", Elements.MineralWaterElement.Id)}.";
    }
}
