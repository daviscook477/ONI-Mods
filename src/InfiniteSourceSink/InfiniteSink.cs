using UnityEngine;

namespace InfiniteSourceSink
{
    public class InfiniteSink : KMonoBehaviour
    {
        [SerializeField]
        public ConduitType Type;

        private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
        private int inputCell;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            accumulator = Game.Instance.accumulators.Add("Sink", (KMonoBehaviour)this);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            var building = GetComponent<Building>();
            inputCell = building.GetUtilityInputCell();

            Conduit.GetFlowManager(Type).AddConduitUpdater(ConduitUpdate);
        }

        protected override void OnCleanUp()
        {
            Conduit.GetFlowManager(Type).RemoveConduitUpdater(ConduitUpdate);
            Game.Instance.accumulators.Remove(accumulator);
            base.OnCleanUp();
        }

        private Operational.Flag incomingFlag = new Operational.Flag("incoming", Operational.Flag.Type.Requirement);

        private void ConduitUpdate(float dt)
        {
            var flowManager = Conduit.GetFlowManager(Type);
            if (flowManager == null || !flowManager.HasConduit(inputCell))
            {
                GetComponent<Operational>().SetFlag(incomingFlag, false);
                return;
            }

            var contents = flowManager.GetContents(inputCell);
            GetComponent<Operational>().SetFlag(incomingFlag, contents.mass > 0.0f);
            if (GetComponent<Operational>().IsOperational)
            {
                flowManager.RemoveElement(inputCell, contents.mass);
                Game.Instance.accumulators.Accumulate(accumulator, contents.mass);
            }
        }

    }
}
