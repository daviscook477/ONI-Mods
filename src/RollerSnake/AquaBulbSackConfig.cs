using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace AquaBulb
{
    public class AquaBulbSackConfig : IEntityConfig
    {
        public const string Id = "AquaBulbSack";

        public static string Name = UI.FormatAsLink("Aqua Bulb Sack", Id.ToUpper());

        public static string Description = $"The bulbous, water-filled membrane of a {AquaBulbConfig.Name}. The water within can be used by Duplicants once the membrane is ruptured.";

        public GameObject CreatePrefab()
        {
            var looseEntity = EntityTemplates.CreateLooseEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1000000f,
                unitMass: false,
                anim: Assets.GetAnim("aquabulbsack_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.BuildingBack,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.4f,
                height: 0.8f,
                isPickupable: true,
                additionalTags: new List<Tag>()
                {
                    GameTags.IndustrialIngredient,
                    GameTags.Organics
                });
            looseEntity.AddOrGet<EntitySplitter>();
            looseEntity.AddOrGet<SimpleMassStatusItem>();
            EntityTemplates.CreateAndRegisterCompostableFromPrefab(looseEntity);
            return looseEntity;
        }

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst) { }
    }
}
