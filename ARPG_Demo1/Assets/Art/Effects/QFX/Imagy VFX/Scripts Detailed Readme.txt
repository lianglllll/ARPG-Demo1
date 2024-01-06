-----------------------------------------------------------------------------------------------------------
BlinkController
-----------------------------------------------------------------------------------------------------------

BlinkOffset - the length of the Blink
LineRenderer - Prefab of the LineRenderer

AddMaterial - add "material adder" into target object. (Material Adder - adds material to specific lifetime)
Material - Material for "material adder"
MaterialLifeTime - Lifetime of the material
MaterialTargets - You can specify the certain objects for which the material will be added

AddShaderAnimator - adds "shader animator" into target object
DynamicShaderParameter - settings of the shader animator parameter

MakeClone - allows to make clone
CloneSpecificTarget - you can set a specific object as a cloning target
CloneOpacityAnimation - opacity animation of the clone
CloneLifeTime - lifetime of the clone
CloneMaterial - material of the clone
CloneComponentsWithRequirements - you can specify several components that should be removed. Since they have circular references, and it's impossible to do this automatically

LineRendererWidthAnimation - Animation of the LineRenderer

BlinkLaunchFx - Start blink effect
BlinkLaunchFxAnchor - Position of the Start blink effect

BlinkLandingFx - Effect on the ground
BlinkLandingFxAnchor - Position of the ground effect

-----------------------------------------------------------------------------------------------------------
DarkParalysisAbilityController
-----------------------------------------------------------------------------------------------------------

TargetObjectFinder - Will find a specific object near to the effect

AuraCloner - Component which will create a object clones
AuraClonerLifeTime - AuraCloner will be added to the found object near to effect, and it’s a lifetime of new component

AnimatorAffector - Will play animation clip in found object

DarkParalysisStaff - Prefab of the staff
InstantiateDelay - Delay between copies of the staff
Transform[] DarkParalysisStaffsPosition - Positions of the staffs

-----------------------------------------------------------------------------------------------------------
PaperPowerAbilityController
-----------------------------------------------------------------------------------------------------------

ActivateTransformParent - parent of the target position to throw. this object will have “Emitter” position,
ActivateTransform - position to throw paper staff

LineRenderer - Ink curve line renderer
LaunchPs - Launch splash
LaunchPsTransform - launch splash position
HitPs - Hit splash
LaunchPsDelay - Launch splash delay
HitPsDelay - Hit splash delay

BezierCurveSegmentCount - Count of Ink curve segments
CurveSettings[] Curves - Positions of ink curves to the calculate bezier curve
InstantiateCurveDelay - Delay between curves

LaunchAnchorName - Will find this object in the “Emitter”, for example anchor attached to the hand.

LerpMotion - Will be used for the paper staff motion, you can tweak params of motion here.

MotionFinishedPs - Paper staff motion finished particle system.

PaperPowerSelfTransform - Paper staff without any dependency, like ActivateTransform.

-----------------------------------------------------------------------------------------------------------
SoulCrystalAbilityController
-----------------------------------------------------------------------------------------------------------

ShakeRadius - Radius of random sphere for shake
ShakeDuration - Shake duration
ShakeRate - Shake rate

Crystal - “Normal” Crystal part
BrokenCrystal - “Broken” Crystal part
BrokenPartOffset - Broken parts will move for this length

BrokenPartSpeed - Speed of the lerp motion
BrokenPartLifeTime - LifeTime of the broken part
BrokenPartShaderAnimator - Shader animator of the broken part to the dissolve it

HomingParticleSystem - “Souls” particle system, will follow found object
HomingParticleSystemShaderAnimator - shader animator to animate “trail”, because CustomParameters can’t be passed into particle trail.
TargetObjectFinder - Will find a specific object to follow him. You can modify HomingParticleSystem to follow center of the bounds.

-----------------------------------------------------------------------------------------------------------
IFX_DangerousPumpkinAbilityController
-----------------------------------------------------------------------------------------------------------

JumpingPs - effect of jumping
LandingPS - effect of landing
DeathPs - effect of death

AccelerationDistance - distance to target object when motion will be accelerated
AccelerationFactor - value to accelerate motion

ObjectFinder - will find target object to follow him

DeathAnimationStateName - animation of pumpkin when it very close to object and ready to explode


-----------------------------------------------------------------------------------------------------------
IFX_DoubleAttackShurikenAbilityController
-----------------------------------------------------------------------------------------------------------

AttachAnchorName - name of the anchor to which shuriken will be attached until it will be in “Activated” state (until function “Activate” will be called)

ShurikenPs - shuriken itself
ThrowPs - effect of throwing
ShurikenMotionSpeed - motion speed of shuriken PS
ShurikenMotionRotatationSpeed - turn speed of shuriken PS

TargetOffset - point (offset) located behind target object or position
ReturnDistance - distance to target point to return shuriken

DissapearDistance - distance when Shader animator will be run and shuriken with the trail will be dissolved

TrailShaderAnimator, ShurikenShaderAnimator - shader animators to dissolve these objects

PositionY - if selection method is “Position”, not a “Detect object”, you can specify Y position of shuriken target

-----------------------------------------------------------------------------------------------------------
IFX_GrenadeAbilityController
-----------------------------------------------------------------------------------------------------------

GrenadeFx - grenade with the mesh, this object will be thrown
GrenadeMesh - mesh itself, will be hidden when it will be very close to target point
FxAnchorName - name of anchor to attach grenade until it will be thrown
FxAnchorRotation - anchor rotation offset
FxAnchorOffset - anchor position offset

ExplodeDistance - distance to target object (position) to explode
ExplosionPs - effect of explosion

TrailRenderer - trail renderer of grenade, which will be shown when grenade will be thrown
TrailShaderAnimator - trail shader animator, which will dissolve trail when it will be very close to target point

-----------------------------------------------------------------------------------------------------------
IFX_GunshotAbilityController
-----------------------------------------------------------------------------------------------------------

LaunchAnchorName - name of anchor to attach gun
ShaderAnimator - shader animator to appear and dissolve gun
ProjectileWeapon - projectile weapon to instantiate projectiles and do other work with effects

-----------------------------------------------------------------------------------------------------------
IFX_ShadowHoleAbilityController
-----------------------------------------------------------------------------------------------------------

ShadowAura - effect of shadow aura on emitter object
ShadowHole - effect of shadow hole on target object

LineRenderer - shadow curve line renderer
BezierCurveSegmentCount - count of curve segments
CurveSettings[] Curves - control points of curves

IsTargetMotionEnabled - if enabled target object will be moved
IsTargetDestroyEnabled - if enabled target object will be destroyed

TargetMotionDelay - delay to move target object
TargetMotionOffset - amount of offset
TargetMotionSpeed - target object motion speed

TargetLifeTime - life time of target object

-----------------------------------------------------------------------------------------------------------
IFX_WaterSplashAbilityController
-----------------------------------------------------------------------------------------------------------

CastingPs - effect of casting on emitter
TargetPs - effect of water splash on target object

TrailRenderer - trail renderer which will be created and attached to hands
RightHandAnchorName - name of the anchor to attach the trail
LeftHandAnchorName - name of the anchor to attach the trail

-----------------------------------------------------------------------------------------------------------
IFX_SakuraStrikeAbilityController
-----------------------------------------------------------------------------------------------------------

MuzzleFlashPs - effect of muzzle flash
ImpactPs - effect of impact
StrikeLineRenderer - line renderer for glaive texture and shader animator

StrikeCount - count of strikes
StrikeDelay - delay between strikes
StrikeRadius - radius of the random circle to instantiate strikes
StrikeOffset - offset of the random circle

-----------------------------------------------------------------------------------------------------------
Other:
-----------------------------------------------------------------------------------------------------------

________________
AnimatorAffector
Settings to play animation clip.

_______________
HomingParticleSystem
Allows particles follow the target object.

________________
AuraCloner
Component to create clones:

CloneLifeTime - Life time of the clone
CloneRate - Create clone rate

IsRandomOffsetEnabled - If enabled position for the new clone will be inside random sphere
RandomSphereSize - Size of the random sphere
CloneOffset - Uses if IsRandomOffsetEnabled  is disabled to move clone forward/backward

CloneMaterial - All materials will be replaced by this material

CloneSpeed - Speed of clone (Uses lerp motion component)
CloneOpacityAnimation - Opacity animation (Uses shader animator component)

ForceMakeClone - If enabled, CloneRate won’t work, clone will be create only by call function

ComponentsWithRequirements - all components except of “Transform”, “Renderer”, “MeshFilter” will be removed from the clone, but some of the has component with the dependency, therefore you can specify these components, for example it’s ThirdPersonCharacterController, etc..
