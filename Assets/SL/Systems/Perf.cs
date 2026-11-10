using System;
using UnityEngine;

namespace SL.Systems
{
    public enum QualityTier { Low = 0, Mid = 1, High = 2 }

    /// <summary>Per-phone-tier limits (planning figures from the design; tune on real devices).</summary>
    [Serializable]
    public struct QualityBudget
    {
        public QualityTier Tier;
        public int TargetFps;
        public int CharacterTrianglesTotal;
        public int FullDetailPlayers;
        public int OwnCharacterTriangles;
        public int TerrainTriangles;
        public int TreesInView;
        public float DrawDistance;
        public bool RealtimeShadows;
        public bool BlobShadows;
        public bool HalfTextures;
        public float EffectScale;
        public float IkRadius;
        public int MaxLiveVfx;
        public int NpcsActive;
        public float NearRing, MidRing, FarRing;

        public static QualityBudget For(QualityTier t)
        {
            QualityBudget b = new QualityBudget();
            b.Tier = t;
            switch (t)
            {
                case QualityTier.Low:
                    b.TargetFps = 30; b.CharacterTrianglesTotal = 100000; b.FullDetailPlayers = 6; b.OwnCharacterTriangles = 8000;
                    b.TerrainTriangles = 20000; b.TreesInView = 40; b.DrawDistance = 120f; b.RealtimeShadows = false; b.BlobShadows = true;
                    b.HalfTextures = true; b.EffectScale = 0.4f; b.IkRadius = 12f; b.MaxLiveVfx = 24; b.NpcsActive = 12;
                    b.NearRing = 18f; b.MidRing = 45f; b.FarRing = 90f;
                    break;
                case QualityTier.Mid:
                    b.TargetFps = 45; b.CharacterTrianglesTotal = 200000; b.FullDetailPlayers = 12; b.OwnCharacterTriangles = 10000;
                    b.TerrainTriangles = 40000; b.TreesInView = 100; b.DrawDistance = 180f; b.RealtimeShadows = false; b.BlobShadows = true;
                    b.HalfTextures = false; b.EffectScale = 0.7f; b.IkRadius = 25f; b.MaxLiveVfx = 48; b.NpcsActive = 24;
                    b.NearRing = 25f; b.MidRing = 60f; b.FarRing = 120f;
                    break;
                default:
                    b.TargetFps = 60; b.CharacterTrianglesTotal = 400000; b.FullDetailPlayers = 24; b.OwnCharacterTriangles = 12000;
                    b.TerrainTriangles = 80000; b.TreesInView = 250; b.DrawDistance = 260f; b.RealtimeShadows = true; b.BlobShadows = false;
                    b.HalfTextures = false; b.EffectScale = 1f; b.IkRadius = 60f; b.MaxLiveVfx = 96; b.NpcsActive = 40;
                    b.NearRing = 35f; b.MidRing = 80f; b.FarRing = 160f;
                    break;
            }
            return b;
        }
    }

    /// <summary>Detects the phone tier at launch, applies it, and lets Settings override it.</summary>
    public static class TierManager
    {
        const string PrefKey = "SL_TierOverride";
        static bool inited;
        public static QualityTier DeviceTier { get; private set; }
        public static QualityTier Current { get; private set; }
        public static QualityBudget Budget { get; private set; }
        public static bool Auto { get; private set; }
        public static event Action<QualityBudget> Changed;

        public static QualityTier Detect()
        {
            int ram = SystemInfo.systemMemorySize;
            int cores = SystemInfo.processorCount;
            bool desktop = SystemInfo.deviceType == DeviceType.Desktop;
            QualityTier t;
            if (desktop) t = QualityTier.High;
            else if (ram <= 2800) t = QualityTier.Low;
            else if (ram <= 4800) t = QualityTier.Mid;
            else t = QualityTier.High;
            if (!desktop && cores <= 4 && t > QualityTier.Low) t = (QualityTier)((int)t - 1);
            return t;
        }

        public static void Init()
        {
            if (inited) return;
            inited = true;
            DeviceTier = Detect();
            int ov = PlayerPrefs.GetInt(PrefKey, -1);
            Auto = !(ov >= 0 && ov <= 2);
            Apply(Auto ? DeviceTier : (QualityTier)ov);
        }

        /// <summary>-1 = automatic, 0 = Low, 1 = Mid, 2 = High.</summary>
        public static void SetOverride(int tier)
        {
            PlayerPrefs.SetInt(PrefKey, tier);
            PlayerPrefs.Save();
            Auto = !(tier >= 0 && tier <= 2);
            Apply(Auto ? DeviceTier : (QualityTier)tier);
        }

        public static void Apply(QualityTier t)
        {
            Current = t;
            Budget = QualityBudget.For(t);
            Application.targetFrameRate = Budget.TargetFps;
            QualitySettings.vSyncCount = 0;
            QualitySettings.shadows = Budget.RealtimeShadows ? ShadowQuality.All : ShadowQuality.Disable;
            QualitySettings.lodBias = t == QualityTier.Low ? 0.6f : (t == QualityTier.Mid ? 1f : 1.5f);
            QualitySettings.pixelLightCount = t == QualityTier.Low ? 1 : (t == QualityTier.Mid ? 2 : 4);
#if UNITY_2022_2_OR_NEWER
            QualitySettings.globalTextureMipmapLimit = Budget.HalfTextures ? 1 : 0;
#endif
            if (Changed != null) Changed(Budget);
        }
    }

    /// <summary>Watches the frame rate and moves the tier down (or back up) with long delays so it does not flicker.</summary>
    public sealed class TierAutoTuner : MonoBehaviour
    {
        float ema = 0.016f, lowTimer, highTimer, cooldown;

        void Update()
        {
            if (!TierManager.Auto) return;
            float dt = Time.unscaledDeltaTime;
            if (dt <= 0f || dt > 0.5f) return;
            ema = Mathf.Lerp(ema, dt, 0.05f);
            if (cooldown > 0f) { cooldown -= dt; return; }
            float fps = 1f / ema;
            int target = TierManager.Budget.TargetFps;
            if (fps < target * 0.8f) { lowTimer += dt; highTimer = 0f; }
            else if (fps > target * 1.15f) { highTimer += dt; lowTimer = 0f; }
            else { lowTimer = 0f; highTimer = 0f; }

            if (lowTimer > 5f && TierManager.Current > QualityTier.Low)
            {
                TierManager.Apply((QualityTier)((int)TierManager.Current - 1));
                lowTimer = 0f; cooldown = 30f;
            }
            else if (highTimer > 20f && TierManager.Current < TierManager.DeviceTier)
            {
                TierManager.Apply((QualityTier)((int)TierManager.Current + 1));
                highTimer = 0f; cooldown = 30f;
            }
        }
    }
}
