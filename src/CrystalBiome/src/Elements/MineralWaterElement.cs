using STRINGS;

namespace CrystalBiome.Elements
{
    public class MineralWaterElement
    {
        public const string Data = @"elements:
  - elementId: MineralWater
    maxMass: 1000
    liquidCompression: 1.01
    speed: 100
    minHorizontalFlow: 0.01
    minVerticalFlow: 0.01
    specificHeatCapacity: 3.4
    thermalConductivity: 0.609
    solidSurfaceAreaMultiplier: 1
    liquidSurfaceAreaMultiplier: 25
    gasSurfaceAreaMultiplier: 1
    lowTemp: 269.5
    highTemp: 375.5
    lowTempTransitionTarget: MineralIce
    highTempTransitionTarget: Steam
    highTempTransitionOreId: AluminumSalt
    highTempTransitionOreMassConversion: 0.3
    defaultTemperature: 283.15
    defaultMass: 1200
    molarMass: 27.6
    toxicity: 0
    lightAbsorptionFactor: 0.25
    tags:
    - AnyWater
    isDisabled: false
    state: Liquid
    localizationID: STRINGS.ELEMENTS.MINERALWATER.NAME";

        public const string Id = "MineralWater";
        public static string Name = UI.FormatAsLink("Mineral Water", Id.ToUpper());
        public static string Description = $"Mineral water is a natural, highy concentrated solution of {UI.FormatAsLink("Aluminum-based minerals", "ALUMINUM_SALT")} dissolved in {UI.FormatAsLink("Water", "WATER")}.\n\nIt can be used to grow crystals or in the desalination process, separating out useable salt.";
        public static SimHashes SimHash = (SimHashes)Hash.SDBMLower(Id);
    }
}
