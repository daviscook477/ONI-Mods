using STRINGS;

namespace CrystalBiome.Elements
{
    public class SodaliteElement
    {
        public const string Data = @"elements:
  - elementId: Sodalite
    specificHeatCapacity: 1
    thermalConductivity: 2
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 1
    gasSurfaceAreaMultiplier: 1
    strength: 1
    highTemp: 1683
    highTempTransitionTarget: Magma
    defaultTemperature: 276.15
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
    localizationID: STRINGS.ELEMENTS.SODALITE.NAME";

        public const string Id = "Sodalite";
        public static string Name = UI.FormatAsLink("Sodalite", Id.ToUpper());
        public static string Description = $"Blue mineral. Contains traces of aluminum.";
        public static SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
    }
}
