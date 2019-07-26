using KSerialization;

namespace CrystalBiome
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class LivingCrystal : KMonoBehaviour, ISaveLoadable
    {

        [Serialize]
        private float accumulatedTemperature = 0.0f;

        [Serialize]
        private float accumulatedMass = 0.0f;

        public void Initialize()
        {
            accumulatedMass = 0.0f;
            accumulatedTemperature = 0.0f;
        }

        public void AccumulateMass(float mass, float temperature)
        {
            accumulatedTemperature = GameUtil.GetFinalTemperature(temperature, mass, accumulatedTemperature, accumulatedMass);
            accumulatedMass += mass;
        }

        public bool ValidateMass()
        {
            return !float.IsNaN(accumulatedMass) && !float.IsInfinity(accumulatedMass) && accumulatedMass > 0.0f;
        }

        public bool ValidateTemperature()
        {
            return !float.IsNaN(accumulatedTemperature) && !float.IsInfinity(accumulatedTemperature) && accumulatedTemperature > 0.0f;
        }

        public bool CanConsumeTemperature()
        {
            bool canConsume = ValidateMass() && ValidateTemperature();
            if (!canConsume)
            {
                Initialize();
            }
            return canConsume;
        }

        public float ConsumeTemperature()
        {
            float tempTemperature = accumulatedTemperature;
            Initialize();
            return tempTemperature;
        }

    }
}
