using UnityEngine;

namespace SL.Systems
{
    /// <summary>Starts the phone-tier, effects and combat-validation systems automatically; no scene setup needed.</summary>
    public static class SystemsBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Boot()
        {
            TierManager.Init();
            GameObject g = new GameObject("[SL Systems]");
            UnityEngine.Object.DontDestroyOnLoad(g);
            g.AddComponent<TierAutoTuner>();
            VfxPool.RegisterDefaults();
            System.Collections.Generic.List<string> errors = CombatActionLibrary.ValidateAll();
            for (int i = 0; i < errors.Count; i++) Debug.LogError("[SL Combat] " + errors[i]);
            Debug.Log("[SL Systems] Tier " + TierManager.Current + " (device " + TierManager.DeviceTier + "), target " + TierManager.Budget.TargetFps +
                      " fps, " + CombatActionLibrary.All.Count + " combat actions, " + BeginnerSpecies.All.Count + " beginner species.");
        }
    }
}
