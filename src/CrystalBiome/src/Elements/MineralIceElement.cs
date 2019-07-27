using STRINGS;

namespace CrystalBiome.Elements
{
    public class MineralIceElement
    {
        public const string Data = @"elements:
  - elementId: MineralIce
    specificHeatCapacity: 3.4
    thermalConductivity: 2.18
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 1
    gasSurfaceAreaMultiplier: 1
    highTemp: 269.5
    highTempTransitionTarget: MineralWater
    defaultTemperature: 232.15
    defaultMass: 1000
    maxMass: 1100
    molarMass: 18.01528
    lightAbsorptionFactor: 0.33333
    materialCategory: Liquifiable
    tags:
    - IceOre
    - BuildableAny
    isDisabled: false
    state: Solid
    localizationID: STRINGS.ELEMENTS.MINERAL_ICE.NAME";

        public const string Id = "MineralIce";
        public static string Name = UI.FormatAsLink("Mineral Ice", Id.ToUpper());
        public static string Description = $"Mineral ice is a frozen, highy concentrated solution of {UI.FormatAsLink("Aluminum-based minerals", "ALUMINUM_SALT")} dissolved in {UI.FormatAsLink("Water", "WATER")}.\n\nIt can be used to grow crystals or in the desalination process, separating out useable salt.";
        public static SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
    }
}
