using System.Collections.Generic;
using UnityEngine;

namespace SL.Systems
{
    public enum Ring { Full = 0, Mid = 1, Far = 2, Hidden = 3 }

    public struct AoiEntry
    {
        public int Id;
        public Vector3 Pos;
        public bool Priority; // friends, party members and the current target stay full detail
    }

    /// <summary>Client side: decides which detail ring each other player is drawn in, capped by the phone tier.</summary>
    public sealed class AoiSelector
    {
        struct Rank { public int Id; public float D; public bool Pri; }
        readonly Dictionary<int, Ring> prev = new Dictionary<int, Ring>();
        readonly List<Rank> ranks = new List<Rank>();

        static int Cmp(Rank a, Rank b)
        {
            if (a.Pri != b.Pri) return a.Pri ? -1 : 1;
            return a.D.CompareTo(b.D);
        }

        static Ring RingFor(float d, QualityBudget b)
        {
            if (d < b.NearRing) return Ring.Full;
            if (d < b.MidRing) return Ring.Mid;
            if (d < b.FarRing) return Ring.Far;
            return Ring.Hidden;
        }

        static float Edge(Ring r, QualityBudget b)
        {
            switch (r) { case Ring.Full: return b.NearRing; case Ring.Mid: return b.MidRing; case Ring.Far: return b.FarRing; default: return float.MaxValue; }
        }

        public void Assign(Vector3 viewer, IList<AoiEntry> entries, QualityBudget b, Dictionary<int, Ring> result)
        {
            result.Clear();
            ranks.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                Rank r = new Rank();
                r.Id = entries[i].Id; r.Pri = entries[i].Priority;
                r.D = Vector3.Distance(viewer, entries[i].Pos);
                ranks.Add(r);
            }
            ranks.Sort(Cmp);
            int full = 0;
            for (int i = 0; i < ranks.Count; i++)
            {
                Rank r = ranks[i];
                Ring ring = RingFor(r.D, b);
                Ring old;
                if (prev.TryGetValue(r.Id, out old) && old < ring && r.D < Edge(old, b) * 1.1f) ring = old; // hysteresis
                if (r.Pri && r.D < b.MidRing) ring = Ring.Full;
                if (ring == Ring.Full)
                {
                    if (full >= b.FullDetailPlayers && !r.Pri) ring = Ring.Mid;
                    else full++;
                }
                result[r.Id] = ring;
                prev[r.Id] = ring;
            }
        }

        public void Forget(int id) { prev.Remove(id); }
    }

    /// <summary>Server side: uniform grid so each phone is sent only what is near its player.</summary>
    public sealed class AoiGrid
    {
        readonly float cell;
        readonly Dictionary<long, List<int>> cells = new Dictionary<long, List<int>>();
        readonly Dictionary<int, long> where = new Dictionary<int, long>();
        readonly Dictionary<int, Vector2> pos = new Dictionary<int, Vector2>();

        public AoiGrid(float cellSize) { cell = Mathf.Max(1f, cellSize); }

        long Key(int cx, int cy) { return ((long)cx << 32) ^ (long)(uint)cy; }

        public void Set(int id, Vector2 p)
        {
            long k = Key(Mathf.FloorToInt(p.x / cell), Mathf.FloorToInt(p.y / cell));
            long old;
            if (where.TryGetValue(id, out old))
            {
                if (old != k) { List<int> l; if (cells.TryGetValue(old, out l)) l.Remove(id); }
                else { pos[id] = p; return; }
            }
            List<int> list;
            if (!cells.TryGetValue(k, out list)) { list = new List<int>(); cells[k] = list; }
            list.Add(id);
            where[id] = k;
            pos[id] = p;
        }

        public void Remove(int id)
        {
            long k;
            if (where.TryGetValue(id, out k)) { List<int> l; if (cells.TryGetValue(k, out l)) l.Remove(id); }
            where.Remove(id); pos.Remove(id);
        }

        public void Query(Vector2 center, float radius, List<int> output)
        {
            output.Clear();
            int x0 = Mathf.FloorToInt((center.x - radius) / cell), x1 = Mathf.FloorToInt((center.x + radius) / cell);
            int y0 = Mathf.FloorToInt((center.y - radius) / cell), y1 = Mathf.FloorToInt((center.y + radius) / cell);
            float r2 = radius * radius;
            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                {
                    List<int> l;
                    if (!cells.TryGetValue(Key(x, y), out l)) continue;
                    for (int i = 0; i < l.Count; i++)
                        if ((pos[l[i]] - center).sqrMagnitude <= r2) output.Add(l[i]);
                }
        }
    }
}
