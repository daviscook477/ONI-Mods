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
            private readonly ElementConverter[] _converters;

            public SMInstance(Mineralizer master) : base(master)
            {
                _operational = master.GetComponent<Operational>();
                _converters = master.GetComponents<ElementConverter>();
            }

            public bool CanConvert()
            {
                foreach (ElementConverter converter in _converters)
                {
                    if (converter.HasEnoughMassToStartConverting()) return true;
                }
                return false;
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
                    .EventTransition(GameHashes.OnStorageChange, StartWorking, smi => smi.CanConvert());

                StartWorking
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(Working);

                Working
                    .QueueAnim("working_loop", true)
                    .EventTransition(GameHashes.OnStorageChange, StopWorking, smi => !smi.CanConvert());

                StopWorking
                    .PlayAnim("working_pst")
                    .OnAnimQueueComplete(Operational);
            }
        }
    }
}