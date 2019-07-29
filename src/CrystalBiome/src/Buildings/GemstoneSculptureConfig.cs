using UnityEngine;

using TUNING;

namespace CrystalBiome.Buildings
{
    public class GemstoneSculptureConfig : IBuildingConfig
    {
        public const string Id = "GemstoneSculpture";
        public const string DisplayName = "Gemstone Block";
        public const string Description = "Brightens any room with the colorful shine of pretty gemstones.";
        public static string Effect = $"Greatly increses {STRINGS.UI.FormatAsLink("Decor", "DECOR")}, contributing to {STRINGS.UI.FormatAsLink("Morale", "MORALE")}.\n\nMust be sculpted by a Duplicant. ";

        public const string CompletedName = "Gemstone Sculpture";
        public static string PoorQualityName = $"\"Abstract\" {CompletedName}";
        public static string ExcellentQualityName = $"Genius {CompletedName}";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 2,
                height: 2,
                anim: "gem_sculpture",
                hitpoints: BUILDINGS.HITPOINTS.TIER0,
                construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                construction_materials: new string[] { "Gemstone" },
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER1,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: new EffectorValues()
                {
                    amount = 40,
                    radius = 8
                },
                noise: NOISE_POLLUTION.NONE,
                0.2f);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.ViewMode = OverlayModes.Decor.ID;
            buildingDef.DefaultAnimState = "slab";
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<BuildingComplete>().isArtable = true;
            go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration, false);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Artable artable = go.AddComponent<Sculpture>();
            SymbolOverrideControllerUtil.AddToPrefab(go);
            artable.stages.Add(new Artable.Stage("Default", DisplayName, "slab", 0, false, Artable.Status.Ready));
            artable.stages.Add(new Artable.Stage("Bad", PoorQualityName, "crap", 5, false, Artable.Status.Ugly));
            artable.stages.Add(new Artable.Stage("Good", ExcellentQualityName, "idle", 10, true, Artable.Status.Great));
        }
    }
}
