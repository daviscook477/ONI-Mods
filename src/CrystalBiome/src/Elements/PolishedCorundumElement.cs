using STRINGS;

namespace CrystalBiome.Elements
{
    public class PolishedCorundumElement
    {
        public const string Data = @"elements:
  - elementId: PolishedCorundum
    specificHeatCapacity: 1
    thermalConductivity: 2
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 1
    gasSurfaceAreaMultiplier: 1
    strength: 1
    highTemp: 1683
    highTempTransitionTarget: Magma
    defaultTemperature: 283.15
    defaultMass: 1840
    maxMass: 1840
    hardness: 25
    molarMass: 50
    lightAbsorptionFactor: 1
    materialCategory: Gemstone
    tags:
    - BuildableAny
    buildMenuSort: 5 
    isDisabled: false
    state: Solid
    localizationID: STRINGS.ELEMENTS.POLISHEDCORUNDUM.NAME";

        public const string Id = "PolishedCorundum";
        public static string Name = UI.FormatAsLink("Polished Corundum", Id.ToUpper());
        public static string Description = $"Corundum that has been tumbled in a {UI.FormatAsLink("Rock Polisher", Buildings.RockPolisherConfig.Id)} to give it a pretty shimmering surface.";
        public static SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
    }
}
