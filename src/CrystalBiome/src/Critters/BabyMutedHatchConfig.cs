using UnityEngine;

namespace CrystalBiome.Critters
{
    public class BabyHatchMutedConfig : IEntityConfig
    {

        public GameObject CreatePrefab()
        {
            GameObject hatch = HatchMutedConfig.CreateHatch(
                HatchMutedConfig.BabyId, 
                HatchMutedConfig.BabyName, 
                HatchMutedConfig.BabyDescription, 
                "baby_hatch_new", 
                true);
            EntityTemplates.ExtendEntityToBeingABaby(hatch, HatchMutedConfig.Id, null);
            return hatch;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }

}
