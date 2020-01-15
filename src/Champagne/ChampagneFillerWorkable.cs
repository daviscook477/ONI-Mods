using Klei;
using Klei.AI;
using TUNING;
using System;

namespace Champagne
{
    public class ChampagneFillerWorkable : Workable, IWorkerPrioritizable
    {
        [MyCmpReq]
        private Operational operational;
        public int basePriority;
        private ChampagneFiller champagneFiller;

        private ChampagneFillerWorkable()
        {
            SetReportType(ReportManager.ReportType.PersonalTime);
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            overrideAnims = new KAnimFile[1]
            {
                Assets.GetAnim("anim_interacts_champagnefiller_kanim")
            };
            showProgressBar = true;
            resetProgressOnStop = true;
            synchronizeAnims = false;
            SetWorkTime(30f);
            champagneFiller = GetComponent<ChampagneFiller>();
        }

        protected override void OnStartWork(Worker worker)
        {
            operational.SetActive(true, false);
        }

        protected override void OnCompleteWork(Worker worker)
        {
            Storage component1 = GetComponent<Storage>();
            SimUtil.DiseaseInfo disease_info1;
            float aggregate_temperature;
            component1.ConsumeAndGetDisease(SimHashes.Ethanol.CreateTag(), champagneFiller.ethanolMassPerUse, out disease_info1, out aggregate_temperature);
            SimUtil.DiseaseInfo disease_info2;
            component1.ConsumeAndGetDisease(champagneFiller.ingredientTag, champagneFiller.ingredientMassPerUse, out disease_info2, out aggregate_temperature);
            GermExposureMonitor.Instance smi = worker.GetSMI<GermExposureMonitor.Instance>();
            if (smi != null)
            {
                smi.TryInjectDisease(disease_info1.idx, disease_info1.count, SimHashes.Ethanol.CreateTag(), Sickness.InfectionVector.Digestion);
                smi.TryInjectDisease(disease_info2.idx, disease_info2.count, champagneFiller.ingredientTag, Sickness.InfectionVector.Digestion);
            }
            Effects component2 = worker.GetComponent<Effects>();
            if (!string.IsNullOrEmpty(champagneFiller.specificEffect))
                component2.Add(champagneFiller.specificEffect, true);
            if (string.IsNullOrEmpty(champagneFiller.trackingEffect))
                return;
            component2.Add(champagneFiller.trackingEffect, true);
        }

        protected override void OnStopWork(Worker worker)
        {
            operational.SetActive(false, false);
        }

        public bool GetWorkerPriority(Worker worker, out int priority)
        {
            priority = basePriority;
            Effects component = worker.GetComponent<Effects>();
            if (!string.IsNullOrEmpty(champagneFiller.trackingEffect) && component.HasEffect(champagneFiller.trackingEffect))
            {
                priority = 0;
                return false;
            }
            if (!string.IsNullOrEmpty(champagneFiller.specificEffect) && component.HasEffect(champagneFiller.specificEffect))
                priority = RELAXATION.PRIORITY.RECENTLY_USED;
            return true;
        }
    }

}