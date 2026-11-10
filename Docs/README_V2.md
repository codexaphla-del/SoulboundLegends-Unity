# Soulbound Legends — V2 Systems build (for Unity Cloud Build)

Base: the V1 Foundation (Fixed2) project that already built in Cloud Build, plus the new systems and the owner's base meshes.
**This was written without being able to compile or run Unity, so expect to send back any compile error from the first build log.**

## Using it with Unity Cloud Build (Build Automation)
1. Unity Build Automation builds from a **source-control repository**, not from an uploaded zip. Unzip this project and commit it to your repo (Git or Unity Version Control). A `.gitignore` is included.
2. Unity version: `ProjectSettings/ProjectVersion.txt` (Unity 6, 6000.5.x). Use the same version in the build target.
3. Pre-Export method name: `SL.EditorTools.SLEditorSetup.CloudPreExport` (already in the project from V1).
4. Build target Android (or Windows). First build will import the new OBJ meshes (about 6 MB).
5. Inside the editor, menu **Soulbound > Export Project Zip** makes a fresh zip of Assets, Packages and ProjectSettings on your desktop.

## Direct3D 11 (Windows editor)
Menu **Soulbound > Use Direct3D11 on Windows**, or Edit > Project Settings > Player > Other Settings, untick Auto Graphics API for Windows, and put Direct3D11 first.

## What is new in this build
- `Assets/SL/Systems/` (new assembly SL.Systems, starts automatically, no scene setup):
  - **Perf.cs** phone tiers Low / Mid / High (2 GB RAM = Low), budgets from the design, settings override, automatic step-down if the frame rate drops.
  - **Aoi.cs** area-of-interest: client detail rings (full / mid / far / hidden) capped per tier with hysteresis, and a server-side grid.
  - **Pooling.cs** object pool plus pooled particle effects built in code, with level of detail (scaled by tier and distance, capped live effects).
  - **Combat.cs** every combat action has distinct **PreCast, Active and Recovery** frames, readable hitboxes (gizmos), cancel windows, server-confirm hit events, and a validator that rejects an action missing any phase. 14 actions (player and creature).
  - **GameData.cs** item grades, the gods enum (Omnigod and hidden Shadow God included), the **20 beginner species** (two per level, Levels 1 to 10), loot roller (server seed).
  - **BaseMeshLibrary.cs** loads your meshes from `Assets/Resources/Meshes`, fixes winding, paints vertex colours per species, scales to real size, stands them on the ground.
- Your **base meshes** (converted from Tripo GLB to OBJ so Unity opens them without extra packages), in `Assets/Resources/Meshes/`:
  | File | Triangles | What I identified it as |
  |---|---|---|
  | monster_slime | 2,894 | slime |
  | monster_wolf | 3,212 | wolf |
  | monster_boar | 3,816 | boar |
  | monster_bat | 2,006 | bat |
  | monster_wasp | 1,404 | wasp (unsure: could be another insect) |
  | monster_spider | 3,394 | spider |
  | monster_scorpion | 4,720 | scorpion |
  | monster_beetle | 4,116 | horned beetle |
  | monster_mole | 3,212 | mole (unsure: could be the toad) |
  | monster_troll | 7,734 | troll |
  | body_male / body_female | 3,212 each | headless bodies (not wired yet) |
  | head_a / head_b | 1,404 each | heads (not wired yet; which is male or female is unknown) |
- The 20 species are registered with the old game's monster list and spawn in the adventure world (2 each, further out for higher levels). They use your base meshes instead of the code-built shapes.
- Editor tools (menu **Soulbound**): Use Direct3D11, Validate Systems, Export Project Zip; and an import postprocessor that makes the meshes readable.

## Known limits of this build (honest list)
- **Not compiled or run.** Send the build log if anything fails.
- **Creature animation is still the old simple motion** (body bob / hop / flap on the whole mesh). Your meshes have no skeleton, so limb animation per the compendium (rig discovery, IK, landing, finishers) is NOT built yet.
- **Mesh facing may be wrong** for some creatures. Edit the numbers in `BaseMeshLibrary.cs` (`yawFix`) if one walks sideways or backwards.
- Only 10 of your 11 monster base types are present in the mesh files (no separate toad file was identified).
- **Bodies and heads are included but not used yet**: player characters are still the old code-built ones. Rigging them, hair, clothes, the customisation screen and the saved look are not built yet.
- **Terrain (Tripo island) is not imported.** It is about 2 million triangles; it needs the tile / LOD tool first. The old procedural terrain remains.
- **Village buildings, infrastructure and layout are not built yet** (specs are in `Docs/Village_Building_Spec.md`).
- **HUD:** the Master Game Design asks for a clean black-focused button language, which differs from the earlier translucent-grey Free Fire style. The HUD in this build is the old one; it has not been changed to either.
- Most of the other systems in `Docs/Soulbound_Legends_Master_Game_Design.txt` (store, Luck Royale, Wanted, mounts, housing, guilds and so on) are not built.
- The combat action system exists, but the old server combat has not been switched over to it yet.
- Cloud Build, the Unity MCP connection and these systems have not been tested together.

## Decisions I made where documents conflict (reversible)
- God list: ten gods plus Omnigod and a hidden Shadow God (Master Game Design), replacing the earlier list.
- Beginner monsters: the Master Game Design rule (two species per level, Levels 1 to 10) is used.
- Materials in village buildings and infrastructure follow the pictures exactly (stone is stone).
