using System.Collections.Generic;
using UnityEngine;
using SL.Core;

namespace SL.Systems
{
    /// <summary>One central ticker returns pooled objects when their lifetime ends (no Update on every effect).</summary>
    public sealed class PoolRunner : MonoBehaviour
    {
        static PoolRunner inst;
        struct Live { public GameObject Go; public float Expire; public string Id; }
        readonly List<Live> live = new List<Live>();

        public static PoolRunner Instance
        {
            get
            {
                if (inst == null)
                {
                    GameObject g = new GameObject("[SL PoolRunner]");
                    DontDestroyOnLoad(g);
                    inst = g.AddComponent<PoolRunner>();
                }
                return inst;
            }
        }

        public int LiveCount { get { return live.Count; } }

        public void Track(GameObject go, string id, float life)
        {
            Live l = new Live();
            l.Go = go; l.Id = id; l.Expire = Time.time + life;
            live.Add(l);
        }

        void Update()
        {
            float now = Time.time;
            for (int i = live.Count - 1; i >= 0; i--)
            {
                Live l = live[i];
                if (l.Go == null) { live.RemoveAt(i); continue; }
                if (now >= l.Expire) { PrefabPool.Despawn(l.Go, l.Id); live.RemoveAt(i); }
            }
        }
    }

    /// <summary>Generic prefab pool: objects are created once, switched off, and reused.</summary>
    public static class PrefabPool
    {
        public delegate GameObject Factory();
        static readonly Dictionary<string, Factory> factories = new Dictionary<string, Factory>();
        static readonly Dictionary<string, Stack<GameObject>> free = new Dictionary<string, Stack<GameObject>>();
        static Transform root;

        static Transform Root
        {
            get
            {
                if (root == null)
                {
                    GameObject g = new GameObject("[SL Pools]");
                    Object.DontDestroyOnLoad(g);
                    root = g.transform;
                }
                return root;
            }
        }

        public static void Register(string id, Factory factory, int prewarm)
        {
            factories[id] = factory;
            if (!free.ContainsKey(id)) free[id] = new Stack<GameObject>();
            for (int i = 0; i < prewarm; i++) free[id].Push(Make(id));
        }

        static GameObject Make(string id)
        {
            GameObject go = factories[id]();
            go.transform.SetParent(Root, false);
            go.SetActive(false);
            return go;
        }

        public static GameObject Spawn(string id, Vector3 pos, Quaternion rot, float life)
        {
            if (!factories.ContainsKey(id)) return null;
            Stack<GameObject> s = free[id];
            GameObject go = null;
            while (s.Count > 0 && go == null) go = s.Pop();
            if (go == null) go = Make(id);
            go.transform.SetPositionAndRotation(pos, rot);
            go.SetActive(true);
            if (life > 0f) PoolRunner.Instance.Track(go, id, life);
            return go;
        }

        public static void Despawn(GameObject go, string id)
        {
            if (go == null) return;
            go.SetActive(false);
            go.transform.SetParent(Root, false);
            Stack<GameObject> s;
            if (free.TryGetValue(id, out s)) s.Push(go);
            else Object.Destroy(go);
        }
    }

    /// <summary>
    /// Pooled particle effects with level of detail. Effects are built in code (no art needed), reused, scaled by the phone tier
    /// and skipped entirely when too far away or when the tier's live-effect cap is reached.
    /// </summary>
    public static class VfxPool
    {
        sealed class Recipe { public Color Color; public int Count; public float Life, Speed, Size, Radius, Gravity; }
        static readonly Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();
        static bool ready;

        static void Add(string id, uint rgb, int count, float life, float speed, float size, float radius, float gravity)
        {
            Recipe r = new Recipe();
            r.Color = new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f, 1f);
            r.Count = count; r.Life = life; r.Speed = speed; r.Size = size; r.Radius = radius; r.Gravity = gravity;
            recipes[id] = r;
            string key = id;
            PrefabPool.Register(key, delegate () { return Build(key); }, 2);
        }

        public static void RegisterDefaults()
        {
            if (ready) return;
            ready = true;
            Add("hit_spark", 0xffe9a0, 14, 0.25f, 5f, 0.12f, 0.1f, 0f);
            Add("slash_arc", 0xbfe6ff, 10, 0.20f, 7f, 0.10f, 0.25f, 0f);
            Add("dust", 0xb59a78, 10, 0.60f, 1.6f, 0.30f, 0.3f, -0.2f);
            Add("heal_pulse", 0x7dff9a, 16, 0.80f, 1.4f, 0.14f, 0.5f, -0.4f);
            Add("fire_burst", 0xff8a2a, 24, 0.50f, 4f, 0.20f, 0.2f, -0.2f);
            Add("ice_shard", 0x9fe8ff, 16, 0.50f, 4.5f, 0.14f, 0.2f, 0.4f);
            Add("thunder_arc", 0xb48cff, 20, 0.30f, 8f, 0.10f, 0.2f, 0f);
            Add("shadow_smoke", 0x5a3a7a, 14, 0.90f, 1.2f, 0.35f, 0.3f, -0.1f);
            Add("precast_glow", 0xfff2b0, 8, 0.40f, 0.8f, 0.18f, 0.35f, -0.3f);
        }

        static GameObject Build(string id)
        {
            Recipe r = recipes[id];
            GameObject go = new GameObject("VFX_" + id);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystem.MainModule m = ps.main;
            m.loop = false; m.playOnAwake = false;
            m.startLifetime = r.Life; m.startSpeed = r.Speed; m.startSize = r.Size;
            m.startColor = r.Color; m.gravityModifier = r.Gravity;
            m.maxParticles = Mathf.Max(8, r.Count * 2);
            m.simulationSpace = ParticleSystemSimulationSpace.World;
            ParticleSystem.EmissionModule e = ps.emission;
            e.rateOverTime = 0f;
            e.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)r.Count) });
            ParticleSystem.ShapeModule sh = ps.shape;
            sh.shapeType = ParticleSystemShapeType.Sphere; sh.radius = r.Radius;
            ParticleSystemRenderer pr = go.GetComponent<ParticleSystemRenderer>();
            pr.sharedMaterial = SLMat.Particle(r.Color);
            pr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            pr.receiveShadows = false;
            return go;
        }

        /// <summary>Plays an effect. Returns false when it was skipped (too far, over the live cap, unknown id).</summary>
        public static bool Play(string id, Vector3 pos, float distanceToViewer)
        {
            if (string.IsNullOrEmpty(id)) return false;
            RegisterDefaults();
            Recipe r;
            if (!recipes.TryGetValue(id, out r)) return false;
            QualityBudget b = TierManager.Budget;
            if (distanceToViewer > b.FarRing) return false;
            if (PoolRunner.Instance.LiveCount >= b.MaxLiveVfx) return false;
            float scale = b.EffectScale;
            if (distanceToViewer > b.MidRing) scale *= 0.5f;
            else if (distanceToViewer > b.NearRing) scale *= 0.75f;
            GameObject go = PrefabPool.Spawn(id, pos, Quaternion.identity, r.Life + 0.6f);
            if (go == null) return false;
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            ParticleSystem.EmissionModule e = ps.emission;
            e.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)Mathf.Max(2, Mathf.RoundToInt(r.Count * scale))) });
            ps.Clear();
            ps.Play();
            return true;
        }
    }
}
