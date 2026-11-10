using System.Collections.Generic;
using UnityEngine;
using SL.Core;

namespace SL.Systems
{
    /// <summary>
    /// Loads the owner's base meshes (Assets/Resources/Meshes/*.obj), fixes winding, paints vertex colours from a species recipe,
    /// scales to real-world size and stands the model on the ground. Falls back to null so the code-built creature is used.
    /// </summary>
    public static class BaseMeshLibrary
    {
        // Facing correction in degrees. Orientation of each mesh is a best guess from its shape; change a number here if one walks sideways or backwards.
        static readonly Dictionary<string, float> yawFix = new Dictionary<string, float>
        {
            { "monster_wolf", 180f }, { "monster_boar", 0f }, { "monster_bat", 0f }, { "monster_wasp", 0f }, { "monster_spider", 0f },
            { "monster_scorpion", 0f }, { "monster_beetle", 0f }, { "monster_troll", 0f }, { "monster_mole", 0f }, { "monster_slime", 0f }
        };

        static readonly Dictionary<string, Mesh> cache = new Dictionary<string, Mesh>();
        static readonly Dictionary<string, Mesh> sources = new Dictionary<string, Mesh>();

        public static bool Exists(string meshName)
        {
            return Resources.Load<GameObject>("Meshes/" + meshName) != null;
        }

        static Mesh LoadSource(string meshName)
        {
            Mesh m;
            if (sources.TryGetValue(meshName, out m) && m != null) return m;
            GameObject go = Resources.Load<GameObject>("Meshes/" + meshName);
            if (go == null) return null;
            MeshFilter mf = go.GetComponentInChildren<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) return null;
            sources[meshName] = mf.sharedMesh;
            return mf.sharedMesh;
        }

        static void EnsureOutward(int[] tris, Vector3[] v, Vector3[] n)
        {
            if (tris == null || tris.Length < 3) return;
            double sum = 0;
            int count = Mathf.Max(1, Mathf.Min(tris.Length / 3, 3000));
            int step = Mathf.Max(1, (tris.Length / 3) / count);
            for (int t = 0; t < tris.Length / 3; t += step)
            {
                int a = tris[t * 3], b = tris[t * 3 + 1], c = tris[t * 3 + 2];
                Vector3 fn = Vector3.Cross(v[b] - v[a], v[c] - v[a]);
                Vector3 vn = n[a] + n[b] + n[c];
                sum += Vector3.Dot(fn.normalized, vn.normalized);
            }
            if (sum < 0)
            {
                for (int t = 0; t < tris.Length; t += 3) { int tmp = tris[t + 1]; tris[t + 1] = tris[t + 2]; tris[t + 2] = tmp; }
            }
        }

        static Mesh Colorize(Mesh src, Color a, Color b, Color c)
        {
            if (!src.isReadable) return src; // import setting missing: use as is (white)
            Mesh m = new Mesh();
            m.name = src.name + "_species";
            Vector3[] v = src.vertices;
            Vector3[] n = src.normals;
            int[] tris = src.triangles;
            if (n == null || n.Length != v.Length) { m.vertices = v; m.triangles = tris; m.RecalculateNormals(); n = m.normals; }
            EnsureOutward(tris, v, n);
            Bounds bb = src.bounds;
            Color[] cols = new Color[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                float h = Mathf.InverseLerp(bb.min.y, bb.max.y, v[i].y);
                float up = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(-0.4f, 0.4f, n[i].y));
                Color col = Color.Lerp(b, a, up);
                if (h < 0.12f) col = Color.Lerp(col, c, (0.12f - h) / 0.12f);
                float hash = Mathf.Abs(Mathf.Sin(v[i].x * 91.7f + v[i].y * 47.3f + v[i].z * 63.1f)) * 0.12f;
                col = new Color(Mathf.Clamp01(col.r * (0.94f + hash)), Mathf.Clamp01(col.g * (0.94f + hash)), Mathf.Clamp01(col.b * (0.94f + hash)), 1f);
                cols[i] = col;
            }
            m.vertices = v;
            m.normals = n;
            m.uv = src.uv;
            m.triangles = tris;
            m.colors = cols;
            m.RecalculateBounds();
            return m;
        }

        static Color Rgb(int rgb)
        {
            return new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f, 1f);
        }

        public static GameObject BuildForSpecies(string speciesId)
        {
            SpeciesRecord s = BeginnerSpecies.Get(speciesId);
            if (s == null) return null;
            return Build(s.BaseMesh, s.SizeMeters, Rgb(s.ColorA), Rgb(s.ColorB), Rgb(s.ColorC));
        }

        /// <summary>Builds a standing model, longest side = sizeMeters, feet on y = 0, centred on x and z.</summary>
        public static GameObject Build(string meshName, float sizeMeters, Color a, Color b, Color c)
        {
            Mesh src = LoadSource(meshName);
            if (src == null) return null;
            string key = meshName + a + b + c;
            Mesh m;
            if (!cache.TryGetValue(key, out m) || m == null) { m = Colorize(src, a, b, c); cache[key] = m; }

            GameObject root = new GameObject("Base_" + meshName);
            GameObject vis = new GameObject("mesh");
            vis.transform.SetParent(root.transform, false);
            Bounds bb = m.bounds;
            float longest = Mathf.Max(bb.size.x, Mathf.Max(bb.size.y, bb.size.z));
            float sc = sizeMeters / Mathf.Max(0.001f, longest);
            float yaw;
            if (!yawFix.TryGetValue(meshName, out yaw)) yaw = 0f;
            Quaternion rot = Quaternion.Euler(0f, yaw, 0f);
            Vector3 off = rot * new Vector3(-bb.center.x * sc, 0f, -bb.center.z * sc);
            off.y = -bb.min.y * sc;
            vis.transform.localScale = Vector3.one * sc;
            vis.transform.localRotation = rot;
            vis.transform.localPosition = off;
            MeshFilter mf = vis.AddComponent<MeshFilter>();
            mf.sharedMesh = m;
            MeshRenderer mr = vis.AddComponent<MeshRenderer>();
            mr.sharedMaterial = SLMat.Vertex();
            mr.shadowCastingMode = TierManager.Budget.RealtimeShadows ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            return root;
        }
    }
}
