using UnityEngine;

namespace CrystalBiome.Elements
{
    public class BaseElement
    {
        public static Substance CreateLiquidSubstance(string id, Substance source, Color32 color)
        {
            return ModUtil.CreateSubstance(
              name: id,
              state: Element.State.Liquid,
              kanim: source.anim,
              material: source.material,
              colour: color,
              ui_colour: color,
              conduit_colour: color
            );
        }

        public static Material CreateSolidMaterial(string id, Material source, Texture2D materialTexture)
        {
            var material = new Material(source);
            material.mainTexture = materialTexture;
            material.name = $"mat{id}";
            return material;
        }

        public static Color32 White = new Color32(255, 255, 255, 255);

        public static Substance CreateSolidSubstance(string id, Material source, string anim, Texture2D materialTexture)
        {
            KAnimFile kanim = Assets.GetAnim(anim);
            return ModUtil.CreateSubstance(
              name: id,
              state: Element.State.Solid,
              kanim: kanim,
              material: CreateSolidMaterial(id, source, materialTexture),
              colour: White,
              ui_colour: White,
              conduit_colour: White
            );
        }
    }
}
