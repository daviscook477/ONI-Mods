using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;

namespace Champagne
{
    public class ChampagneFiller : StateMachineComponent<ChampagneFiller.StatesInstance>, IEffectDescriptor
    {
        public string specificEffect;
        public string trackingEffect;
        public Tag ingredientTag;
        public float ingredientMassPerUse;
        public float ethanolMassPerUse;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi.StartSM();
            GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule, true), null, null);
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
        {
            string str = tag.ProperName();
            Descriptor descriptor = new Descriptor();
            descriptor.SetupDescriptor(
                string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, 
                str, 
                GameUtil.GetFormattedMass(mass, 
                GameUtil.TimeSlice.None, 
                GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), 
                string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, 
                str, 
                GameUtil.GetFormattedMass(mass,
                GameUtil.TimeSlice.None, 
                GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), 
                Descriptor.DescriptorType.Requirement);
            descs.Add(descriptor);
        }

        List<Descriptor> IEffectDescriptor.GetDescriptors(BuildingDef def)
        {
            List<Descriptor> descs = new List<Descriptor>();
            Descriptor descriptor = new Descriptor();
            descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, 
                UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, 
                Descriptor.DescriptorType.Effect);

            descs.Add(descriptor);
            Effect.AddModifierDescriptions(gameObject, descs, specificEffect, true);
            AddRequirementDesc(descs, ingredientTag, ingredientMassPerUse);
            AddRequirementDesc(descs, SimHashes.Ethanol.CreateTag(), ethanolMassPerUse);
            return descs;
        }

        public class States : GameStateMachine<States, StatesInstance, ChampagneFiller>
        {
            private State unoperational;
            private State operational;
            private ReadyStates ready;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = unoperational;
                unoperational
                    .PlayAnim("off")
                    .TagTransition(GameTags.Operational, operational, false);
                operational
                    .PlayAnim("off")
                    .TagTransition(GameTags.Operational, unoperational, true)
                    .Transition(ready, smi => IsReady(smi), UpdateRate.SIM_200ms)
                    .EventTransition(GameHashes.OnStorageChange, ready, smi => IsReady(smi));
                ready
                    .TagTransition(GameTags.Operational, unoperational, true)
                    .DefaultState(ready.idle)
                    .ToggleChore(CreateChore, operational);
                ready.idle
                    .Transition(operational, smi => !IsReady(smi), UpdateRate.SIM_200ms)
                    .EventTransition(GameHashes.OnStorageChange, operational, 
                    smi => !IsReady(smi))
                    .WorkableStartTransition(smi => smi.master.GetComponent<ChampagneFillerWorkable>(), ready.working);
                ready.working
                    .PlayAnim("working_pre")
                    .WorkableStopTransition(smi => smi.master.GetComponent<ChampagneFillerWorkable>(), ready.post);
                ready.post
                    .PlayAnim("working_pst")
                    .OnAnimQueueComplete(ready);
            }

            private Chore CreateChore(StatesInstance smi)
            {
                Workable component = smi.master.GetComponent<ChampagneFillerWorkable>();
                Chore chore = new WorkChore<ChampagneFillerWorkable>(Db.Get().ChoreTypes.Relax, component, 
                    null, true, null, null, null, false, 
                    Db.Get().ScheduleBlockTypes.Recreation, false, true, null, false, true, false, 
                    PriorityScreen.PriorityClass.high, 5, false, true);
                chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
                return chore;
            }

            private bool IsReady(StatesInstance smi)
            {
                PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Ethanol);
                return !(primaryElement == null) 
                    && primaryElement.Mass >= smi.master.ethanolMassPerUse 
                    && smi.GetComponent<Storage>().GetAmountAvailable(smi.master.ingredientTag) >= (double)smi.master.ingredientMassPerUse;
            }

            public class ReadyStates : State
            {
                public State idle;
                public State working;
                public State post;
            }
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, ChampagneFiller, object>.GameInstance
        {
            public StatesInstance(ChampagneFiller smi)
              : base(smi)
            {
            }
        }
    }
}
