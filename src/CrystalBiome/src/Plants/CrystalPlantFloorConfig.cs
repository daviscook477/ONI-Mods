using STRINGS;
using UnityEngine;

namespace CrystalBiome.Plants
{
    public class CrystalPlantFloorConfig : IEntityConfig
    {
        public const string Id = "CrystalPlantFloor";
        public const string SeedId = "CrystalPlantFloorSeed";

        public static string Name = UI.FormatAsLink("Galvanized Stalagmite", Id.ToUpper());
        public static string SeedName = UI.FormatAsLink("Crystal Starter", Id.ToUpper());

        public static string Description = $"An electrified crystal {UI.FormatAsLink("formation", "PLANTS")} that grows by absorbing all of the solid mass in mineral water.";
        public static string SeedDescription = $"The beginnings of a {Name}. Just add {UI.FormatAsLink("mineral water", Elements.MineralWaterElement.Id)}";
        public static string DomesticatedDescription = $"This {UI.FormatAsLink("outcrop", "PLANTS")} of crystals produces high energy density fragments that can be mined off when fully grown.";

        public const SingleEntityReceptacle.ReceptacleDirection Direction = SingleEntityReceptacle.ReceptacleDirection.Top;

        public GameObject CreatePrefab()
        {
            return BaseCrystalPlant.CreateCrystalPlantPrefab(Id, SeedId, Name, SeedName, Description, SeedDescription, DomesticatedDescription, "crystal_plant", Direction);
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

    }
}
