# SOULBOUND LEGENDS — COMPLETE HANDOFF PROMPT

Paste this whole file into a new chat with Claude. Also attach the two spec files (`Village_Building_Spec.md` and `Creature_Animation_Compendium_v3.md`) and re-upload the reference pictures listed in Section 14, because a new chat cannot see pictures from the old one.

---

## 0. HOW TO USE THIS PROMPT (instructions to Claude)

You are continuing a long project with the owner of a Unity mobile game. Everything below was agreed in an earlier conversation. Follow it exactly.

Working rules (mandatory):
1. **Be truthful.** The owner wants truthful answers, and wants every possible option mentioned when answering a question. If you cannot do something, or cannot see something clearly, say so plainly. Never invent details of an image, a mesh, or a result you have not seen.
2. **Follow instructions word for word.** When the owner gives a list or a document, follow it exactly. Do not skip, shorten, reorder or silently change anything. If an instruction is unclear, conflicts with another, or cannot be done (for example because of a phone limit), say so and ask. Never replace it without telling the owner.
3. **Reference pictures are binding.** Build what the pictures show. If a picture shows stone, use stone. If it shows timber, use timber. No exceptions. If a detail is too small or unclear to read, mark it "unclear" and ask before building it.
4. **Do not start building until the owner says start.** Reading and checking the Unity project is fine. Changing it is not, until told.
5. **Work in small steps** and check the Unity console after each one. Report what changed, what to test, and any warnings.
6. **Compare against the references.** After building a scene, capture an editor screenshot and compare it with the matching reference picture, then say where it differs.
7. **Back up first.** Ask the owner to commit the project to Git (or copy the folder) before large changes. Do not delete the owner's assets without asking.

---

## 1. WHAT THE GAME IS

**Soulbound Legends** is a mobile fantasy open-world MMORPG built in Unity. Its tagline is "10 Inheritances. 1 Destiny. Explore. Fight. Build. Become a Legend." Its store text describes an epic open-world fantasy MMORPG with intense battles, multiple game modes, deep customization and a living world.

- **Modes (from the design sheet):**
  - **Adventure World** (unlocked from the start): quests, NPCs, crafting, dungeons, rifts, Supreme Ruins, God Inheritances.
  - **CS (Clash Squad):** practice and sharpen, elimination battles.
  - **BR (Battle Royale):** adapt and survive, large maps, random teleport, up to 100 players.
  - **Siege:** coordinate and execute, attack and defend, best of 2, final objectives.
  - The small print on the sheet (map counts) is hard to read, so confirm numbers with the owner.
  - CS, BR and Siege are locked until **Level 10**.
- **The elemental God Inheritances (corrected from the Master Game Design file):** Water, Fire, Earth, Void, Thunder, Nature, Light, Air, Ice and the Omnigod, plus a hidden Shadow God.
- **Currencies:** **gold (coins)** buys and crafts everything in village shops, and village shops are open from Level 0. **Aether** is the premium currency, and the premium store stays locked until Level 10.
- **Other systems on the design sheet:** Luck Royale (spin rewards), events, world chat, store, character gear, weapons and skills, character voice, quests and NPCs, dungeons and bosses, rift events, Supreme Ruin, PK system, multiplayer instances, Sentinel (security), settings.
- **Regional note:** paid random rewards (Luck Royale) are restricted in some countries and app stores require odds disclosure. Remind the owner to check before launch.

### The 24 screens (the owner's design sheet)
1 Loading screen · 2 Opening intro · 3 Account sign in (Apple, Google, Email) · 4 Enter your name · 5 Character customisation · 6 Main lobby · 7 World chat · 8 Store · 9 Luck Royale · 10 Events · 11 Game modes · 12 In-game HUD and controls · 13 Character and gear · 14 Weapons and skills · 15 Character voice · 16 Quests and NPCs · 17 Dungeons and bosses · 18 Rift events · 19 Supreme Ruin · 20 God Inheritances · 21 PK system · 22 Multiplayer instances · 23 Soulbound Sentinel · 24 Settings.
Plus a key-art banner (top left) and a closing banner (bottom right).

**Flow the owner stated:** entrance, loading, name entry, character customisation, lobby, then profile, vault, events and Luck Royale.

---

## 2. HOW THE GAME WILL BE MADE (the agreed method)

**History:** the first plan was that Claude would write code and give a zip, the owner would build it with Unity Cloud Build, and send back errors. That is **replaced**.

**Current plan:**
1. The owner connects Claude to Unity through **Unity MCP** on their laptop, with maximum permissions.
2. Claude builds the game **directly inside the owner's Unity project**.
3. The owner tests in the Unity editor.
4. When it is good, the owner exports the game.

**Connection status at the end of the last chat:** the connection worked for reading. The project is a fresh **URP template**: scene `SampleScene` with only a Main Camera, Directional Light and Global Volume; mobile and PC renderer assets; the input system actions file; and the tutorial files. The console had no errors, only three harmless warnings from Unity's own AI assistant plugin about account access and signature checks. Nothing had been changed. The create, script and play-mode tools had not been tested yet.

**What Claude can do through the connection:** read the project, scenes, console and profiler; create scripts, scenes, prefabs and materials; import meshes and textures; wire UI; run the editor and read results; capture screenshots.

**What Claude cannot do:** make art (models, textures, icons, audio), provide servers, accounts or payments, judge how the game feels, or test on a real phone. Those stay with the owner.

**Build order (batches):** (1) foundation (landscape fix, URP and quality setup, scene flow), then screens 1 to 6 (loading to lobby); (2) the playable hero with movement, camera and combat; (3) the village; (4) systems (inventory, quests, crafting, store, events, chat); (5) monsters and the world; (6) multiplayer, modes and anti-cheat.

**Earlier code:** an older code-built project exists (`SoulboundLegends_V1_Foundation_Fixed2.zip`, plus a `Fixed3` with smooth bodies). It had code-built characters and monsters, many screens, quests, crafting, loot and basic craters. The new Unity project is separate and empty. Ask the owner if they want any of the old code brought in.

---

## 3. WHAT THE OWNER UPLOADS, AND WHERE

**Inside the Unity project (`Assets/`)** because Unity can only load files inside the project folder. Suggested folders:
- `Assets/Art/Backdrops/` the nine backdrop images
- `Assets/Art/Characters/` male and female bodies, heads, base clothes
- `Assets/Art/Monsters/` monster base meshes and extra parts
- `Assets/Art/Terrain/` the terrain mesh and textures
- `Assets/Docs/` the spec files, so they travel with the project

**In the chat (reference pictures only):** the design collage, building and infrastructure sheets, layout blueprint, HUD prototype, Free Fire screenshots, goblin sheet, monster pictures. Unity never reads these. Claude looks at them while building.

**Tripo notes:** exports from Tripo may come as GLB or FBX, and phone apps may save them with no file extension. GLB files from Tripo are compressed with meshopt, which some tools cannot open. Free-tier Tripo models are for non-commercial use, so check the license before shipping a released game.

---

## 4. CHARACTERS (human varieties from base meshes)

**Quality target:** in-game characters should look like **Free Fire's in-game models**, not like the promo art. Clean silhouette, flat-shaded clothing, small details painted as texture or vertex color, toon shading with outlines.

**Base meshes the owner provides**
- **Male body:** about 2,757 vertices and **3,212 triangles**, no head, A-pose, no skeleton.
- **Female body:** about 2,790 vertices and **3,212 triangles**, no head, A-pose, no skeleton.
- **Two heads:** about **1,404 triangles** each (unknown which is male and which is female; ask).
- A female FBX with texture maps also exists (4096 px base color, plus normal, roughness and metallic maps). The base textures are plain grey, so tint and reduce them for phones.

**What Claude generates from code, so the owner does not need to bring it**
- **Hair:** 12 or more styles (spiky, swept, long, ponytail, braids, topknot, short crops, wild anime styles), each with color and a glossy highlight, fitted to the heads.
- **Clothes:** tops, jackets, pants, boots, gloves, helmets, capes and accessories, built as shells that fit the body (so any outfit fits any body shape). The owner may also bring base garments (a jacket, coat, armor chest, pants, boots, gloves, helmet, cape) that Claude tunes into variants.
- **Faces and body variety:** face choices (about 12 or more), eye shapes and brows, skin tones, eye and hair colors, and body sliders (height, weight, muscle, shoulders, waist, hips, chest, arm and leg length) implemented as blend shapes on the base meshes, with capped ranges.
- **Customisation screen (screen 5):** tabs for Face, Hair, Body, Outfit, Weapon, Accessories, Voice and Name, with Randomize and Reset buttons, a Front, Side and Back preview, and a Chosen Style panel.
- **One source of truth:** the look chosen after the name screen is packed into a small saved value and used for the lobby, the world, other players, the profile picture and portraits. Players can **change their look later**.
- **Rig:** the project's skeleton has **18 bones** (root, hips, spine, chest, neck, head, left and right shoulder, elbow and hand, left and right hip, knee and foot). Bones must be named to match the animation compendium's prefix tables (Section 6). Hands move as one piece (no finger bones).
- **Gender:** body type and voice options stay as in the original creator.

---

## 5. MONSTERS (varieties from base meshes)

**Method:** the owner uploads **base meshes** and a set of **extra parts**. Claude builds every species as a short **recipe** applied to the same base mesh: a base, parts, size and proportions, colors and surface patterns, an element effect, and behavior numbers. All variants of a base share its mesh, skeleton and animations. Only the species ID travels over the network.

**What a recipe can change:** size and proportions; fur or skin color, patterns and scars drawn on the texture; added parts (horns, tusks, spikes, armor plates, tails, wings, fins, claws, crowns); element effects (fire cracks and embers, ice frost and crystals, shadow smoke, thunder arcs); and behavior (speed, attack timing).

**Limits:** a base cannot become a different creature (a wolf stays a wolf). Effects cost frame rate and scale down on weak phones.

**The 11 base meshes from the owner's pictures:** slime (blob), wolf, boar, bat, spider, beetle, scorpion, wasp, toad, mole, troll. The earlier plan also kept goblin, drake, floating spirit and harpy unless the owner cuts them.

**Beginner village roster (about Lv 1 to 8):**
- Slime base: Mossy Gloop, Mud Slime, Honey Slime, Frost Slime
- Wolf base: Fangrunner, Forest Wolf, Grey Wolf, Scarred Wolf
- Humanoid (goblin) base: Goblin Grunt, Archer, Shaman, **Goblin Chieftain** (village boss)
- Bat base: Cave Bat, Venom Wisp, Night Bat

**One kingdom roster (about Lv 8 to 25):** heavy golem (Stone Golem, Ashen Brute, Moss Golem, **Ancient Warden** boss), floating spirit (Grave Wraith, Storm Spirit), winged humanoid (Gale Harpy and variants), drake (Ember, Frost, Swamp Drakelings), serpent or fish (River Serpent, Fin Stalker), spider (Cave, Venom, Web Weaver). With the owner's pictures added: boar, scorpion, beetle, wasp, toad, mole, troll.

**Pictures' material and copyright notes:** some monster pictures carry watermarks or credits (one is credited to a game company). Use all of them as mood references only and make original versions. Do not feed them into other tools.

**Monster triangle budgets (nearest level of detail):** slime 800 to 1,200 · wasp 1,500 to 2,000 · bat 1,500 to 2,500 · mole 2,000 to 3,000 · toad 2,500 to 3,500 · spider 2,500 to 4,000 · boar 3,000 to 4,000 · wolf 3,000 to 4,000 · beetle 3,000 to 4,500 · scorpion 3,500 to 5,000 · troll (boss) 5,000 to 8,000 · extra parts 100 to 800 each. A species with parts should end up no more than about 30% above its base. Detail goes into texture and effects, not triangles.

---

## 6. ANIMATION (the rules)

**Follow `Creature_Animation_Compendium_v3.md` word for word.** It includes sections 0 to 12 plus **section 13**, added by the owner: foot IK on real ground (slopes, steps), landing (compression scaled to fall height, per-foot landing), position control (stance, foot spacing, root height, facing, weight shift), hand and weapon IK, look and aim IK, body adaptation to measured leg, arm and torso lengths, IK performance tiers, and new manifest fields and error codes.

**Key obligations from the compendium:**
- Discover bones by name (never guess), validate or synthesize a base pose, identify the creature type, build clips from the helper library, set tangents, enforce the global rules, and output a **generator function, a ClipManifest and a validation report** for every clip, with a structured error when a rule cannot be met.
- Creature types: biped, quadruped, hexapod, octopod, flying, aquatic, serpentine, hybrid, beast or amorphous.
- Basic states (idle, walk, run, jump, land, hurt, block, dodge, death, cast and so on) and advanced actions (pounce, bite, swing, claw, tail whip, grapple, charge, stomp, roar, roll, dig, and more).

**Mapping for this game:** characters use Biped. Wolf, boar, mole and toad use Quadruped. Spider and scorpion use Octopod. Beetle uses Hexapod. Bat and wasp use Flying. Slime uses Beast or amorphous. Troll uses Biped.

**Owner's style requests:** smooth blending between idle, walk, run and attacks; stylish, flowing motion with weight shifts and follow-through; feet planted without sliding; **finishers and entrances built by combining small reusable moves** (step, crouch, slash, spin, jump, kneel, land) with layered body parts and events at exact frames (glows, impacts, sounds); and animation that adapts to each body's height, weight and limb length.

**Known quirks in the compendium (left unchanged; raise them only if one blocks a clip):** `DISCOVERY_FAILED` appears in section 2.4 but not in the section 1 error list; some role prefixes overlap (for example "Jaw" in both head and effector for quadrupeds); the biped support list names thigh, shin and foot but expects 2 bones.

---

## 7. COMBAT CONTROLS AND HUD

**Style:** the plain **translucent grey round buttons with white silhouette icons** from the Free Fire screenshots, with a few accent colors. Icons and button colors are drawn in code, with no art files needed.

**Layout (landscape, two-thumb grip):**
- **Left:** movement joystick (outer ring sprints, slide up to lock the run), **Left-hand attack placed beside the joystick**, weapon swap, heal, rune bomb (replaces grenade), backpack, a small collapsible chat and party list, sprint, crouch.
- **Right:** **Right-hand attack as the big button at the bottom right**, jump, dodge (drag to choose direction), block and parry, skill wheel (8 slots), interact (appears only near an NPC, shop, door or loot), camera drag on empty right-side space.
- **Top:** HP, mana and stamina bars; target HP and aggro; minimap and quest tracker; menu, technique book, settings, ping, compass, coins, players and kills counters.
- **Bottom center:** 3 to 4 quick-use slots; a **flow ring** around the attack buttons showing chain momentum.
- **Gun controls changed to fantasy:** fire becomes a sword-slash attack, aim becomes camera focus, the rifle panel becomes a weapon panel with a charge bar and Blade, Staff and Bow loadouts (bow has unlimited arrows), grenades become rune bombs, prone becomes Shadow Veil.
- **Mode-specific buttons** appear only when relevant (world map, quest tracker, team voice, BR zone, CS round info, Siege objective, mount).
- **Extra mechanics:** stamina and flow system, drag swings in 8 directions, technique certification toasts, shield guard arc.
- **Comfort options:** HUD editor (move, resize, fade buttons), left-hand mode, flick sensitivity, a tap-only mode with auto-direction.

---

## 8. THE VILLAGE, BUILDINGS AND INFRASTRUCTURE

**Binding rule:** everything is built **exactly as the pictures show**, including materials (stone is stone, timber is timber), with no exceptions. Unclear details are marked and asked about.

**Pictures to re-upload:** the **Building Showcase** (20 buildings, each with front, back, left, right exterior and four interior views), the **Infrastructure Showcase** (34 items), and the **Layout Blueprint**. `Village_Building_Spec.md` records everything read from them (Parts 1 to 5), including materials.

**20 buildings:** Village Chief's Hall, Adventurer's Guild, Weapon Shop (Blacksmith), Armor Shop, General Store, Potion/Alchemy Shop, Inn/Tavern, Training Grounds, Stable, Residential Houses, Storage/Warehouse, Market Square, Guard Barracks/Watchtower, Shrine/Elemental Temple, Crafting Workshop, Farmhouse/Mill, Farms & Gardens, Fountain/Well, Notice Board, Main Village Gate.

**34 infrastructure items:** Main Entrance Gate, Village Wall (stone), Watchtower, Stone Bridge, Wooden Bridge, Stone Staircase, Main Road, Pathway, Market Stalls, Fountain, Statue, Notice Board, Farms & Crop Fields, Animal Pen, Well, Windmill, Fishing Dock, Warehouse Yard, Gardens, Greenhouse, Lumberjack Site, Mining Site, Quarry, Campfire Site, Trees, Rocks, Flowers, Bushes, Lantern Posts, Fences, Banners & Flags, Barrels & Crates, Carts & Wagons, Signposts.

**Six items appear on both sheets** (gate, market, fountain and well, notice board, farms and gardens, windmill and mill). Build each once. That is about 47 unique pieces.

**Layout (from the blueprint, north at the top):** a stone-walled rectangle with corner towers, extra watchtowers, and stairs through the wall.
- **North gate:** Main Entrance Gate, with the **Chief's Hall just inside**. North row, west to east: Farms & Gardens, Farmhouse/Mill, Chief's Hall, Adventurer's Guild, Training Grounds.
- **Middle row:** Stable, Armor Shop, Weapon Shop, **Market Square with the fountain at the exact center**, Potion/Alchemy Shop, General Store, Inn/Tavern.
- **South row:** Residential Houses (about four), Storage/Warehouse, then the Main Road, Crafting Workshop, Shrine/Temple, Guard Barracks/Watchtower.
- **South gate:** Main Exit Gate. **Outside the south wall:** Fishing Dock on the south-west lake, Notice Board, Well, and Windmill with its farm plot in the south-east.
- **Roads:** the Main Road runs from the entrance gate past both sides of the Chief's Hall to the plaza, then straight south to the exit gate. Side paths form a grid.
- **Water:** a river down the west side into a lake in the south-west; streams and waterfalls beyond the north wall.
- **Not on the blueprint (ask the owner):** where the Mining Site, Quarry and Lumberjack Site go.
- **PENDING:** the owner said a **new layout that fits their terrain** is coming. Do not finalize placement until it arrives.

**Building behavior (all buildings):** working doors that open when the player walks up; solid walls with proper door gaps; a real interior room whose four walls match the Interior Front, Back, Left and Right views; interiors loaded only when the player is near, with the camera-facing wall fading when inside; staged destruction and repair; shared kit pieces (walls, roofs, windows, doors, chimneys, signs, lanterns, awnings) reused across buildings.

**NPCs:** built the same way as player characters (outfit, hair, gear by profession), with daily routines (walking between points, working, chatting, sitting), stopping to face the player, name tags, quest markers and short dialogue. Professions: blacksmith, armorer, alchemist, tailor, jeweler and enchanter, carpenter, cook, farmer, fisher, herbalist, general merchant, innkeeper, healer, banker, stable master, quest-board scribe, guard captain, elder, bard, villagers and children.

**Economy loop:** gather (herbs, ore, wood, fish, crops, stone) then craft (weapons, armor, potions, meals, enchants; brewing takes time) then buy and sell with gold. The server validates every purchase and craft.

**Other world features:** terrain destruction in **all modes** (craters sized by skill power, element-specific marks, building damage stages, props that react, rebuild and regrow, protected safe zones, capped changes); a realistic sun with a fantasy look (glow, rays, flare, day cycle, moon and stars at night); a village boundary.

---

## 9. TERRAIN

**The owner's terrain: a Tripo island mesh.** About **1,993,414 triangles**, 20 MB, one mesh with a flat central basin ringed by rocky hills and a solid underside; its base texture is plain grey. It is far too heavy for phones as it is.

**Owner's decision:** use the Tripo mesh as the real terrain (not a height-map rebuild by Claude, and not Claude's procedural terrain previews, which the owner rejected). Reduce the triangles with the tile and level-of-detail plan below.

**Reduction plan**
1. Cut the terrain into square **tiles** (about 32 by 32 m, around 80 tiles).
2. Make **three detail versions** of each tile: near (about 3,000 triangles), middle (about 800) and far (about 200).
3. Draw only tiles in view and within range. Chunks outside the camera are skipped; far chunks fade into fog; chunks load ahead of the player and unload behind.
4. Use skirts and fog to hide cracks between different detail levels.
5. Example for a mid-range phone: 5 near tiles + 20 middle + 55 far is about **42,000 triangles** in total.
6. Drop the underside, replace the grey texture with a terrain shader (grass, rock, dirt, cobbles, sand, snow blended by slope and height), then place the village, roads, water, trees and scatter on top.
7. Terrain destruction is stored and synced per tile.

**Technical note:** the file's triangle connections use meshopt compression. A decoder was started but unfinished. Options: finish the decoder, use Unity-side tools, or have the owner reduce the mesh in Blender (Decimate), or retopologize at the source. Ask the owner which they prefer.

**Second terrain concept (pending decision):** the owner also sent two grey angled renders, a top-down height map (with a black-padded 16-bit file whose data is only 8 levels, so it needs smoothing), and a color picture showing the intended result: a green basin with dirt patches, mound-like rocky hills, four lakes joined by rivers, a ring of cliffs with moss tops, snow-capped back mountains, waterfalls (one drops off the front edge), cloud rims and a small bridge. Ask the owner whether this replaces the Tripo island or is a different zone.

**Overall texture goal:** real surface detail and roughness (cracked layered rock, pebbled dirt, grass blades, moss), not flat color. Photo-scanned textures cannot be generated in code. The owner can supply free CC0 texture sets (grass, rock, dirt, cobblestone) for the closest match.

---

## 10. PHONE PERFORMANCE PLAN (the triangle reduction plan)

**Weakest target phone:** a **2 GB RAM Android** at the lowest quality. 3 to 4 GB is the comfortable target. Quality tier is picked at launch from the phone's RAM and graphics chip, can be changed in Settings, and is lowered automatically if the frame rate drops.

**What "too many triangles" means:** the total triangles drawn each frame (30 to 60 times a second), including whole objects that are partly visible and triangles hidden behind others, plus memory for meshes and textures. A phone screen is about 1 million pixels, so triangles beyond that are wasted.

**Starting budgets (planning guesses; test on real phones):**

| | Low (2 GB) | Mid (3 to 4 GB) | High (6 GB+) |
|---|---|---|---|
| All characters in view, total triangles | about 100,000 | about 200,000 | about 400,000 |
| Full-detail players near you | about 6 | about 12 | about 24 |
| Your own character | about 8,000 | about 10,000 | about 12,000 |
| Frame rate target | 30 | 30 to 45 | 60 |
| Shadows | none (blob under feet) | soft blob | real-time |
| Textures | half resolution | full | full |
| Effects (glow, rays, particles) | reduced | normal | full |
| Terrain triangles in view | about 20,000 | about 40,000 | about 80,000 |
| Trees in view | about 40 | about 100 | about 250 |
| Draw distance | short, with fog | medium | long |
| Water | flat tint | simple waves | reflections |
| IK | own character and nearby players | medium radius | full area of interest |

**Character budget (one full-detail character, about 10,000 triangles):**

| Piece | Triangles |
|---|---|
| Head and face | 1,800 |
| Body (neck down, hands included) | 3,600 |
| Hair | 900 |
| Clothes, all slots | 3,700 |

Clothes by slot: top, jacket or armor chest 1,300; pants 800; boots (pair) 600; gloves or arm guards 300; helmet or headgear 400; cape or back piece 300. Most outfits do not fill every slot. Skin hidden by clothes is not drawn.

**The owner brings only one full-detail version of each character. Claude makes every lower version** with an editor tool that simplifies the mesh, carries over the skin weights, UVs and colors, protects the neck seam, face and hands, and swaps versions by distance with a buffer to prevent flicker:
- **Mid distance:** about 4,500 triangles (head 900, body 1,700, hair 400, clothes 1,500)
- **Far:** about 1,800 (head 350, body 700, hair 150, clothes 600)
- **Very far:** a 300 to 600 triangle silhouette, or just a nameplate

**Buildings and props (nearest detail; the next level is about 35%, then 10%):**
Large buildings (Chief's Hall, Guild, Inn, Temple, Stable, Barracks) 3,500 to 5,000 exterior and 2,500 to 4,000 interior; medium buildings (Weapon, Armor, General, Potion, Workshop, Warehouse, Farmhouse and Mill) 2,500 to 3,500 and 1,500 to 3,000; small (houses, Notice Board) 1,000 to 2,000 and 1,200 to 2,000; open-air 2,000 to 4,000; Gate 3,000 to 4,000. Small props 60 to 1,000. Trees 150 to 400, rocks 80 to 300, repeated items drawn as shared copies.

**Area of interest (AOI):** the server sends each phone only the players, monsters and terrain changes near that player. Three rings around the player: near (full model), middle (lighter copy), far (very simple or nameplate). A hard cap on full-detail players (nearest N), with friends, party members and the current target always full detail. Players must move a little past a ring edge before switching (hysteresis). Maximum players drawn is roughly the character triangle budget divided by triangles per player. The AOI also cuts network traffic, which matters most in a 100-player Battle Royale.

**Other performance measures:** tiles and culling for terrain and buildings; shared materials and texture atlases to cut draw calls (draw calls hurt phones more than triangles); reduced animation updates at distance; cheap shaders; interiors loaded only when near; capped destruction rebuilds spread over frames; capped numbers of NPCs and monsters active near the player. Add an on-screen debug counter for triangles, draw calls and frame time so the numbers can be tuned on real phones.

---

## 11. MULTIPLAYER AND ANTI-CHEAT

- **Server decides, client asks.** Currency, inventory, Luck Royale rolls, combat, XP, loot and movement checks all run on the server. Never trust the phone.
- **Sentinel** (the owner's anti-cheat) is written in **HTML, JavaScript and CSS** and monitors and flags. Server scripts enforce. The client reports events to Sentinel through a thin interface. Ask the owner for Sentinel's endpoints or docs to match them.
- Session tokens, request signing and rate limiting on every call. Sentinel responses: warn, kick, ban or shadow-restrict.
- **Hosting, accounts, payments, chat moderation and store review rules** need the owner's accounts and money. Claude can write the scripts but cannot host or test them.

---

## 12. THE OWNER'S NINE BACKDROPS (in the order and mapping agreed)

1 Entrance: twin floating castles under a blue sky · 2 Loading: castle on a lake with a bridge · 3 Name entry: village at golden hour (portrait, will be cropped) · 4 Character customisation: purple and orange lightning sky · 5 Lobby: floating castle city with a sun flare · 6 Profile: dark blue circuit city · 7 Vault: dark blue-purple cosmic canyon · 8 Events: wide fantasy panorama with a ship (very wide, can drift slowly) · 9 Luck Royale: dark red volcanic dusk (portrait, will be cropped).

All are below 2560 by 1440, so they will look slightly soft on large phones. Claude resizes, compresses and wires each to its screen, adds a dark gradient behind text on bright images, and uses a plain placeholder if one is missing. The other screens have no clean backdrop, so they use the nearest of the nine or a gradient unless the owner sends more.

---

## 13. LIMITS AND HONESTY NOTES

- Claude cannot make art, servers, accounts or payments, cannot judge game feel, and cannot test on a real phone.
- Everything code-built (hair, clothes, icons, props, effects) will be stylized and simpler than the painted pictures.
- Tripo free-tier exports may not be cleared for a released game. Check the license before shipping.
- Some reference pictures carry watermarks or credits to other companies. Use them as references only and make original work.
- Small text in the reference images is hard to read. Claude must mark what it cannot read and ask.
- Claude's estimates (triangle budgets, tier numbers) are planning figures and must be checked on real devices.

---

## 14. REFERENCE PICTURES TO RE-UPLOAD IN THE NEW CHAT

1. The **24-screen design collage** (key art plus 24 numbered screens)
2. The **lobby, character creation and in-game mockups**, and the **goblin sheet**
3. The **nine backdrops** (also copy them into the Unity project)
4. The **Building Showcase**, **Infrastructure Showcase** and **Layout Blueprint**
5. The **HUD prototype** (with its control list) and the **Free Fire screenshots**
6. The **monster pictures** (slimes, wolves, boar, bat, spiders, beetles, scorpion, wasp, toad, mole, troll)
7. The **Tripo screenshots** (head, male and female bodies), and the terrain pictures (grey renders, height map, color picture)
8. The new **layout that fits the terrain**, when ready

Also copy these files into the Unity project: the male and female body GLBs, the two heads, the monster base meshes and parts, and the terrain mesh.

---

## 15. OPEN QUESTIONS (ask the owner)

1. Does the Tripo island stay as the terrain, or does the lakes-and-waterfalls layout replace it? Where is the new layout?
2. Which head is male and which is female?
3. Where do the Mining Site, Quarry and Lumberjack Site go?
4. How many Residential Houses, and is the river or lake crossed by a bridge?
5. Which monster bases come first, and will the owner bring the extra parts (wings, fins, horns, tails, armor)?
6. Does the owner want to bring base garments, or start with code-made clothes only?
7. Which route for the terrain reduction: finish the mesh decoder, Blender Decimate, or retopology at the source?
8. Sentinel's endpoints or docs, and what backend language the server scripts should target.
9. Should the Shrine tie into the God Inheritance system?
10. Confirm the gameplay roles inferred for gathering sites (Lumberjack gives wood, Mine gives ore, Quarry gives stone, Dock gives fish).

---

## 16. FIRST STEPS IN THE NEW CHAT

1. Read this file and both spec files. Reply with a short summary and any questions from Section 15.
2. Confirm the Unity connection by reading (project data, scene, console). Report the Unity version, render pipeline and any errors.
3. Ask the owner to back up the project and copy the assets in.
4. Wait for the owner to say **start**, then begin batch 1: foundation, then screens 1 to 6, checking the console and comparing screenshots against the references after each step.
