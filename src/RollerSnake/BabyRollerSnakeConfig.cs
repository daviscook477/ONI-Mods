using UnityEngine;
using STRINGS;

namespace RollerSnake
{
    public class BabyRollerSnakeConfig : IEntityConfig
    {
        public const string Id = "RollerSnakeBaby";
        public static string Name = UI.FormatAsLink("Roller Snakelet", RollerSnakeConfig.Id.ToUpper());
        public static string Description = $"The young of a {RollerSnakeConfig.Name}. Smaller and has trouble rolling up into an actual loop.";

        public GameObject CreatePrefab()
        {
            GameObject rollerSnake = RollerSnakeConfig.CreateRollerSnake(Id, Name, Description, "babyrollersnake_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(rollerSnake, BaseRollerSnakeConfig.SpeciesId, null);
            return rollerSnake;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
