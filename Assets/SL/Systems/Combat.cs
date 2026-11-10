using System;
using System.Collections.Generic;
using UnityEngine;

namespace SL.Systems
{
    public enum CombatPhase { Idle = 0, PreCast = 1, Active = 2, Recovery = 3 }
    public enum HitShape { Sphere = 0, Box = 1 }

    [Serializable]
    public sealed class HitboxDef
    {
        public string Name;
        public HitShape Shape;
        public Vector3 Offset;      // local to the attacker
        public Vector3 Size;        // box half-extents
        public float Radius;        // sphere radius
        public int StartFrame;      // inside the Active phase, 0-based
        public int EndFrame;        // inclusive
        public float DamageScale = 1f;

        public HitboxDef(string name, HitShape shape, Vector3 offset, float radius, Vector3 size, int start, int end, float dmg)
        {
            Name = name; Shape = shape; Offset = offset; Radius = radius; Size = size; StartFrame = start; EndFrame = end; DamageScale = dmg;
        }
    }

    /// <summary>
    /// Every action has three distinct phases so hitboxes are readable: PreCast (wind-up, harmless, telegraphs the move),
    /// Active (hitboxes live) and Recovery (vulnerable, cancel window may open). Frames are counted at FrameRate (default 60).
    /// </summary>
    [Serializable]
    public sealed class CombatActionDef
    {
        public string Id, Name;
        public int FrameRate = 60;
        public int PreCastFrames, ActiveFrames, RecoveryFrames;
        public float Damage = 1f, Stamina, Mana;
        public int CancelStart = -1, CancelEnd = -1;   // frames counted from the start of the whole action
        public string PreCastVfx, ActiveVfx, RecoveryVfx;
        public string PreCastAnim, ActiveAnim, RecoveryAnim;
        public List<HitboxDef> Hitboxes = new List<HitboxDef>();

        public int TotalFrames { get { return PreCastFrames + ActiveFrames + RecoveryFrames; } }
        public float Seconds { get { return TotalFrames / (float)Mathf.Max(1, FrameRate); } }

        public CombatPhase PhaseAt(int frame)
        {
            if (frame < 0) return CombatPhase.Idle;
            if (frame < PreCastFrames) return CombatPhase.PreCast;
            if (frame < PreCastFrames + ActiveFrames) return CombatPhase.Active;
            if (frame < TotalFrames) return CombatPhase.Recovery;
            return CombatPhase.Idle;
        }

        /// <summary>Returns null when the action is valid, otherwise a readable error.</summary>
        public string Validate()
        {
            if (string.IsNullOrEmpty(Id)) return "Action has no id.";
            if (PreCastFrames < 2) return Id + ": PreCast needs at least 2 frames so the move is telegraphed.";
            if (ActiveFrames < 1) return Id + ": Active needs at least 1 frame.";
            if (RecoveryFrames < 2) return Id + ": Recovery needs at least 2 frames.";
            if (FrameRate < 10) return Id + ": FrameRate too low.";
            for (int i = 0; i < Hitboxes.Count; i++)
            {
                HitboxDef h = Hitboxes[i];
                if (h.StartFrame < 0 || h.EndFrame < h.StartFrame || h.EndFrame >= ActiveFrames)
                    return Id + ": hitbox '" + h.Name + "' must sit inside the Active frames.";
                if (h.Shape == HitShape.Sphere && h.Radius <= 0f) return Id + ": hitbox '" + h.Name + "' has no radius.";
            }
            if (Hitboxes.Count == 0) return Id + ": needs at least one hitbox.";
            if (CancelStart >= 0 && (CancelEnd < CancelStart || CancelEnd >= TotalFrames)) return Id + ": bad cancel window.";
            return null;
        }
    }

    public static class CombatActionLibrary
    {
        static readonly Dictionary<string, CombatActionDef> map = new Dictionary<string, CombatActionDef>();
        public static readonly List<CombatActionDef> All = new List<CombatActionDef>();

        static CombatActionDef A(string id, string name, int pre, int act, int rec, float dmg, float stamina, float mana,
                                 string preVfx, string actVfx, string recVfx, string anim)
        {
            CombatActionDef d = new CombatActionDef();
            d.Id = id; d.Name = name; d.PreCastFrames = pre; d.ActiveFrames = act; d.RecoveryFrames = rec;
            d.Damage = dmg; d.Stamina = stamina; d.Mana = mana;
            d.PreCastVfx = preVfx; d.ActiveVfx = actVfx; d.RecoveryVfx = recVfx;
            d.PreCastAnim = anim + "_precast"; d.ActiveAnim = anim + "_active"; d.RecoveryAnim = anim + "_recovery";
            d.CancelStart = pre + act + rec / 2; d.CancelEnd = pre + act + rec - 1;
            All.Add(d); map[id] = d;
            return d;
        }

        static CombatActionLibrary()
        {
            // Player actions
            A("light_left", "Left-hand strike", 6, 5, 12, 1.0f, 6f, 0f, "precast_glow", "slash_arc", null, "atk_light_l")
                .Hitboxes.Add(new HitboxDef("blade", HitShape.Sphere, new Vector3(-0.4f, 1.0f, 1.2f), 0.9f, Vector3.zero, 0, 4, 1f));
            A("light_right", "Right-hand strike", 6, 5, 12, 1.0f, 6f, 0f, "precast_glow", "slash_arc", null, "atk_light_r")
                .Hitboxes.Add(new HitboxDef("blade", HitShape.Sphere, new Vector3(0.4f, 1.0f, 1.2f), 0.9f, Vector3.zero, 0, 4, 1f));
            A("heavy", "Heavy swing", 14, 6, 22, 2.2f, 16f, 0f, "precast_glow", "slash_arc", "dust", "atk_heavy")
                .Hitboxes.Add(new HitboxDef("arc", HitShape.Box, new Vector3(0f, 1.0f, 1.5f), 0f, new Vector3(1.4f, 0.8f, 1.2f), 0, 5, 1f));
            A("dodge", "Dodge", 3, 12, 8, 0f, 12f, 0f, null, "dust", null, "dodge")
                .Hitboxes.Add(new HitboxDef("evade_window", HitShape.Sphere, new Vector3(0f, 1f, 0f), 0.2f, Vector3.zero, 0, 11, 0f));
            A("skill_cast", "Skill cast", 18, 8, 20, 1.8f, 0f, 20f, "precast_glow", "fire_burst", "dust", "skill")
                .Hitboxes.Add(new HitboxDef("burst", HitShape.Sphere, new Vector3(0f, 1f, 2.0f), 1.8f, Vector3.zero, 0, 7, 1f));
            A("parry", "Block and parry", 4, 10, 14, 0.5f, 8f, 0f, null, "hit_spark", null, "parry")
                .Hitboxes.Add(new HitboxDef("guard", HitShape.Box, new Vector3(0f, 1f, 0.8f), 0f, new Vector3(0.9f, 1f, 0.5f), 0, 9, 0.5f));
            A("finisher", "Finisher", 20, 8, 30, 3.0f, 24f, 0f, "precast_glow", "thunder_arc", "dust", "finisher")
                .Hitboxes.Add(new HitboxDef("finish", HitShape.Sphere, new Vector3(0f, 1f, 1.6f), 2.0f, Vector3.zero, 0, 7, 1f));
            // Creature actions (compendium section 7)
            A("bite", "Bite", 10, 4, 18, 1.2f, 0f, 0f, "precast_glow", "hit_spark", null, "bite")
                .Hitboxes.Add(new HitboxDef("jaw", HitShape.Sphere, new Vector3(0f, 0.8f, 1.1f), 0.7f, Vector3.zero, 0, 3, 1f));
            A("claw", "Claw swipe", 9, 5, 16, 1.0f, 0f, 0f, "precast_glow", "slash_arc", null, "claw")
                .Hitboxes.Add(new HitboxDef("swipe", HitShape.Box, new Vector3(0f, 0.9f, 1.2f), 0f, new Vector3(0.9f, 0.6f, 0.8f), 0, 4, 1f));
            A("pounce", "Pounce", 16, 8, 24, 1.8f, 0f, 0f, "precast_glow", "dust", "dust", "pounce")
                .Hitboxes.Add(new HitboxDef("landing", HitShape.Sphere, new Vector3(0f, 0.6f, 1.6f), 1.2f, Vector3.zero, 2, 7, 1f));
            A("slam", "Stomp / slam", 24, 6, 34, 2.6f, 0f, 0f, "precast_glow", "dust", "dust", "slam")
                .Hitboxes.Add(new HitboxDef("shock", HitShape.Sphere, new Vector3(0f, 0.2f, 1.8f), 2.6f, Vector3.zero, 0, 5, 1f));
            A("tail_whip", "Tail whip", 12, 5, 18, 1.1f, 0f, 0f, "precast_glow", "slash_arc", null, "tailwhip")
                .Hitboxes.Add(new HitboxDef("tail", HitShape.Sphere, new Vector3(0f, 0.7f, -1.4f), 1.2f, Vector3.zero, 0, 4, 1f));
            A("charge", "Charge", 20, 14, 30, 2.0f, 0f, 0f, "precast_glow", "dust", "dust", "charge")
                .Hitboxes.Add(new HitboxDef("ram", HitShape.Box, new Vector3(0f, 0.8f, 1.0f), 0f, new Vector3(0.9f, 0.8f, 1.0f), 0, 13, 1f));
        }

        public static CombatActionDef Get(string id)
        {
            CombatActionDef d;
            return map.TryGetValue(id, out d) ? d : null;
        }

        public static List<string> ValidateAll()
        {
            List<string> errors = new List<string>();
            for (int i = 0; i < All.Count; i++)
            {
                string e = All[i].Validate();
                if (e != null) errors.Add(e);
            }
            return errors;
        }
    }

    /// <summary>
    /// Runs one action at a time, frame by frame, and fires phase events (for animation and VFX) plus hit events.
    /// Hits are PREDICTIONS on the client; the server must confirm damage.
    /// </summary>
    public sealed class CombatActionRunner : MonoBehaviour
    {
        public CombatActionDef Current { get; private set; }
        public CombatPhase Phase { get; private set; }
        public int Frame { get; private set; }
        public LayerMask HitMask = ~0;
        public bool ShowGizmos = true;
        public event Action<CombatActionDef, CombatPhase> PhaseChanged;
        public event Action<CombatActionDef, HitboxDef, Collider> Hit;

        float acc;
        readonly Collider[] buf = new Collider[24];
        readonly HashSet<int> alreadyHit = new HashSet<int>();

        public bool IsBusy { get { return Phase != CombatPhase.Idle; } }

        public bool TryStart(CombatActionDef def)
        {
            if (def == null) return false;
            if (IsBusy)
            {
                bool canCancel = Current != null && Current.CancelStart >= 0 && Frame >= Current.CancelStart && Frame <= Current.CancelEnd;
                if (!canCancel) return false;
            }
            Current = def; Frame = -1; acc = 0f; alreadyHit.Clear();
            SetPhase(CombatPhase.PreCast);
            Frame = 0;
            return true;
        }

        public void Cancel() { Current = null; SetPhase(CombatPhase.Idle); }

        void SetPhase(CombatPhase p)
        {
            if (p == Phase) return;
            Phase = p;
            if (PhaseChanged != null) PhaseChanged(Current, p);
        }

        void Update()
        {
            if (Phase == CombatPhase.Idle || Current == null) return;
            acc += Time.deltaTime;
            float step = 1f / Mathf.Max(1, Current.FrameRate);
            int guard = 0;
            while (acc >= step && guard++ < 8) { acc -= step; Step(); if (Current == null) break; }
        }

        void Step()
        {
            Frame++;
            CombatPhase p = Current.PhaseAt(Frame);
            if (p == CombatPhase.Idle) { Current = null; SetPhase(CombatPhase.Idle); return; }
            SetPhase(p);
            if (p == CombatPhase.Active) TestHitboxes(Frame - Current.PreCastFrames);
        }

        void TestHitboxes(int activeFrame)
        {
            for (int i = 0; i < Current.Hitboxes.Count; i++)
            {
                HitboxDef h = Current.Hitboxes[i];
                if (activeFrame < h.StartFrame || activeFrame > h.EndFrame || h.DamageScale <= 0f) continue;
                Vector3 c = transform.TransformPoint(h.Offset);
                int n = h.Shape == HitShape.Sphere
                    ? Physics.OverlapSphereNonAlloc(c, h.Radius, buf, HitMask, QueryTriggerInteraction.Ignore)
                    : Physics.OverlapBoxNonAlloc(c, h.Size, buf, transform.rotation, HitMask, QueryTriggerInteraction.Ignore);
                for (int k = 0; k < n; k++)
                {
                    Collider col = buf[k];
                    if (col == null || col.transform.IsChildOf(transform)) continue;
                    int key = col.GetHashCode() * 31 + i;
                    if (!alreadyHit.Add(key)) continue;
                    if (Hit != null) Hit(Current, h, col);
                }
            }
        }

        void OnDrawGizmos()
        {
            if (!ShowGizmos || Current == null) return;
            Color c = Phase == CombatPhase.PreCast ? new Color(1f, 0.9f, 0.2f, 0.6f)
                    : (Phase == CombatPhase.Active ? new Color(1f, 0.2f, 0.2f, 0.6f) : new Color(0.3f, 0.6f, 1f, 0.4f));
            Gizmos.color = c;
            int af = Frame - Current.PreCastFrames;
            for (int i = 0; i < Current.Hitboxes.Count; i++)
            {
                HitboxDef h = Current.Hitboxes[i];
                Vector3 p = transform.TransformPoint(h.Offset);
                if (h.Shape == HitShape.Sphere) Gizmos.DrawWireSphere(p, h.Radius);
                else { Matrix4x4 old = Gizmos.matrix; Gizmos.matrix = Matrix4x4.TRS(p, transform.rotation, Vector3.one); Gizmos.DrawWireCube(Vector3.zero, h.Size * 2f); Gizmos.matrix = old; }
            }
        }
    }
}
