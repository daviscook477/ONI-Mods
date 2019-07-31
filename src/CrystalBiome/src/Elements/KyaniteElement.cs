using STRINGS;

namespace CrystalBiome.Elements
{
    public class KyaniteElement
    {
        public const string Data = @"elements:
  - elementId: Kyanite
    specificHeatCapacity: 1
    thermalConductivity: 2
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 1
    gasSurfaceAreaMultiplier: 1
    strength: 1
    highTemp: 1683
    highTempTransitionTarget: Magma
    defaultTemperature: 285.15
    defaultMass: 1840
    maxMass: 1840
    hardness: 25
    molarMass: 50
    lightAbsorptionFactor: 1
    materialCategory: BuildableRaw
    tags:
    - Plumbable
    - Crushable
    - BuildableAny
    - Crystal
    buildMenuSort: 5
    isDisabled: false
    state: Solid
    localizationID: STRINGS.ELEMENTS.KYANITE.NAME";

        public const string Id = "Kyanite";
        public static string Name = UI.FormatAsLink("Kyanite", Id.ToUpper());
        public static string Description = $"Purple mineral. Contains traces of aluminum.";
        public static SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
    }
}
