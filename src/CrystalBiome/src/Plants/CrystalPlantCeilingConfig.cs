using STRINGS;
using UnityEngine;

namespace CrystalBiome.Plants
{
    public class CrystalPlantCeilingConfig : IEntityConfig
    {
        public const string Id = "CrystalPlantCeiling";
        public const string SeedId = "CrystalPlantCeilingSeed";

        public static string Name = UI.FormatAsLink("Galvanized Stalactite", Id.ToUpper());
        public static string SeedName = UI.FormatAsLink("Upside-down Crystal Starter", Id.ToUpper());

        public static string Description = $"An electrified crystal {UI.FormatAsLink("formation", "PLANTS")} that grows by absorbing all of the solid mass in mineral water.";
        public static string SeedDescription = $"The beginnings of a {Name}. Just add {UI.FormatAsLink("mineral water", Elements.MineralWaterElement.Id)}";
        public static string DomesticatedDescription = $"This {UI.FormatAsLink("outcrop", "PLANTS")} of crystals produces high energy density fragments that can be mined off when fully grown.";

        public const SingleEntityReceptacle.ReceptacleDirection Direction = SingleEntityReceptacle.ReceptacleDirection.Bottom;

        public GameObject CreatePrefab()
        {
            return BaseCrystalPlant.CreateCrystalPlantPrefab(Id, SeedId, Name, SeedName, Description, SeedDescription, DomesticatedDescription, "crystal_plant_down", Direction);
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

    }
}
