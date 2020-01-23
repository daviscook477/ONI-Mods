using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klei.AI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using Klei.AI;
using STRINGS;

namespace PacuMorphs
{
    public class BetaPacuConfig : IEntityConfig
    {
        public static readonly EffectorValues DECOR = TUNING.BUILDINGS.DECOR.BONUS.TIER4;
        public const string ID = "PacuBeta";
        public const string BASE_TRAIT_ID = "PacuBetaBaseTrait";
        public const string EGG_ID = "PacuBetaEgg";
        public const float SALT_WATER_CONVERTED_PER_CYCLE = 120f;
        public const SimHashes INPUT_ELEMENT = SimHashes.SaltWater;
        public const SimHashes OUTPUT_ELEMENT = SimHashes.Water;
        public const int EGG_SORT_ORDER = 501;

        public static string NAME = UI.FormatAsLink("Beta Fish", ID.ToUpper());
        public static string DESCRIPTION = "Every organism in the known universe finds the Pacu extremely delicious";
        public static string EGG_NAME = UI.FormatAsLink("Beta Fry Egg", ID.ToUpper());

        public static GameObject CreatePacu(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
        {
            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BasePacuConfig.CreatePrefab(id, BASE_TRAIT_ID, name, desc, anim_file, is_baby, null, 303.15f, 353.15f), PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
            if (!is_baby)
            {
                wildCreature.AddComponent<Storage>().capacityKg = 10f;
                ElementConsumer elementConsumer = wildCreature.AddOrGet<PassiveElementConsumer>();
                elementConsumer.elementToConsume = INPUT_ELEMENT;
                elementConsumer.consumptionRate = 0.2f;
                elementConsumer.capacityKG = 10f;
                elementConsumer.consumptionRadius = 3;
                elementConsumer.showInStatusPanel = true;
                elementConsumer.sampleCellOffset = new Vector3(0.0f, 0.0f, 0.0f);
                elementConsumer.isRequired = false;
                elementConsumer.storeOnConsume = true;
                elementConsumer.showDescriptor = false;
                wildCreature.AddOrGet<UpdateElementConsumerPosition>();
                BubbleSpawner bubbleSpawner = wildCreature.AddComponent<BubbleSpawner>();
                bubbleSpawner.element = OUTPUT_ELEMENT;
                bubbleSpawner.emitMass = 2f;
                bubbleSpawner.emitVariance = 0.5f;
                bubbleSpawner.initialVelocity = new Vector2f(0, 1);
                ElementConverter elementConverter = wildCreature.AddOrGet<ElementConverter>();
                elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(INPUT_ELEMENT.CreateTag(), 0.2f)
                };
                elementConverter.outputElements = new ElementConverter.OutputElement[1]
                {
                    new ElementConverter.OutputElement(0.2f, OUTPUT_ELEMENT, 0.0f, true, true, 0.0f, 0.5f, 1f, byte.MaxValue, 0)
                };
            }
            return wildCreature;
        }

        public GameObject CreatePrefab()
        {
            return EntityTemplates.ExtendEntityToFertileCreature(
                EntityTemplates.ExtendEntityToWildCreature(
                    CreatePacu(ID, 
                    NAME,
                    DESCRIPTION, 
                    "custompacu_kanim", 
                    false), 
                    PacuTuning.PEN_SIZE_PER_CREATURE, 
                    25f),
                EGG_ID, 
                EGG_NAME, 
                DESCRIPTION, 
                "egg_custompacu_kanim",
                PacuTuning.EGG_MASS,
                BabyBetaPacuConfig.ID,
                15f,
                5f, 
                CustomPacuTuning.EGG_CHANCES_BETA,
                EGG_SORT_ORDER, 
                false,
                true, 
                false, 
                0.75f);
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
            ElementConsumer component = inst.GetComponent<ElementConsumer>();
            if (component == null)
                return;
            component.EnableConsumption(true);
        }
    }
}
