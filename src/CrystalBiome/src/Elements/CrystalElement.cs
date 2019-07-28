using STRINGS;

namespace CrystalBiome.Elements
{
    public class CrystalElement
    {
        public const string Data = @"elements:
  - elementId: ElectrifiedCrystal
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
    materialCategory: ConsumableOre
    tags:
    - BuildableAny
    buildMenuSort: 5 
    isDisabled: false
    state: Solid
    localizationID: STRINGS.ELEMENTS.ELECTRIFIEDCRYSTAL.NAME";

        public const string Id = "ElectrifiedCrystal";
        public static string Name = UI.FormatAsLink("Energized Fragment", Id.ToUpper());
        public static string Description = $"A crystal fragment that whirs with energy. Mined from a {UI.FormatAsLink("Galvanic Outcrop", Plants.CrystalPlantCeilingConfig.Id)}.";
        public static SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
    }
}
