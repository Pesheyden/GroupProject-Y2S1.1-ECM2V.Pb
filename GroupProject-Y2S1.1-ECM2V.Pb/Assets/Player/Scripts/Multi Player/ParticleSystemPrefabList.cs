using UnityEngine;

namespace MultiPlayer.Player
{
    [CreateAssetMenu(fileName = "ParticleEffectPrefabList", menuName = "Scriptable Objects/ParticleEffectPrefabList")]
    public class ParticleSystemPrefabList : ScriptableObject
    {
        [field: SerializeField] public ParticleSystem Ripping { get; private set; }
        [field: SerializeField] public ParticleSystem Smashed { get; private set; }
        [field: SerializeField] public ParticleSystem Sticking { get; private set; }
    }

}
