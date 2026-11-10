You're right. I've been giving you scaffolding and caveats instead of the actual deliverable. Here is the complete compendium — written to be the final artifact, not a draft. It covers every creature type, every animation category including advanced combat/traversal actions (bite, pounce, swing, claw, tail-whip, grapple, and more), and a formal extension system so any new animation request maps cleanly. It is self-contained: paste it into Claude and it has everything it needs.

---

🐾 THE COMPLETE CREATURE ANIMATION COMPENDIUM (v3 — Final)

A universal, rig-aware, pose-grounded system for generating fluent creature animation in Unity

---

§0. HOW CLAUDE USES THIS DOCUMENT

This document is the single source of truth. Follow it in order:

1. Read §1 (prime directive and failure policy).
2. Discover the rig using §2. If discovery fails, return the structured error — never guess.
3. Identify the creature in §3 (role maps + default prefixes + expected counts).
4. Validate or synthesize a base pose using §4.
5. Identify the animation in §6 (state recipes) or §7 (advanced action recipes). If the request is not listed, follow §8 (extension protocol).
6. Compose the clip from the helpers in §5, passing the BoneMap and ContactMode.
7. Set tangents per §5.5.
8. Enforce §9 (global rules).
9. Output the generator function, the ClipManifest (§10), and the validation report.

Every rule is mandatory. Every failure is loud. Every success is machine-verifiable.

---

§1. PRIME DIRECTIVE AND FAILURE POLICY

Prime directive: Produce fluent, layered, rig-correct animation clips by composing procedural curve generators over discovered bone roles and validated base poses.

Failure policy: If any hard rule cannot be satisfied, return a structured error of the form:

```
AnimationError {
  code: "MISSING_ROOT" | "BIND_POSE_DETECTED" | "SPINE_TOO_SHORT" |
        "LEG_COUNT_MISMATCH" | "PHASE_SUM_INVALID" | "POSE_UNRESOLVABLE" |
        "CONTACT_CLAIM_WITHOUT_IK" | "UNKNOWN_CREATURE" | "AMBIGUOUS_ROLE"
  message: human-readable explanation
  fix: concrete next step for the user
}
```

A structured error is always preferable to a silent bad clip.

---

§2. RIG DISCOVERY PROTOCOL

§2.1 The BoneMap contract

```csharp
[Serializable] public struct BoneMap {
    public Transform root;
    public Transform[] spine;       // ordered root → tip
    public Transform head;
    public Transform[] support;     // legs / ground-contact limbs
    public Transform[] effector;    // arms, mandibles, jaws, weapons
    public Transform[] appendage;   // wings, fins
    public Transform tail;          // terminal chain root
    public Transform[] tailChain;   // ordered
    public Transform[] ears;
    public Transform[] eyes;
    public Transform[] extras;      // claws, fingers, antennae
    public Transform[] junction;    // hybrid blend bones
}
```

§2.2 Discovery algorithm

```
DiscoverBones(GameObject root, PrefixTable table, CreatureType type):
  1. Collect all transforms under root.
  2. For each transform, test name against each role's prefix list (case-insensitive).
  3. Assign matches to role buckets.
  4. Sort spine and tailChain by hierarchy depth (ascending from root).
  5. For each required role in type's map (§3):
       if bucket empty → record MISSING_ROLE(severity=hard)
       if bucket count != expected → record COUNT_MISMATCH(expected, found)
  6. For each optional role: if empty, record MISSING_OPTIONAL (soft, non-fatal).
  7. Return (BoneMap, DiscoveryReport).
```

§2.3 The PrefixTable

A user-overridable ScriptableObject asset. Defaults are provided per creature type in §3. If the user's rig deviates, they supply a custom table — Claude must never hardcode names beyond these defaults.

§2.4 Validation gate

Before any clip is generated:

```
if DiscoveryReport.hasHardErrors:
    return AnimationError("DISCOVERY_FAILED", report, "Supply a custom PrefixTable or rename bones.")
```

---

§3. CREATURE ROLE MAPS (complete bestiary)

Each entry lists required (R), optional (O), expected count, default prefixes, and gait reference (§5.3).

§3.1 Biped (human, humanoid, fantasy 2-leg)

```
root:       R  ["Hips","Pelvis","Root"]                  count=1
spine:      R  ["Spine","Chest","Neck"]                  count=3, ordered
head:       R  ["Head","Skull"]                          count=1
support:    R  ["Leg_L","Leg_R","Thigh","Shin","Foot"]   count=2
effector:   O  ["Arm_L","Arm_R","Hand","Clavicle"]       count=2
appendage:  O  []                                         count=0
tail:       O  ["Tail"]                                   count=0-1
ears:       O  ["Ear_L","Ear_R"]                          count=0-2
eyes:       O  ["Eye_L","Eye_R"]                          count=0-2
extras:     O  ["Finger","Toe","Claw"]                    count=0-20
Gait: alternate (phase 0.0 / 0.5). Body bob @ 2×.
```

§3.2 Quadruped (wolf, horse, lion, bear, fantasy 4-leg)

```
root:       R  ["Hips","Pelvis","Root"]                  count=1
spine:      R  ["Spine","Chest","Neck"]                  count=3, ordered
head:       R  ["Head","Skull","Jaw"]                    count=1
support:    R  ["Leg_FL","Leg_FR","Leg_BL","Leg_BR",
                "Thigh","Shin","Paw","Hoof"]             count=4
effector:   O  ["Jaw","Mandible","Claw"]                 count=0-2
appendage:  O  []                                         count=0
tail:       O  ["Tail"]                                   count=0-1
tailChain:  O  ["Tail_01","Tail_02",...]                 count=0-N, ordered
ears:       O  ["Ear_L","Ear_R"]                          count=0-2
eyes:       O  ["Eye_L","Eye_R"]                          count=0-2
Gait: trot (FL+BR 0.0, FR+BL 0.5) or walk (0.0/0.25/0.5/0.75).
```

§3.3 Hexapod (insect, fantasy 6-leg)

```
root:       R  ["Thorax","Root"]                          count=1
spine:      R  ["Abdomen"]                                count=1-3, ordered
head:       R  ["Head"]                                   count=1
support:    R  ["Leg_1".."Leg_6"]                         count=6
effector:   O  ["Mandible","Fang"]                        count=0-2
Gait: tripod (1,3,5 @ 0.0; 2,4,6 @ 0.5).
```

§3.4 Octopod (spider, octopus, fantasy 8-leg)

```
root:       R  ["Cephalothorax","Root"]                   count=1
spine:      R  ["Abdomen"]                                count=1-3, ordered
head:       R  ["Head"]                                   count=1
support:    R  ["Leg_1".."Leg_8"]                         count=8
effector:   O  ["Fang","Pedipalp"]                        count=0-2
Gait: alternating tetrapod (odd 0.0, even 0.5, +0.05·i ripple).
```

§3.5 Flying (bird, bat, insect, dragon, fantasy flier)

```
root:       R  ["Body","Root"]                            count=1
spine:      R  ["Spine","Neck"]                           count=2-4, ordered
head:       R  ["Head","Beak"]                            count=1
support:    O  ["Leg","Talon","Foot"]                     count=0-2
effector:   O  ["Beak","Talon","Claw"]                    count=0-2
appendage:  R  ["Wing_L","Wing_R"]                        count=2
tail:       O  ["Tail"]                                   count=0-1
tailChain:  O  ["Tail_01",...]                            count=0-N, ordered
Gait: flap (L 0.0, R 0.5). Body bob @ 2× flap. Tail follow-through +0.15s.
```

§3.6 Aquatic (fish, shark, whale, fantasy swimmer)

```
root:       R  ["Body","Root"]                            count=1
spine:      R  ["Spine_01".."Spine_N"]                    count≥3, ordered (HARD min 3)
head:       R  ["Head","Skull"]                           count=1
support:    O  ["Fin_Pectoral_L","Fin_Pectoral_R"]        count=0-2
effector:   O  ["Jaw","Mouth"]                            count=0-1
appendage:  R  ["Fin_Dorsal","Fin_Caudal","Fin"]          count≥1
tail:       R  ["Tail","Caudal"]                          count=1
Gait: spine traveling wave (phaseStep 0.15–0.25/segment). Fins @ 0.0/0.5. Lateral sway.
```

§3.7 Serpentine (snake, eel, worm)

```
root:       R  ["Head","Body","Root"]                     count=1 (anchor)
spine:      R  ["Spine_01".."Spine_N"]                    count≥6, ordered (HARD min 6)
head:       R  ["Head","Skull","Jaw"]                     count=1
support:    —  (intentionally empty, not an error)
effector:   O  ["Jaw","Fang"]                             count=0-1
appendage:  O  ["Tail_Tip"]                               count=0-1
Gait: pure spine traveling wave. Amplitude peaks mid-body, grows at tail.
      Head leads, tail lags +0.1s. Order head→tail strictly.
```

§3.8 Hybrid (wyvern, merfolk, centaur, chimera)

```
Concatenate the base maps of the constituent types.
For each junction between base regions, define:
  junction: Transform(s) at the blend point
  blendWidth: number of bones over which motion blends
Motion from region A blends to region B across blendWidth bones.
Document every junction in the ClipManifest.
```

§3.9 Beast / amorphous (slime, elemental, construct)

```
root:       R  ["Root","Body"]                            count=1
spine:      O  ["Spine","Core"]                           count=0-3
head:       O  ["Head"]                                   count=0-1
support:    O  ["Leg","Foot"]                             count=0-N
extras:     O  ["Blob","Node","Segment"]                  count=0-N
Gait: pulse/squash cycle (root scale wobble), no limbs required.
```

---

§4. BASE POSE VALIDATION AND SYNTHESIS

§4.1 Validation

```
ValidateBasePose(BoneMap, CreatureType) → BasePoseReport:
  1. Read current localEulerAngles of all mapped bones.
  2. Compare against bind-pose signatures:
       Biped:       arms horizontal (±15° of 90° from vertical) → T_POSE
       Quadruped:   legs straight down + spine horizontal → BIND_POSE
       Serpentine:  spine perfectly straight → BIND_POSE
       Flying:      wings fully extended horizontally → BIND_POSE
       Aquatic:     spine straight + fins fully spread → BIND_POSE
  3. If BIND/T detected → HARD ERROR:
       "BIND_POSE_DETECTED: Set an idle base pose, or call
        GenerateNeutralBasePose(type) (flagged synthetic)."
  4. If plausible base pose → source="scene".
```

§4.2 GenerateNeutralBasePose(type)

Returns a canonical, type-appropriate standing/coiled pose. Always flagged synthetic in the manifest. When used, all motion amplitudes reduce by 30% (§5.4) to compensate for pose uncertainty.

---

§5. HELPER LIBRARY (complete generator vocabulary)

§5.1 Primary motion helpers

Helper Signature Purpose
MakeLimbCycle (BoneMap, role, dur, amp, phase, ease, contactMode) one limb's cyclic motion
MakeSpineWave (BoneMap, chain, dur, ampProfile, phaseStep, tailLag) traveling wave along chain
MakeBodyLayer (BoneMap, freqMult, bobAmp, swayAmp, leanDeg, contactMode) mandatory body dynamics
MakeFollowThrough (BoneMap, role, lagSec, ampMult) delayed copy for secondary lag
MakeSecondary (BoneMap, ears, eyes, tail, lagSec) ear flicks, blinks, tail sway

§5.2 State-shape helpers

Helper Signature Purpose
MakeOneShot (BoneMap, phases[], contactMode) phased timeline (anticipation/action/recover)
MakeHoldPose (BoneMap, pose[], pulseFreq?) static or pulsing held pose
MakeIrregularLoop (BoneMap, base, noiseAmp, freq, seed) non-rhythmic looping (seeded)
MakeImpact (BoneMap, intensity, direction) sharp shake + recoil overlay

§5.3 Gait phase tables

Legs Gait Phases
2 alternate 0.0, 0.5
4 trot FL 0.0, BR 0.0, FR 0.5, BL 0.5
4 walk 0.0, 0.25, 0.5, 0.75
4 gallop FL 0.0, FR 0.1, BL 0.5, BR 0.6
6 tripod 1,3,5 @ 0.0; 2,4,6 @ 0.5
8 alternating tetrapod odd 0.0, even 0.5, +0.05·i
wings flap L 0.0, R 0.5
fins paddle L 0.0, R 0.5

§5.4 Speed tiers (multipliers on any locomotion)

Tier dur amp bob lean lag
Walk/Cruise ×1.0 ×1.0 ×1.0 0° base
Run/Fast ×0.6 ×1.3 ×1.4 +8° +0.05s
Sprint/Dash ×0.45 ×1.5 ×1.6 +12° +0.08s
Synthetic-pose compensation — ×0.7 ×0.7 — —

§5.5 Tangent policy

· Auto/ClampedAuto for all living motion.
· Flat/zero for: death settle, stun slack, sleep, limp limbs.
· Fast ease-out for: attack strike, launch phase, impact.
· Ease-in for: recovery phases, landing absorb.
· Manifest records the tangent mode per curve.

§5.6 Contact mode

```csharp
enum ContactMode { FullIK, NoIK, Airborne, Substrate }
```

· NoIK: reduce vertical amplitude ×0.5; set contactless=true.
· Airborne: no ground contact expected; contactless=true.
· Substrate: burrow/climb; contactless=true relative to ground.

---

§6. BASIC STATE RECIPES (complete table)

State Composition Required roles Contact default
Idle BodyLayer(low) + Secondary + occasional IrregularLoop root, head FullIK
Walk LimbCycle×N(phase table) + Body + FollowThrough root, support FullIK
Run Walk × speed tier same FullIK
Sprint Walk × speed tier same FullIK
Crawl LimbCycle(low, slow) + Body(reduced bob) root, support FullIK
Swim SpineWave + LimbCycle(fins) + Body(lateral sway) root, spine, appendage NoIK
Fly LimbCycle(wings) + Body(bob@2×) + FollowThrough(tail) root, appendage Airborne
Glide HoldPose(wings held) + Body(pitch) root, appendage Airborne
Hover LimbCycle(wings fast, small) + Body(upright) root, appendage Airborne
Climb LimbCycle(alternating reach-pull) + Body(pressed) root, support Substrate
Burrow LimbCycle(claw alternation) + Body(low push) root, support Substrate
Jump OneShot[crouch/launch/air/land] root, support FullIK
Fall IrregularLoop(flail, seeded) + Body(tilt 15°) root Airborne
Land OneShot[compress/absorb/rise] root, support FullIK
Idle-Alert OneShot[head turn + orient] root, head FullIK
Idle-Sleep IrregularLoop(breathing very low) + Body(lowered) root, head FullIK
Idle-Eat LimbCycle(head-down) + jaw cycle + Body(settle) root, head, effector FullIK
Hurt-Light OneShot[recoil 20/30/50] + MakeImpact(0.3) root, head FullIK
Hurt-Heavy OneShot[recoil 20/30/50] + MakeImpact(0.7) root, head FullIK
Block HoldPose(raised limbs) + Body(brace bob ×0.5) root, support or effector FullIK
Dodge OneShot[lateral/rotational evade] + FollowThrough root FullIK
Death OneShot[stagger/collapse/settle] + Flat tangents root, head, support FullIK
Spawn OneShot[assemble/unfurl] → blend idle root FullIK
Stun IrregularLoop(wobble, seeded) + slack tangents root, head FullIK
Knockback OneShot[launch/arch/recover] + MakeImpact root FullIK
Cast HoldPose(ritual) + pulse + spine arch root, effector FullIK
Channel IrregularLoop(pulse) + HoldPose(ritual) root, effector FullIK
Taunt OneShot[exaggerated pose] + flourish root, head, appendage FullIK
Carry HoldPose(grip) + locomotion underneath ×0.6 root, support, effector FullIK
Turn OneShot[weight shift + rotate] + Body(lean into turn) root, support FullIK

---

§7. ADVANCED ACTION RECIPES (combat, traversal, interaction)

These are the actions you asked for explicitly — bite, pounce, swing, and the full extended set. Each is a one-shot with named key poses. Claude must hit these poses; the helper calls are listed per recipe.

§7.1 Pounce (quadruped, hexapod, octopod, serpentine)

```
Phases: coil 0–25% | launch 25–40% | air 40–75% | land 75–100%
Key poses:
  COIL:   hind legs compressed (knees ~60°), front paws tucked, spine curled,
          head lowered, tail coiled.
  LAUNCH: hind legs fully extended, front paws reaching forward,
          spine straightening, head raised, tail streaming.
  AIR:    legs spread wide, spine arched, head forward, tail trailing.
  LAND:   front paws contact first, spine compressing,
          hind legs follow, tail whips forward.
Helpers: MakeOneShot(phases) + BodyLayer(lean+18°, bob×1.5)
         + FollowThrough(tail, +0.12s) + Secondary(ears on impact)
Contact: FullIK (launch + land need ground); Airborne (mid-flight)
Root motion: bake forward along +Z if requested
```

§7.2 Bite (all creatures with jaw/effector)

```
Phases: wind-up 0–20% | open 20–35% | snap 35–45% | hold 45–60% | recover 60–100%
Key poses:
  WIND-UP: head pulls back, jaw closed, body braces.
  OPEN:    jaw wide (max extension), head thrusts forward.
  SNAP:    jaw closes fast (ease-out), head lunges, body follows.
  HOLD:    jaw closed, slight head shake (MakeImpact small).
  RECOVER: return to neutral.
Helpers: MakeOneShot + MakeImpact(at snap, 0.4)
         + FollowThrough(head, +0.05s) + BodyLayer(fwd lean)
Role gate: requires effector (jaw) OR head (for beak/mandible creatures)
```

§7.3 Swing (biped with arms, or tail-wielding creatures)

```
Phases: raise 0–25% | wind 25–40% | swing 40–55% | follow 55–75% | recover 75–100%
Key poses:
  RAISE:  arm/wing/tail lifts, body coils opposite direction.
  WIND:   maximum backswing, spine twists away from target.
  SWING:  fast forward arc (ease-out), spine twists toward target,
          hips rotate, opposite arm counterbalances.
  FOLLOW: arm continues past target, body over-rotates slightly.
  RECOVER: settle to neutral, spine untwists.
Helpers: MakeOneShot + BodyLayer(spine twist ±, hip counter)
         + FollowThrough(arm, +0.08s) + FollowThrough(tail if present)
```

§7.4 Claw / Slash (quadruped, biped, hexapod)

```
Phases: raise 0–20% | swipe 20–40% | follow 40–60% | recover 60–100%
Key poses:
  RAISE:   front limb(s) raised, claws extended, body leans back.
  SWIPE:   fast diagonal arc (ease-out), body lunges forward,
           spine twists into the swipe.
  FOLLOW:  limb crosses past centerline, body over-extends.
  RECOVER: limb returns, body settles.
Helpers: MakeOneShot + MakeImpact(at swipe, 0.3)
         + BodyLayer(fwd lean + twist)
```

§7.5 Tail Whip (serpentine, aquatic, creatures with tail)

```
Phases: coil 0–30% | release 30–50% | follow 50–80% | recover 80–100%
Key poses:
  COIL:   spine/tail curls to one side, maximum stored energy.
  RELEASE: rapid uncoil (ease-out), tail snaps through target zone.
  FOLLOW: tail overshoots, body counter-rotates.
  RECOVER: tail returns, spine settles.
Helpers: MakeOneShot + MakeSpineWave(low amp, counter-phase)
         + FollowThrough(tailChain, +0.1s)
```

§7.6 Grapple / Grab (biped, quadruped, hexapod)

```
Phases: reach 0–25% | contact 25–35% | pull 35–60% | hold 60–80% | release 80–100%
Key poses:
  REACH:   limb(s) extended toward target, body leans in.
  CONTACT: limb closes, body braces.
  PULL:    limb retracts, body pulls back, weight shifts to rear.
  HOLD:    limb locked, body tension (slight tremble).
  RELEASE: limb opens, body recoils slightly.
Helpers: MakeOneShot + HoldPose(during hold) + MakeImpact(at contact, 0.2)
```

§7.7 Charge / Rush (quadruped, biped, hexapod)

```
Phases: wind-up 0–20% | acceleration 20–50% | impact 50–60% | recover 60–100%
Key poses:
  WIND-UP:  body lowers, head forward, legs coiled.
  ACCEL:    full-speed locomotion cycle at Sprint tier, body horizontal.
  IMPACT:   body slams forward, head/effector leads, MakeImpact(0.8).
  RECOVER:  body decelerates, legs brace, return to idle.
Helpers: MakeOneShot + LimbCycle(Sprint tier) during accel
         + MakeImpact(at impact, 0.8)
Root motion: bake forward fast during accel
```

§7.8 Stomp / Slam (large creatures, quadruped, biped)

```
Phases: raise 0–30% | hold 30–40% | slam 40–55% | impact 55–65% | recover 65–100%
Key poses:
  RAISE:  limb(s) raised high, body leans back.
  HOLD:   brief pause at apex (anticipation).
  SLAM:   limb drops fast (ease-in), body compresses on impact.
  IMPACT: MakeImpact(0.9) — full-body shake, dust.
  RECOVER: body rises, limb returns.
```

§7.9 Roar / Shout (all creatures with head)

```
Phases: inhale 0–30% | roar 30–70% | sustain 70–85% | recover 85–100%
Key poses:
  INHALE:  head pulls back, chest expands, jaw closes.
  ROAR:    jaw wide, head thrusts forward, spine arches, body vibrates.
  SUSTAIN: hold roar with subtle MakeImpact tremble.
  RECOVER: jaw closes, head returns, body settles.
Helpers: MakeOneShot + HoldPose(during sustain) + MakeImpact(tremble)
```

§7.10 Roll / Evade (biped, quadruped, amorphous)

```
Phases: tuck 0–25% | roll 25–65% | rise 65–100%
Key poses:
  TUCK:   body curls, limbs pull in, head tucks.
  ROLL:   full rotation (root rotates 360°), limbs stay tucked.
  RISE:   body uncurls, limbs extend, return to idle.
Helpers: MakeOneShot + root rotation curve (360°)
```

§7.11 Dig / Burrow-Down (quadruped, hexapod, serpentine)

```
Phases: scratch 0–40% | dig 40–70% | descend 70–100%
Key poses:
  SCRATCH: alternating front-limb claw cycles.
  DIG:     body lowers, limbs push substrate back.
  DESCEND: root translates downward, body disappears.
Helpers: LimbCycle(alternating) + BodyLayer(lowered) + root Y translate
```

§7.12 Emerge / Surface (from burrow/water)

```
Phases: rise 0–50% | shake 50–75% | idle 75–100%
Key poses:
  RISE:   root translates upward, head leads, body uncurls.
  SHAKE:  MakeImpact(0.5) — full-body shake (water/dirt off).
  IDLE:   blend to Idle.
```

§7.13 Wing Buffet / Gust (flying)

```
Phases: raise 0–30% | flap-forward 30–55% | follow 55–75% | recover 75–100%
Key poses:
  RAISE:    wings raised high.
  FLAP:     wings snap forward/down fast (ease-out).
  FOLLOW:   wings overshoot, body pushed back.
  RECOVER:  wings return to neutral.
Helpers: MakeOneShot + MakeImpact(at flap, 0.4) + BodyLayer(push back)
```

§7.14 Constrict / Coil (serpentine, octopod)

```
Phases: approach 0–25% | coil 25–55% | squeeze 55–80% | release 80–100%
Key poses:
  APPROACH: body extends toward target.
  COIL:     spine wraps (progressive phase-offset curls).
  SQUEEZE:  sustained hold with subtle MakeImpact tremble.
  RELEASE:  spine uncoils rapidly.
Helpers: MakeSpineWave(progressive amp) + HoldPose(squeeze)
```

§7.15 Lunge / Strike (serpentine, aquatic, biped)

```
Phases: coil 0–30% | strike 30–50% | hold 50–65% | recover 65–100%
Key poses:
  COIL:   body pulls back, head retracts.
  STRIKE: head/body thrusts forward fast (ease-out), spine straightens.
  HOLD:   extended pose brief hold.
  RECOVER: return to neutral.
Helpers: MakeOneShot + MakeImpact(at strike, 0.5) + BodyLayer(fwd lean)
```

§7.16 Kick (biped, quadruped rear)

```
Phases: chamber 0–30% | extend 30–50% | impact 50–60% | retract 60–100%
Key poses:
  CHAMBER: knee raised, body leans back for balance.
  EXTEND:  leg snaps forward (ease-out), hip rotates.
  IMPACT:  MakeImpact(0.6) at extension.
  RETRACT: leg returns, body settles.
```

§7.17 Headbutt (quadruped, biped)

```
Phases: rear-back 0–30% | lunge 30–55% | impact 55–65% | recover 65–100%
Key poses:
  REAR-BACK: head pulls back, body coils.
  LUNGE:     head snaps forward, body follows.
  IMPACT:    MakeImpact(0.7).
```

§7.18 Spit / Projectile (serpentine, aquatic, fantasy)

```
Phases: charge 0–40% | release 40–55% | recover 55–100%
Key poses:
  CHARGE:  head rears back, jaw opens, body swells (chest expand).
  RELEASE: head snaps forward, jaw wide, MakeImpact(0.3) recoil.
  RECOVER: return to neutral.
```

§7.19 Mount / Ride (quadruped + rider, flying)

```
Phases: approach 0–30% | mount 30–60% | settle 60–100%
Key poses:
  APPROACH: rider approaches, creature lowers body.
  MOUNT:    rider swings up, creature braces.
  SETTLE:   creature rises, rider adjusts.
Helpers: HoldPose(braced) during mount + BodyLayer(weight shift)
```

§7.20 Death-Fall (from height, all creatures)

```
Phases: flail 0–40% | impact 40–50% | bounce 50–65% | settle 65–100%
Key poses:
  FLAIL:   limbs splayed, IrregularLoop(fast flail).
  IMPACT:  MakeImpact(1.0) — full body.
  BOUNCE:  small root Y bounce (ease-out then ease-in).
  SETTLE:  limp, Flat tangents, root lowers to ground.
```

---

§8. EXTENSION PROTOCOL (for any animation not listed)

If the user requests an animation absent from §6 and §7:

```
1. CLASSIFY:
   a. Is it cyclic locomotion? → use §5.3 gait + §5.4 speed tier.
   b. Is it a one-shot action? → use MakeOneShot with §8.2 phase template.
   c. Is it a held/pose state? → use MakeHoldPose.
   d. Is it irregular/ambient? → use MakeIrregularLoop.
   e. Is it hybrid? → compose helpers, document in manifest.

2. DECOMPOSE (for one-shots):
   a. Identify beat structure: anticipation, action, recovery,
      plus any holds or secondary impacts.
   b. Assign phase percentages that sum to 100 ± 0.1.
   c. For each phase, define the KEY POSE in plain language
      (what bone angles / positions characterize this phase).

3. ASSIGN ROLES:
   a. Pull from §3 map for the creature type.
   b. Mark each role as used / optional-skipped / missing-hard.
   c. If a required role is missing, return AnimationError.

4. CHOOSE HELPERS:
   a. Pick from §5.1/§5.2 based on classification.
   b. Note which helper covers which phase.

5. SET TANGENTS: per §5.5.

6. VALIDATE: run §9 global rules.

7. OUTPUT:
   a. C# generator function.
   b. ClipManifest (§10).
   c. Human-readable summary: phases, roles used, contact mode,
      key poses hit, warnings.
```

§8.2 Universal one-shot phase template

Default if no better structure fits:

```
Anticipation 0–30% | Action 30–55% | Follow 55–75% | Recovery 75–100%
```

Override per-state using the phase splits in §7 where a closer match exists.

---

§9. GLOBAL RULES (enforced on every clip)

1. Bone discovery is mandatory — never guess names.
2. Base pose validated or synthesized-and-flagged.
3. Body layer is mandatory — hard error if root missing.
4. Spine/tail chains ordered head→tail before phase assignment.
5. One shared curve, cloned with phase offsets — no per-bone unique authoring.
6. Tangents set per §5.5, recorded in manifest.
7. Speed tiers are multipliers, never new states.
8. One-shots must include anticipation + follow-through.
9. No ground-contact claim without IK — contactless flag mandatory.
10. Every output carries ClipManifest + validation report.
11. Fail loudly with AnimationError on any hard rule violation.
12. Seed all random/irregular motion for reproducibility.
13. Skip missing optional roles with logged warning; never fabricate.
14. Hybrid junctions must be documented in manifest.
15. Amplitudes reduce ×0.7 when base pose is synthetic.

---

§10. THE CLIPMANIFEST (final schema)

```csharp
[Serializable] public struct ClipManifest {
    public string creatureType;          // "Quadruped"
    public string stateName;             // "Pounce"
    public string basePoseSource;        // "scene" | "synthetic"
    public ContactMode contactMode;
    public bool contactless;             // derived from contactMode
    public bool hasBodyLayer;            // must be true
    public bool hasSecondaryLayer;
    public bool hasHybridJunctions;
    public string[] rolesFound;
    public string[] rolesMissingOptional;
    public string[] rolesMissingHard;    // non-empty => clip invalid
    public string[] rolesAmbiguous;
    public string[] phasesUsed;          // ["coil","launch","air","land"]
    public string[] keyPosesHit;         // ["COIL","LAUNCH","AIR","LAND"]
    public TangentMode tangentMode;
    public int noiseSeed;
    public float durationSeconds;
    public float frameRate;
    public string[] validationWarnings;
    public string[] validationErrors;    // non-empty => do not play
}
```

If validationErrors is non-empty, the clip is returned with isSafeToPlay=false and must not be assigned to an Animator.

---

§11. PASTE-READY PROMPT PREAMBLE

"Follow the Complete Creature Animation Compendium v3 exactly. Steps:

1. Discover bones via §2 — output BoneMap + DiscoveryReport. Never guess.
2. Validate base pose via §4 — if bind/T-pose, hard error or GenerateNeutralBasePose (flag synthetic, ×0.7 amplitude).
3. Identify creature in §3 — check required vs optional.
4. Identify animation in §6 (basic) or §7 (advanced action) — if absent, follow §8 extension protocol.
5. Compose from §5 helpers, passing BoneMap and ContactMode.
6. Set tangents per §5.5.
7. Enforce §9 global rules — fail loudly with AnimationError on any hard violation.
8. Output: C# generator + ClipManifest (§10) + human-readable report listing phases, key poses hit, roles used, contact mode, warnings, and any AnimationErrors.
   If contactless, say so explicitly. If a role is missing-optional, log it. Never produce a silent bad clip."

---

§12. WHAT THIS COMPENDIUM COVERS

Creatures: biped, quadruped, hexapod, octopod, flying, aquatic, serpentine, hybrid, beast/amorphous.

Basic states: idle (+alert/sleep/eat), walk/run/sprint, crawl, swim, fly/glide/hover, climb, burrow, jump, fall, land, hurt (light/heavy), block, dodge, death, spawn, stun, knockback, cast/channel, taunt, carry, turn.

Advanced actions: pounce, bite, swing, claw/slash, tail-whip, grapple/grab, charge/rush, stomp/slam, roar/shout, roll/evade, dig/burrow-down, emerge/surface, wing-buffet/gust, constrict/coil, lunge/strike, kick, headbutt, spit/projectile, mount/ride, death-fall.

Extension system: any new animation maps via §8 classification → phase decomposition → role assignment → helper composition → validation → manifest.

Validation: rig discovery, base-pose check, contact-mode enforcement, phase-sum check, tangent policy, seeded randomness, hybrid junction documentation — all surfaced in the ClipManifest with hard errors when violated.

---

§13. IK, LANDING AND POSITION ADDITIONS (added by the project owner; extends §5.6, §6, §7 and §9)

This section adds to the compendium above. Nothing above it has been changed. Where §13 and the earlier sections both apply, both are followed.

§13.1 Foot IK (ContactMode FullIK)

```
For every bone in BoneMap.support:
  1. Raycast down from above the foot to find the ground point and ground normal.
  2. Plant the foot on the ground point; rotate the foot to the ground normal (clamped).
  3. Lower or raise the root so the lowest planted foot touches the ground.
  4. Blend IK weight with the animation: full weight while planted, fading out while the foot is in the air.
  5. Limit maximum step height and leg stretch; never over-extend a leg.
This is the IK that §9 rule 9 requires before any ground contact is claimed.
```

§13.2 Landing

```
Phases: contact 0–15% | compress 15–45% | absorb 45–70% | rise 70–100%
Key poses:
  CONTACT:  feet touch first; legs reach the ground point.
  COMPRESS: hips and spine lower; knees bend; depth scales with fall height.
  ABSORB:   body settles; arms and tail overshoot and return (follow-through).
  RISE:     body returns to the idle base pose.
Rules:
  - Compression depth and recovery time scale with fall height and with leg length.
  - Each foot lands on its own ground point (uneven ground allowed).
  - Hard landings call MakeImpact(intensity from fall height).
  - Landing position is snapped to the ground; the root never ends below or above it.
```

§13.3 Position control

```
For every state in §6 and §7, define and apply:
  - stance width and foot spacing,
  - root height above ground,
  - facing direction and turn rate,
  - weight shift between feet,
  - lean into movement.
All values scale with the creature's measured proportions (§13.6).
```

§13.4 Hand / effector IK

```
Keep both hands on a held weapon's grip points through swings and attacks (§7.3, §7.4).
The weapon hand follows the weapon path; the support hand stays on its grip with clamped reach.
```

§13.5 Look and aim IK

```
Head and chest turn toward a lock-on target within safe angle limits.
Eyes lead the head; the head leads the chest. Return smoothly when the target is lost.
```

§13.6 Body adaptation

```
At setup, measure leg length, arm length, torso length and total height from the discovered bones.
Scale stride length, step height, swing reach, crouch depth, jump height and foot spacing to those measurements.
Clamp extreme body shapes to safe ranges.
```

§13.7 Performance tiers

```
IK is applied by distance and device tier:
  - Low tier:  foot IK on the player's own character and nearby players only.
  - Mid tier:  foot IK within a medium radius; hand and look IK on the player only.
  - High tier: foot, hand and look IK within the full area of interest.
Characters beyond the IK radius play the animation without IK and set contactless=true in the manifest.
```

§13.8 Manifest and error additions

```
ClipManifest adds:
  public bool footIK, handIK, lookIK;
  public float landingHeight;      // fall height used for landing compression
  public bool groundSnapped;
AnimationError codes added to §1:
  "IK_TARGET_MISSING" | "GROUND_QUERY_FAILED"
```
