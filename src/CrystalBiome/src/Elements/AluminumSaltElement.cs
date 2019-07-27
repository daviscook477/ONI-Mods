using STRINGS;

namespace CrystalBiome.Elements
{
    public class AluminumSaltElement
    {
        public const string Data = @"elements:
  - elementId: AluminumSalt
    specificHeatCapacity: 1
    thermalConductivity: 2
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 1
    gasSurfaceAreaMultiplier: 1
    strength: 0.1
    highTemp: 933.45
    highTempTransitionTarget: MoltenAluminum
    defaultTemperature:  290
    defaultMass: 700
    maxMass: 2000
    hardness: 5
    molarMass: 50
    lightAbsorptionFactor: 1
    materialCategory: ConsumableOre
    buildMenuSort: 5
    isDisabled: false
    state: Solid
    localizationID: STRINGS.ELEMENTS.ALUMINUMSALT.NAME";

        public const string Id = "AluminumSalt";
        public static string Name = UI.FormatAsLink("Aluminum Salt", Id.ToUpper());
        public static string Description = $"Salt made from aluminum metal. Not safe for duplicant consumption.";
        public static SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
    }
}
