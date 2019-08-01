using UnityEngine;

namespace CrystalBiome.Buildings
{
    public class Mineralizer : StateMachineComponent<Mineralizer.SMInstance>
    {
        [MyCmpGet]
        private readonly Operational _operational;

        public Mineralizer(Operational operational)
        {
            _operational = operational;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi.StartSM();
        }

        public class SMInstance : GameStateMachine<States, SMInstance, Mineralizer, object>.GameInstance
        {
            private readonly Operational _operational;

            public SMInstance(Mineralizer master) : base(master)
            {
                _operational = master.GetComponent<Operational>();
            }
                
            public bool IsOperational => _operational.IsOperational;
        }

        public class States : GameStateMachine<States, SMInstance, Mineralizer>
        {
            public State Working;
            public State StartWorking;
            public State StopWorking;
            public State Operational;
            public State NotOperational;

            public override void InitializeStates(out BaseState defaultState)
            {
                defaultState = NotOperational;

                root
                    .EventTransition(GameHashes.OperationalChanged, NotOperational, smi => !smi.IsOperational);

                NotOperational
                    .QueueAnim("off")
                    .EventTransition(GameHashes.OperationalChanged, Operational, smi => smi.IsOperational);

                Operational
                    .QueueAnim("on")
                    .EventTransition(GameHashes.OnStorageChange, StartWorking, smi => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());

                StartWorking
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(Working);

                Working
                    .Enter(smi => smi.master._operational.SetActive(true, false))
                    .Enter(smi => smi.master.GetComponent<LoopingSounds>().StartSound("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_lP"))
                    .QueueAnim("working_loop", true)
                    .EventTransition(GameHashes.OnStorageChange, StopWorking, smi => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll())
                    .Exit(smi => smi.master._operational.SetActive(false, false))
                    .Exit(smi => smi.master.GetComponent<LoopingSounds>().StopSound("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_lP"));

                StopWorking
                    .PlayAnim("working_pst")
                    .OnAnimQueueComplete(Operational);
            }
        }
    }
}