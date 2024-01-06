--------------------------------------------------------------------------------------------------------------------
Animators
--------------------------------------------------------------------------------------------------------------------
______________
AnimationModule
Common settings to create the custom curve based on an animator
______________
DynamicShaderParameter
Shader parameter for the ShaderAnimator component
______________
LightAnimator
light intensity animator based on curve 
______________
ShaderAnimator
shader parameters animator based on curve 
______________
ScaleAnimator
object scale animator based on curve 

--------------------------------------------------------------------------------------------------------------------
______________
ControlledObject
It's a base object to allow you to specify a delay before running the object, and it won't execute the code if object is disabled.
“Stub” for Pool Controller in the next updates.
--------------------------------------------------------------------------------------------------------------------

--------------------------------------------------------------------------------------------------------------------
Motion
--------------------------------------------------------------------------------------------------------------------

______________
LerpMotion
Component to achieve lerp motion of the object. Doesn’t use “alone”, only as a part of the secondary scripts.

--------------------------------------------------------------------------------------------------------------------
Target
--------------------------------------------------------------------------------------------------------------------

______________
ObjectFinder
Settings and function to find specific object inside area, to find object which should contains Collider.

______________
TargetAttacher
Scripts to attach and detach object. It uses in the IAnimatorAbility script

--------------------------------------------------------------------------------------------------------------------
Collision
--------------------------------------------------------------------------------------------------------------------
______________
IFX_ColliderIntersectionFxInstancer
Finds objects around the object and creates FX object if there are any, requires rigidbody on the object

--------------------------------------------------------------------------------------------------------------------
Weapon
--------------------------------------------------------------------------------------------------------------------
______________
IFX_ManualProjectileWeapon
Simple projectile weapon, which needs to call Shoot function to instantiate projectile

______________
IFX_SimpleProjectile
Simple projectile, which just moves forward

--------------------------------------------------------------------------------------------------------------------
Trail
--------------------------------------------------------------------------------------------------------------------

______________
IFX_HorizontalTrail
Procedural generation of mesh, which is always alignment in horizontal axis

--------------------------------------------------------------------------------------------------------------------
Particle System
--------------------------------------------------------------------------------------------------------------------

______________
HomingParticleSystem
Allows particles follow the target object

______________
IFX_ParticleSystemLookAtCamera
Allows particles rotate towards to the camera (which is using by LightningSplash)

--------------------------------------------------------------------------------------------------------------------
Utils
--------------------------------------------------------------------------------------------------------------------

______________
MaterialAdder
Adds the material to the all renderers of the objects

______________
MaterialReplacer
Replaces all materials of all object renderers by one material

______________
GroundAttacher
Attaches the object to the ground

______________
SelfDestroyer
Destroys the object after a delay

______________
AuraCloner
Allows to create clones

______________
Cloner
Allows to create one clone

______________
AnimatorAffector
Settings to play animation clip

--------------------------------------------------------------------------------------------------------------------
Ability
--------------------------------------------------------------------------------------------------------------------

AbilityFx - Scriptable object with simple ability settings

AnimatorAbilityFx - Scriptable object with ability setting that will be used with character animation.

  “LaunchStateName” - this animation clip will be player when AnimatorAbilityFxManager will launch ability

  “IsActivationRequired: - if ability is complicated like “Paper Power”, you need to to call LaunchFx/ActivateFx functions from the AnimatorController in order to use the functions of launch and activate. 

AbilityFxManager - Contains several abilities. Launches ability at target position and plays sound.

AnimatorAbilityFxManager - Will launch and activate AnimatorAbilityFx

AbilityUiManager - Shows ui for the ability and calls function in AbilityFxManager or AnimatorAbilityFxManager.

Has two modes to select position: “Position” - just to get raycast hit position, “DetectObject” - to detect object in area around raycast hit (Will use ObjectFinder settings to detect the object)

LaunchAbilityName - to specify which ability need to launch, you need to change this parameter, manually, or from secondary ui (Ability name it’s a FxName in the ScriptableObject)

DefaultAnimatorAbilityFxController - to instantiate simple ability (without any logic) by animator event (for example lightning splash)
