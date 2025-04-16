# Project-Org

# Project Structure

```
├── UniversalRenderPipelineGlobalSettings.asset
├── DialogueEditor/
│   ├── ConversationManager.prefab
│   ├── Assets/
│   │   ├── DefaultSprites/
│   │   │   ├── BackingImage.png
│   │   │   ├── Blank.png
│   │   ├── Prefabs/
│   │   │   ├── ConversationButton.prefab
│   │   ├── Scripts/
│   │   │   ├── Condition.cs
│   │   │   ├── Connection.cs
│   │   │   ├── Conversation.cs
│   │   │   ├── ConversationNode.cs
│   │   │   ├── EditableCondition.cs
│   │   │   ├── EditableConnection.cs
│   │   │   ├── EditableConversation.cs
│   │   │   ├── EditableNode.cs
│   │   │   ├── EditableParameter.cs
│   │   │   ├── EditableSetParamAction.cs
│   │   │   ├── NodeEventHolder.cs
│   │   │   ├── NPCConversation.cs
│   │   │   ├── Parameter.cs
│   │   │   ├── SetParamAction.cs
│   │   │   ├── Editor/
│   │   │   │   ├── DialogueEditorNode.cs
│   │   │   │   ├── DialogueEditorUtil.cs
│   │   │   │   ├── DialogueEditorWindow.cs
│   │   │   │   ├── NPCConversationEditor.cs
│   │   │   ├── UI/
│   │   │   │   ├── ConversationManager.cs
│   │   │   │   ├── UIConversationButton.cs
│   │   │   │   ├── Editor/
│   │   │   │       ├── ConversationManagerEditor.cs
│   ├── Example/
│   │   ├── DialogueEditorExample.unity
│   │   ├── ExampleInputManager.cs
│   │   ├── ManIcon.png
│   │   ├── WizardIcon.png
│   └── scripts/
│       ├── conversationstarter.cs
├── Enemies/
│   ├── enemy_scripts/
│   │   ├── EnemyCombat.cs
│   │   ├── EnemyStats.cs
│   ├── healthbar/
│   │   ├── enemy-lifebars.prefab
│   └── prefabs/
│       └── DungeonCharacters/
│           └── Skeletons_demo/
│               ├── DungeonSkeletons_pack.jpg
│               ├── animation/
│               │   ├── DS_onehand_attack_A.FBX
│               │   ├── DS_onehand_idle_A.FBX
│               │   ├── DS_onehand_walk.FBX
│               │   ├── enemy_ac.controller
│               │   ├── Standing React Small From Left.fbx
│               └── models/
│                   ├── DungeonSkeleton_demo.FBX
│                   └── Materials/
│                       ├── DemoEquipment.mat
│                       ├── DemoEquipment.tga
│                       ├── DemoSkeleton.png
│                       ├── DS_skeleton_standard.mat
├── HDRPDefaultResources/
│   └── HDRenderPipelineGlobalSettings.asset
├── Items nad weapons/
│   ├── lowpoly-sword-3d-model/
│   │   ├── source/
│   │   │   └── Sword.fbx
│   │   └── textures/
│   │       ├── sword.jpeg
│   │       └── sword.jpg
│   └── weapon_Scripts/
│       └── SwordDamage.cs
├── MainMenu/
│   ├── Gamblers_Curse.png
│   └── Scripts/
│       └── MainMenu.cs
├── Map_and_enviroment/
│   ├── collidors.fbx
│   ├── level.fbx
│   └── map_scripts/
│       └── make_player_crouch.cs
├── PauseMenu/
│   └── scripts/
│       └── PauseMenu.cs
├── Scenes/
│   ├── Fight_Scene_1.unity
│   ├── MainMenu.unity
│   ├── SampleScene.unity
│   └── Fight_Scene_1/
│       ├── NavMesh-NavMesh Surface 1.asset
│       ├── NavMesh-NavMesh Surface 2.asset
│       ├── NavMesh-NavMesh Surface 3.asset
│       └── NavMesh-NavMesh Surface.asset
├── Settings/
│   ├── High_PipelineAsset.asset
│   ├── High_PipelineAsset_ForwardRenderer.asset
│   ├── Low_PipelineAsset.asset
│   ├── Low_PipelineAsset_ForwardRenderer.asset
│   ├── Medium_PipelineAsset.asset
│   ├── Medium_PipelineAsset_ForwardRenderer.asset
│   ├── Ultra_PipelineAsset.asset
│   ├── Ultra_PipelineAsset_ForwardRenderer.asset
│   ├── Very High_PipelineAsset.asset
│   ├── Very High_PipelineAsset_ForwardRenderer.asset
│   ├── Very Low_PipelineAsset.asset
│   └── Very Low_PipelineAsset_ForwardRenderer.asset
├── StarterAssets/
│   ├── InputSystem/
│   │   ├── StarterAssets.inputactions
│   │   ├── StarterAssets.inputsettings.asset
│   │   └── StarterAssetsInputs.cs
│   └── ThirdPersonController/
│       ├── Character/
│       │   ├── Animations/
│       │   │   ├── attack1.fbx
│       │   │   ├── attack2.fbx
│       │   │   ├── attack3.fbx
│       │   │   ├── Jump Attack.fbx
│       │   │   ├── Jump--InAir.anim.fbx
│       │   │   ├── Jump--Jump.anim.fbx
│       │   │   ├── Locomotion--Run_N.anim.fbx
│       │   │   ├── Locomotion--Run_N_Land.anim.fbx
│       │   │   ├── Locomotion--Run_S.anim.fbx
│       │   │   ├── Locomotion--Walk_N.anim.fbx
│       │   │   ├── Locomotion--Walk_N_Land.anim.fbx
│       │   │   ├── Sprinting Forward Roll.fbx
│       │   │   ├── Stand To Crouch.fbx
│       │   │   ├── Stand--Idle.anim.fbx
│       │   │   └── StarterAssetsThirdPerson.controller
│       │   ├── Bars/
│       │   │   ├── images.png
│       │   │   └── TH_24579065...jpg
│       │   ├── Materials/
│       │   │   ├── M_Armature_Arms.mat
│       │   │   ├── M_Armature_Body.mat
│       │   │   └── M_Armature_Legs.mat
│       │   ├── Models/
│       │   │   └── Armature.fbx
│       │   ├── Sfx/
│       │   │   ├── Player_Footstep_01.wav ... Player_Footstep_10.wav
│       │   │   └── Player_Land.wav
│       │   └── Textures/
│       │       ├── Armature_Arms_AlbedoTransparency.tif
│       │       ├── Armature_Arms_MetallicSmoothness.tif
│       │       ├── Armature_Arms_Normal.tif
│       │       ├── Armature_Arms_RGB.tif
│       │       ├── Armature_Body_AlbedoTransparency.tif
│       │       ├── Armature_Body_MetallicSmoothness.tif
│       │       └── Armature_Body_Normal.tif

```

# Main Scripts

## **EnemyCombat.cs**

- **Player Detection & Chasing:**
  The enemy detects the player within a range for chase (chaseRange) and moves toward them.
- **Attack Execution:**
  If the player enters the attackrange, the enemy stops, faces them, plays an attack animation, waits (WarningDelay), then deals damage if the player is still in front and within range.
- **Cooldown & Timing:**
  Attacks have a cooldown, and damage is only applied if the player is still in the attack zone and direction after the delay.
- **Animation Control:**
  Triggers walking and attack animations based on movement and combat state.
- **Debug Gizmos:**
  Visualizes attack and chase ranges in the editor with colored wire spheres.

## EnemyStats.cs

- **Health System:**
  Gives the enemy health, shows a health bar, and lets it take damage.
- **Damage Reaction:**
  Plays a hurt animation when hit and dies when health reaches zero.
- **Health Bar Display:**
  Spawns a floating health bar above the enemy that follows and updates as it gets hurt.

## BasicRigidBodyPush.cs

A default script from the Unity basic movemnt asset

## SwordDamage.cs

- ⚠️ **Deals Damage of the enemies are here not in the emey scripts:**
  Damages enemies when the sword hits them during an attack.\*
- **Fasts and Heavy Attacks:**
  Supports light(-damage,-stamina) and heavy(+damage,+stamina) swings.
- **Attack Detection:**
  Only works when an attack is "active" — prevents random damage outside attacks.
  \*it seems a bit confusing the damage dealing maybe in the future will add a single script for that

## PlayerStamina.cs

- **Stamina System:**
  Tracks how much energy (stamina) the player has.
- **Uses & Regains Stamina:**
  - Spends stamina when running or doing actions.
  - “Slowly”(too fast for now but changable) regenerates over time.
- **UI :**
  Updates a stamina bar so the player can see how much is left.

## PlayerAttackSystem:

- **Attack Control:**
  Lets the player perform **fast** or **heavy attacks**, with cooldowns and stamina usage.
- **Combo System:**
  Supports up to 3 chained fast attacks if timed correctly (combo).
- **Stamina Use:**
  Heavy attacks cost more stamina than fast ones, limiting how often they can be used.
- **Visual Feedback (will change this later)**
  Changes the sword color to **green** when you’re in the right timing window for a combo.
- **Animation :**
  Triggers different sword attack animations depending on which combo step you're on times are not correct but changeble maybe.

## PlayerStats:

- **Players Health Management:**
  - Tracks the player's **current** and **max health**.
  - Reduces health when the player takes damage, and stops at 0 health.
- **Damage Handling:**
  - Reduces health when is called. If health reaches 0, the player "dies".
- **Health UI:**
  - Updates the health bar UI smoothly to show current health.
- **Death Handling:**
  - When health drops to zero, a death message is logged, and additional logic (like respawn) can be added. ← need an animaiton in the future

## make_player_crouch.cs

THis forces the player to crouch in certain areas, because here the player is only displaying the crouch animaton and not changing the coliders.

## conversationstarter.cs (we are using an asset for this one as well)

- **Trigger Interaction**
- **Conversation Start**
- **Trigger Exit**
- **End Conversation Method**
- **Key Features**

## ThirdPersonController.cs ← Need to separate this maybe

### **1. Movement and Speed (came with the asset)**

- The character can move at different speeds depending on the situation:
  - **Walking** (normal speed)
  - **Sprinting** (faster movement for quick travel)
  - **Crouching** (slower movement for stealth or tight spaces)
- Movement is controlled through the keyboard (WASD or arrow keys), and the character's speed adjusts based on the actions they are performing (walking, sprinting, crouching).

---

### **2. Jumping and Gravity (came with the asset)**

- The character can **jump** to reach higher areas or avoid obstacles.
- **Gravity** pulls the character back down after jumping.
- The system also ensures the player can’t jump again too quickly (there’s a small delay between jumps to prevent spam).

---

### **3. Camera Control (using cinemachine)**

- The **camera** follows the player's mouse movements, allowing them to look around in 360 degrees.
- The camera’s movement is **limited** up and down (to prevent unnatural looking too far up or down).

---

### **4. Lock-On Targeting (using cinemachine as well)**

- The player can **lock-on** to nearby enemies, causing the camera to focus on the locked target.
- The lock-on system helps during **combat**, ensuring the player always faces the target even while moving.
- we need to add scpecific facing the enemy animaitons as well

---

### **5. Rolling Mechanism**

- **Rolling** is a quick movement used to dodge attacks or move faster.
- Rolling consumes **stamina**, and the player can’t roll indefinitely.
- The character’s speed and the duration of the roll are determined by how the player activates the action.

---

### **6. Crouching Mechanism →** make_player_crouch.cs

- **Crouching** allows the character to move more slowly, useful for stealth or navigating smaller spaces.
- The player presses a button to crouch and move at a reduced speed.
- Only the animation is played here,colliders not changed only in the scene that is why make_player_crouch

---

### **7. Stamina Management → PlayerStamina.cs**

- The character has a **stamina bar** that decreases when sprinting or rolling.
- Stamina regenerates when the player isn’t performing these actions.

\*We need do decrement this one, because it has too many lines and code we should separate this

# What to do Next

## Programing

- **Improve**
  - Animations and more fluid movement
  - lightning
  - npc conversations
  - npc taking damage
- **Add**
  - more elements
  - more enemies with dif attack patterns
  - Final level Boss
  - Finish First level
  - gambling feature
  - death screen
  - player taking damage animation
  - camera shakes and movement

## Design

- **Improve**
  - Animations and maybe change some of them
- **Add**
  - custom UI
  - More Map Elements
  - character mesh
  - enemies mesh
  -
