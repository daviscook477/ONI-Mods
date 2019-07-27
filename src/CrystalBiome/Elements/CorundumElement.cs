using STRINGS;

namespace CrystalBiome.Elements
{
    public class CorundumElement
    {
        public const string Data = @"elements:
  - elementId: Corundum
    specificHeatCapacity: 1
    thermalConductivity: 2
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 1
    gasSurfaceAreaMultiplier: 1
    strength: 1
    highTemp: 1683
    highTempTransitionTarget: Magma
    defaultTemperature: 281.15
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
    buildMenuSort: 5
    isDisabled: false
    state: Solid
    localizationID: STRINGS.ELEMENTS.CORUNDUM.NAME";

        public const string Id = "Corundum";
        public static string Name = UI.FormatAsLink("Corundum", Id.ToUpper());
        public static string Description = $"Pink mineral. Contains traces of aluminum.";
        public static SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
    }
}
