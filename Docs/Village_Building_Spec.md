# Beginner Village — Building Specification
## BINDING RULE (from the project owner)
**Every building and every piece of infrastructure is built exactly as it appears in the picture. If the picture shows stone, it is stone. If it shows timber, it is timber. This applies to the rest of the infrastructure too, with no exceptions and no substitutions.** Where the picture is too small or unclear to tell the material, I will not guess: I will mark it "unclear" and ask before building it.

Source: the "Beginner Village – Building Showcase" sheet (20 buildings, each with Front / Back / Left / Right exterior views and Interior Front / Back / Left / Right views).

How to read this file
- **Seen** = visible on the sheet. **Inferred** = I filled a gap from the building's name or the profession plan; please confirm or correct.
- The interior thumbnails are tiny and dark, so interior contents are mostly **inferred** from the building type and the visible colors.
- Triangle budgets are for the nearest detail level (LOD0). LOD1 is about 35% of that, and LOD2 about 10%.

## Rules that apply to every building
1. **Four exterior sides** are built and checked against the Front / Back / Left / Right strip.
2. **Interior = one walkable room** whose four walls match Interior Front / Back / Left / Right.
3. **Working door** that opens when the player walks up. The interior loads only when the player is near, and the wall facing the camera fades when inside.
4. **Solid walls** with correct door gaps, so no walking through buildings.
5. **Shared kit:** walls, roofs, windows, doors, chimneys, signs, lanterns and awnings are reused, so many buildings share materials and meshes.
6. **Destruction:** buildings take staged damage (cracks, missing chunks, collapse) and can be repaired. Safe zones can protect them.
7. Open-air items (market, training yard, farms, fountain, notice board) have no closed interior; the four "interior" views are built as covered or courtyard areas, or confirmed with you.

## Budgets
| Size | Exterior LOD0 | Interior |
|---|---|---|
| Large (Chief's Hall, Guild, Inn, Temple, Stable, Barracks) | 3,500–5,000 | 2,500–4,000 |
| Medium (Weapon, Armor, General, Potion, Workshop, Warehouse, Farmhouse/Mill) | 2,500–3,500 | 1,500–3,000 |
| Small (Residential Houses, Notice Board) | 1,000–2,000 | 1,200–2,000 |
| Open-air (Training, Market, Farms, Fountain/Well) | 2,000–4,000 | covered area 1,000–2,000 |
| Gate | 3,000–4,000 | gatehouse 1,500 |

## The 20 buildings

### Row 1
**1. Village Chief's Hall** (Large)
- Seen: grand manor, blue roofs, cream stone walls, gables, small towers, ornate double door. Interiors: large hall, arched windows, cool blue glow.
- Inferred: elder NPC at a long desk, banners, quest-giver role, chief's chair on the back wall.

**2. Adventurer's Guild** (Large)
- Seen: blue roof, red banners, central tower, signs. Interiors: purple-blue lighting.
- Inferred: quest counter, notice board wall, party sign-up desk, guildmaster NPC.

**3. Weapon Shop (Blacksmith)** (Medium)
- Seen: dark timber and stone, chimneys with smoke. First interior: glowing orange forge.
- Inferred: forge wall, anvil, weapon racks, smith NPC, weapon crafting station.

**4. Armor Shop** (Medium)
- Seen: blue and cream, shield emblems, banners. Interiors: blue-lit, armor displays.
- Inferred: armor stands, shield wall, armorer NPC, armor crafting station.

### Row 2
**5. General Store** (Medium)
- Seen: purple, blue and teal roofs, striped awnings, colorful signs. Interiors: warm shelves, cyan accents.
- Inferred: shelves of goods, counter, merchant NPC, gold-priced shop.

**6. Potion / Alchemy Shop** (Medium)
- Seen: purple roofs, glowing violet and pink windows and orbs. Interiors: violet glow.
- Inferred: cauldron, shelves of bottles, alchemist NPC, potion brewing station.

**7. Inn / Tavern** (Large)
- Seen: large red-roofed half-timber building, warm lit windows. Interiors: wooden rooms, tables, hearth.
- Inferred: bar counter, hearth, rest beds, innkeeper NPC, cook NPC.

**8. Training Grounds** (Open-air)
- Seen: dirt yard, small wooden pavilions, posts.
- Inferred: practice dummies, weapon racks, trainer NPC, covered pavilion as the "interior".

### Row 3
**9. Stable** (Large)
- Seen: big barn, blue roof, horses visible. Interiors: stalls, beams.
- Inferred: stalls, hay, stable master NPC, mount station.

**10. Residential Houses** (Small, several variants)
- Seen: red-roofed cottages, cream walls, among trees. Interiors: cozy rooms.
- Inferred: bed, table, hearth, villager NPCs; 3 to 4 layout variants.

**11. Storage / Warehouse** (Medium)
- Seen: wooden storehouses, brown and blue roofs. Interiors: crates and shelves.
- Inferred: crates, barrels, banker or storage NPC.

**12. Market Square** (Open-air)
- Seen: plaza with striped stalls (red, yellow, blue).
- Inferred: stall counters with goods, vendor NPCs, covered stall as the "interior".

### Row 4
**13. Guard Barracks / Watchtower** (Large)
- Seen: tall towers with red and blue cone roofs, banners, low barracks. Interiors: bunks and tables.
- Inferred: bunks, weapon racks, guard captain NPC, climbable tower.

**14. Shrine / Elemental Temple** (Large)
- Seen: white-and-blue stone, spires, glowing cyan crystals. Interiors: blue glow around a crystal.
- Inferred: central crystal, altar, priest NPC; ties to the Inheritance system (to confirm).

**15. Crafting Workshop** (Medium)
- Seen: red roofs, tall smoking chimney. Interiors: workbenches.
- Inferred: workbenches, tool walls, carpenter or crafter NPC, crafting station.

**16. Farmhouse / Mill** (Medium)
- Seen: cottages and a windmill on the right. Interiors: sacks, tables.
- Inferred: windmill with turning blades, grain sacks, farmer NPC.

### Row 5
**17. Farms & Gardens** (Open-air)
- Seen: fenced plots, tilled rows, green crops.
- Inferred: harvestable crops, scarecrow, farmer NPC. The sheet's interiors look like generic rooms; confirm what you want.

**18. Fountain / Well** (Open-air)
- Seen: tiered blue fountain, stone well, in a plaza.
- Inferred: animated water, a drinking and healing spot, plaza seating. The sheet's interiors look like courtyards.

**19. Notice Board** (Small)
- Seen: wooden boards with small roofs and posted papers.
- Inferred: quest postings, interact to read quests.

**20. Main Village Gate** (Gate)
- Seen: stone gate with towers, arches, doors and flags. Interiors: gatehouse passages.
- Inferred: opening gate doors, guards on watch, gatehouse rooms in the towers; connects to the village wall (timber palisade with stone gate towers).

## Not on the sheet (needs a decision)
- **Village layout:** now supplied. See Part 5 (Layout Blueprint).
- **Exact back and side views:** the exterior strips look like different compositions, not strict turnarounds. I'll make the sides consistent with each building's front.
- **Interiors of open-air items** (see rule 7).

## Limits
- They will be stylized, code-built versions of these pictures, simpler than the painted art.
- Small signs, banner symbols and props can't be read at this image size, so they are approximations.

---

# Part 2 — Infrastructure Specification
Source: the "Beginner Village – Infrastructure Showcase (Terrain, Roads, Decorations & World Elements)" sheet. 34 items in 5 rows. Same reading rules as Part 1 (Seen / Inferred, LOD0 budgets).

## Budgets (triangles, LOD0)
| Group | Per item |
|---|---|
| Large structures (Gate, Watchtower, Windmill, Mining Site, Quarry, Lumberjack Site, Greenhouse, Warehouse Yard, Fishing Dock) | 1,200–4,000 |
| Medium structures (Stone Bridge, Wooden Bridge, Staircase, Fountain, Statue, Well, Pergola, Animal Pen, Campfire Site) | 400–2,500 |
| Small props (Notice Board, Market Stall, Lantern, Fence segment, Banner, Barrel/Crate, Cart, Signpost) | 60–1,000 |
| Nature (Trees, Rocks, Bushes, Flowers) | 80–400, instanced and shared |
| Roads and paths | terrain texture plus a thin spline mesh |

## Row 1 — Defence and crossings
1. **Main Entrance Gate** — Seen: stone arch between two towers, blue and red banners, cobbled road through the arch. Same structure as building #20 (build once).
2. **Village Wall** — Seen: grey stone wall with blue banners bearing a gold emblem. Inferred: modular segments, corners and towers; the village boundary. (The earlier timber-palisade plan is cancelled. The picture shows **stone**, so it is stone.)
3. **Watchtower** — Seen: wooden upper floor on a stone base, banners, lantern lit. Inferred: climbable, guard post.
4. **Stone Bridge** — Seen: arched stone bridge with lamps over a river.
5. **Wooden Bridge** — Seen: plank bridge with rails over a river.
6. **Stone Staircase** — Seen: wide stone steps up a rocky slope with banners. Inferred: connects terrain levels.

## Row 2 — Roads and plaza elements
7. **Main Road** — Seen: wide cobbled road with wooden fences and flowers along both sides.
8. **Pathway** — Seen: narrower cobbled path through grass.
9. **Market Stalls** — Seen: stalls with striped awnings (red, blue, yellow), tables of goods. Same as building #12.
10. **Fountain** — Seen: round stone basin with a glowing blue crystal and water jets. Same as building #18.
11. **Statue** — Seen: stone winged figure on a pedestal.
12. **Notice Board** — Seen: wooden frame with papers and a blue banner. Same as building #19.

## Row 3 — Farming and trade
13. **Farms & Crop Fields** — Seen: rows of crops, fenced. Same as building #17.
14. **Animal Pen** — Seen: fenced pen with cows and a small shelter.
15. **Well** — Seen: stone well with a wooden roof.
16. **Windmill** — Seen: tower windmill with sails, in a flower meadow. Same as the Mill in building #16.
17. **Fishing Dock** — Seen: wooden pier and moored boat on a lake, lamps on posts.
18. **Warehouse Yard** — Seen: open timber roof over stacked crates and sacks. Related to building #11.

## Row 4 — Gathering sites
19. **Gardens** — Seen: wooden pergola with flower beds.
20. **Greenhouse** — Seen: glass-and-frame greenhouse, warm lit inside.
21. **Lumberjack Site** — Seen: crane or hoist, logs, saw area.
22. **Mining Site** — Seen: arched mine tunnel in a rock wall with a rail track.
23. **Quarry** — Seen: scaffolding and a crane at a rocky cut.
24. **Campfire Site** — Seen: campfire with tents.

## Row 5 — Decoration and world elements
25. **Trees** (pines) · 26. **Rocks** · 27. **Flowers** (purple, white, daisy-like) · 28. **Bushes** · 29. **Lantern Posts** (lit) · 30. **Fences** (wooden) · 31. **Banners & Flags** (blue and red, gold emblem) · 32. **Barrels & Crates** · 33. **Carts & Wagons** · 34. **Signposts** (direction arrows)

## How the items work in the game (inferred, to confirm)
- **Gathering:** Lumberjack Site (wood), Mining Site (ore), Quarry (stone), Fishing Dock (fish), Farms, Gardens and Greenhouse (crops and herbs). These feed the crafting and gold shops.
- **Animals:** the Animal Pen holds farm animals (cows), with simple wander animation.
- **Rest:** Campfire Site and Inn restore health.
- **Movement:** bridges and the staircase are walkable and solid; the mine tunnel is an entrance.
- **Light and motion:** lantern lights at dusk, banners sway, windmill sails turn, fountain water flows, fire has VFX.
- **Destruction:** props can break, and fences, crates and carts can be repaired.

---

# Part 3 — Combining both sheets into one village

## Overlaps (build once, count once)
| Item | Building sheet | Infrastructure sheet |
|---|---|---|
| Main Village Gate | #20 | Main Entrance Gate |
| Market Square | #12 | Market Stalls |
| Fountain / Well | #18 | Fountain and Well |
| Notice Board | #19 | Notice Board |
| Farms & Gardens | #17 | Farms & Crop Fields, Gardens |
| Farmhouse / Mill | #16 | Windmill |

That gives **20 buildings plus 34 infrastructure items, minus 7 duplicates, so about 47 unique pieces**.

## Layout
The layout is now fixed by the owner's **Layout Blueprint** (see Part 5). My earlier layout plan is cancelled.

## Terrain needed
A river with a lake, higher ground in the north, rocky hills at the edge for the mine and quarry, and a flat center. The Tripo terrain or the color-picture layout would both work, with the river and lakes carved in.

## Performance
Everything near the player loads; distant buildings use lower detail and distant props are hidden, with the same area-of-interest limits as characters. Repeated items (trees, fences, crates, lanterns, flowers) are drawn as shared copies.

## Open decisions
1. ~~Stone wall or timber palisade~~ — decided: **follow the picture (stone wall)**.
2. Do you want to supply a village map, or use my layout?
3. Confirm the gameplay roles I inferred.

---

# Part 4 — Materials as seen in the infrastructure picture (binding)
"Unclear" means I cannot tell from the picture at this size and will ask before building.

| Item | Material in the picture |
|---|---|
| Main Entrance Gate | Stone arch and towers; cloth banners; doors look timber (unclear) |
| Village Wall | Stone blocks; cloth banners |
| Watchtower | Stone base, timber upper floor; roof material unclear |
| Stone Bridge | Stone, with lamp posts |
| Wooden Bridge | Timber planks and rails |
| Stone Staircase | Stone steps on a rock slope |
| Main Road / Pathway | Cobblestone |
| Market Stalls | Timber frames with striped cloth awnings |
| Fountain | Stone basin with a glowing blue crystal and water |
| Statue | Stone-colored winged figure on a stone pedestal (stone vs. other unclear) |
| Notice Board | Timber frame with paper notices and a cloth banner |
| Farms & Crop Fields | Soil rows, plants, timber fences |
| Animal Pen | Timber fence and small timber shelter |
| Well | Stone body with a timber roof |
| Windmill | Tower body material unclear; timber-looking sails |
| Fishing Dock | Timber pier and posts, with lamps |
| Warehouse Yard | Timber posts and roof over timber crates and sacks |
| Gardens (pergola) | Timber pergola, flower beds |
| Greenhouse | Glass panes in a frame (frame material unclear) |
| Lumberjack Site | Timber crane or hoist, logs |
| Mining Site | Rock face with a timber-braced tunnel arch and rail track |
| Quarry | Timber scaffolding and crane at a rock cut |
| Campfire Site | Fire, canvas tents (ring material unclear) |
| Trees | Pines |
| Fences | Timber |
| Lantern Posts | Dark post with glass lantern (post material unclear) |
| Banners & Flags | Cloth on timber poles |
| Barrels & Crates | Timber |
| Carts & Wagons | Timber with iron-looking wheel rims |
| Signposts | Timber with arrow boards |

---

# Part 5 — Layout Blueprint (binding)
Source: "Beginner Village – Layout Blueprint (All Buildings & Infrastructure Placement Plan)". The image is an angled painting, so positions below are approximate relative placements (north at the top, as the compass shows). I will place each piece where the blueprint shows it, and I will not move pieces to other zones.

## Schematic (north at the top)
```
                       [ MAIN ENTRANCE GATE (north wall) ]
  Farms &      Farmhouse /    Village Chief's    Adventurer's    Training
  Gardens      Mill           Hall               Guild           Grounds

  Stable   Armor   Weapon    ( MARKET SQUARE     Potion /   General   Inn /
           Shop    Shop        + FOUNTAIN )      Alchemy    Store     Tavern

  Residential     Storage /   [ MAIN ROAD ]   Crafting    Shrine /   Guard Barracks /
  Houses (x4)     Warehouse                   Workshop    Temple     Watchtower

                       [ MAIN EXIT GATE (south wall) ]
  Fishing Dock                 Notice Board   Well        Windmill + farm plot
  (by the lake, outside SW)    (south-east, outside the wall)
```

## Placement table (position inside the walled village)
| Piece | Where in the blueprint |
|---|---|
| Main Entrance Gate | Center of the north wall, with large flanking towers and banners |
| Village Chief's Hall | Center, just south of the entrance gate |
| Adventurer's Guild | North row, east of the Chief's Hall |
| Farmhouse / Mill | North row, west of the Chief's Hall (with a silo or tank beside it) |
| Farms & Gardens | North-west corner, fenced plots |
| Training Grounds | North-east corner, fenced yard with banners |
| Market Square + Fountain | Exact center of the village, circular plaza with the fountain |
| Stable | West side, middle row (fenced yard with horses) |
| Armor Shop | Middle row, west of the plaza (outer) |
| Weapon Shop | Middle row, west of the plaza (inner, next to the plaza) |
| Potion / Alchemy Shop | Middle row, east of the plaza (inner) |
| General Store | Middle row, east of the plaza (middle, striped awning) |
| Inn / Tavern | Middle row, far east |
| Residential Houses | South-west block, about four cottages |
| Storage / Warehouse | South row, west of the main road |
| Crafting Workshop | South row, east of the main road |
| Shrine / Temple | South row, east of the Crafting Workshop |
| Guard Barracks / Watchtower | South-east block, far east |
| Main Exit Gate | Center of the south wall, four towers |
| Notice Board | South-east, outside the south wall |
| Well | South-east, next to the Notice Board, outside the wall |
| Windmill (with farm plot) | South-east corner, outside the wall |
| Fishing Dock | South-west, on the lake outside the wall, with a boat and a small island |

## Roads and paths (legend: Main Road, Side Path)
- **Main Road:** from the Main Entrance Gate down both sides of the Chief's Hall to the circular Market Square, then straight south between the Storage / Warehouse and the Crafting Workshop to the Main Exit Gate.
- **Side Paths:** a grid of paths running east to west across the village on two levels (between the north row and the plaza row, and between the plaza row and the south row), plus short paths to each building door.
- The plaza connects to the roads by short flights of steps.

## Walls, towers and water
- **Stone wall** (per the picture) forms a closed rectangle around the village, with corner towers and extra watchtowers spaced along the sides, the gate towers at both gates, and short stone staircases through the wall near the north-west, north-east and south-west.
- **Water** (legend: blue): a river runs down the west side and into a lake in the south-west with the Fishing Dock, and streams and waterfalls sit beyond the north wall.
- **Decoration** (legend): trees, bushes, flowers, lantern posts, fences, banners, barrels and crates, carts, and signposts, placed along roads, around buildings and along the wall.

## Layout zones (legend, bottom left)
| Zone | Contents |
|---|---|
| Central Zone | Market and core |
| Administrative | Chief, Guild, Temple |
| Commercial | Shops and services |
| Residential | Houses and living |
| Agricultural | Farms and food |
| Defense | Guard and watchtowers |
| Recreational | Training, fishing, events |

## What changes from my earlier assumptions
- The **entrance gate is in the north** and the **exit gate in the south**.
- The **Chief's Hall is just inside the north gate**, not at the far north.
- The **Shrine / Temple is in the south-east block**, not the north.
- The **Training Grounds are in the north-east corner**, not by the gate.
- The **Windmill is a separate piece in the south-east corner**, apart from the Farmhouse / Mill.
- The **Notice Board and Well are outside the south wall**, not in the plaza.
- The **Fishing Dock is outside the south-west wall**, on the lake.

## Unclear in the blueprint (will ask before building)
- Exact number and exact placement of Residential Houses (about four are visible).
- Whether the lake and river inside the south-west corner are crossed by a bridge.
- Exact wall height and tower spacing.
- Where the mine, quarry and lumberjack sites sit (they aren't on this blueprint, so they would go on the outskirts unless you say otherwise).

## Timing
The blueprint is for the final build. I will assemble all buildings and infrastructure in these places then.
