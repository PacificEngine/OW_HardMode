using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace ClassLibrary2
{
    class AnglerfishHelper
    {
        private static void Start(ref AnglerfishController __instance)
        {
            //Anglerfish.createAnglerfish(__instance);
        }

        private static bool Awake(ref AnglerfishController __instance)
        {
            Anglerfish.anglerfish.Add(__instance);
            Anglerfish.updateAnglerfish(__instance);
            Anglerfish._helper?.Console?.WriteLine("Anglerfish Awaken");
            return true;
        }

        private static bool OnSectorOccupantsUpdated(ref AnglerfishController __instance)
        {
            if (Anglerfish.createdAnglerfish.Contains(__instance))
            {
                Anglerfish._helper?.Console?.WriteLine("Update");
            }
            return !Anglerfish.createdAnglerfish.Contains(__instance);
        }


        private static bool OnSectorOccupantAdded(ref AnglerfishController __instance)
        {
            if (Anglerfish.createdAnglerfish.Contains(__instance))
            {
                Anglerfish._helper?.Console?.WriteLine("Added");
            }
            return !Anglerfish.createdAnglerfish.Contains(__instance);
        }

        private static bool OnSectorOccupantRemoved(ref AnglerfishController __instance)
        {
            if (Anglerfish.createdAnglerfish.Contains(__instance))
            {
                Anglerfish._helper?.Console?.WriteLine("Remove");
            }
            return !Anglerfish.createdAnglerfish.Contains(__instance);
        }

        private static bool SetSector(ref Sector sector, ref AnglerfishController __instance)
        {
            if (sector != null)
                Anglerfish._helper?.Console?.WriteLine("SetSector");
            if (Anglerfish.createdAnglerfish.Contains(__instance))
            {
                //sector = null;
            }
            return true;
        }


        private static bool OnDestroy(ref AnglerfishController __instance)
        {
            Anglerfish.anglerfish.Remove(__instance);
            Anglerfish.createdAnglerfish.Remove(__instance);
            return true;
        }

        private static bool onFeel(ref ImpactData impact)
        {
            if (!Anglerfish.canFeel)
            {
                var attachedOwRigidbody = impact.otherCollider.GetAttachedOWRigidbody();
                if ((attachedOwRigidbody.CompareTag("Player") || attachedOwRigidbody.CompareTag("Ship")))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool onHearSound(ref NoiseMaker noiseMaker)
        {
            return Anglerfish.canHear;
        }

        /*private static bool IsLightVisible(ref FogLight.LightData lightData, ref bool __result)
        {
            __result = true;
            return false;
        }

        private static bool DetermineFogVisibilityToPlayer(ref CanvasMarker __instance)
        {
            __instance.enabled = true;
            __instance.SetVisibility(true);
            __instance.SetFogVisibility(true);
            return false;
        }


        private static bool SetFogVisibility(ref bool value)
        {
            value = true;
            return true;
        }


        private static bool SetVisibility(ref bool value)
        {
            value = true;
            return true;
        }

        private static bool AwakeOuterFogWarpVolume(ref OuterFogWarpVolume __instance)
        {
            var values = Enum.GetValues(typeof(OuterFogWarpVolume.Name));
            var random = new System.Random();
            var randomValue = (OuterFogWarpVolume.Name)values.GetValue(random.Next(values.Length));
            __instance.SetValue("_name", randomValue);
            return true;
        }*/
    }

    static class Anglerfish
    {
        public static HashSet<AnglerfishController> anglerfish = new HashSet<AnglerfishController>();
        public static HashSet<AnglerfishController> createdAnglerfish = new HashSet<AnglerfishController>();

        public static ModHelper _helper;

        private static bool? _enabledAI = null;
        private static bool _canStun = true;
        private static bool _canFeel = false;
        private static bool _canHear = false;
        private static bool _canSmell = false;
        private static bool _canSee = false;
        private static float? _overrideAcceleration = null;
        private static float? _overrideInvestigateSpeed = null;
        private static float? _overrideChaseSpeed = null;
        private static float? _overrideTurnSpeed = null;
        private static float? _overrideQuickTurnSpeed = null;
        private static float? _overrideMouthOpenDistance = null;
        private static float? _overridePursueDistance = null;
        private static float? _overrideEscapeDistance = null;

        private static float _visionX = 180f;
        private static float _visionY = 100f;
        private static float _visionYoffset = 45f;
        private static float _visionDistance = 200f;
        private static float _smellDistance = 500f;

        public static bool? enabledAI { get { return _enabledAI.GetValueOrDefault(true); } set { _enabledAI = value; updateAnglerfish(); } }
        public static bool canStun { get { return _canStun; } set { _canStun = value; } }
        public static bool canFeel { get { return _canFeel; } set { _canFeel = value; } }
        public static bool canHear { get { return _canHear; } set { _canHear = value; } }
        public static bool canSmell { get { return _canSmell; } set { _canSmell = value; } }
        public static bool canSee { get { return _canSee; } set { _canSee = value; } }
        public static float? acceleration { get { return _overrideAcceleration.GetValueOrDefault(getParameter("_acceleration", 2f)); } set { _overrideAcceleration = value; updateParameter("_acceleration", value, 2f); } }
        public static float? investigateSpeed { get { return _overrideInvestigateSpeed.GetValueOrDefault(getParameter("_investigateSpeed", 20f)); } set { _overrideInvestigateSpeed = value; updateParameter("_investigateSpeed", value, 20f); } }
        public static float? chaseSpeed { get { return _overrideChaseSpeed.GetValueOrDefault(getParameter("_chaseSpeed", 42f)); } set { _overrideChaseSpeed = value; updateParameter("_chaseSpeed", value, 42f); } }
        public static float? turnSpeed { get { return _overrideTurnSpeed.GetValueOrDefault(getParameter("_turnSpeed", 180f)); } set { _overrideTurnSpeed = value; updateParameter("_turnSpeed", value, 180f); } }
        public static float? quickTurnSpeed { get { return _overrideQuickTurnSpeed.GetValueOrDefault(getParameter("_quickTurnSpeed", 360f)); } set { _overrideQuickTurnSpeed = value; updateParameter("_quickTurnSpeed", value, 360f); } }
        public static float? mouthOpenDistance { get { return _overrideMouthOpenDistance.GetValueOrDefault(getParameter("_arrivalDistance", 100f)); } set { _overrideMouthOpenDistance = value; updateParameter("_arrivalDistance", value, 100f); } }
        public static float? pursueDistance { get { return _overridePursueDistance.GetValueOrDefault(getParameter("_pursueDistance", 200f)); } set { _overridePursueDistance = value; updateParameter("_pursueDistance", value, 200f); } }
        public static float? escapeDistance { get { return _overrideEscapeDistance.GetValueOrDefault(getParameter("_escapeDistance", 500f)); } set { _overrideEscapeDistance = value; updateParameter("_escapeDistance", value, 500f); } }

        public static float? visionX { get { return _visionX; } set { _visionX = value.GetValueOrDefault(180f); } }
        public static float? visionY { get { return _visionY; } set { _visionY = value.GetValueOrDefault(100f); } }
        public static float? visionYoffset { get { return _visionYoffset; } set { _visionYoffset = value.GetValueOrDefault(45f); } }
        public static float? visionDistance { get { return _visionDistance; } set { _visionDistance = value.GetValueOrDefault(200f); } }
        public static float? smellDistance { get { return _smellDistance; } set { _smellDistance = value.GetValueOrDefault(500f); } }

        public static void Start(ModHelper helper)
        {
            _helper = helper;
            helper.HarmonyHelper.AddPostfix<AnglerfishController>("Start", typeof(AnglerfishHelper), "Start");
            helper.HarmonyHelper.AddPrefix<AnglerfishController>("Awake", typeof(AnglerfishHelper), "Awake");
            helper.HarmonyHelper.AddPrefix<AnglerfishController>("OnSectorOccupantAdded", typeof(AnglerfishHelper), "OnSectorOccupantAdded");
            helper.HarmonyHelper.AddPrefix<AnglerfishController>("OnSectorOccupantsUpdated", typeof(AnglerfishHelper), "OnSectorOccupantsUpdated");
            helper.HarmonyHelper.AddPrefix<AnglerfishController>("OnSectorOccupantRemoved", typeof(AnglerfishHelper), "OnSectorOccupantRemoved");
            helper.HarmonyHelper.AddPrefix<AnglerfishController>("SetSector", typeof(AnglerfishHelper), "SetSector");
            helper.HarmonyHelper.AddPrefix<AnglerfishController>("OnDestroy", typeof(AnglerfishHelper), "OnDestroy");
            helper.HarmonyHelper.AddPrefix<AnglerfishController>("OnImpact", typeof(AnglerfishHelper), "onFeel");
            helper.HarmonyHelper.AddPrefix<AnglerfishController>("OnClosestAudibleNoise", typeof(AnglerfishHelper), "onHearSound");
            /*helper.HarmonyHelper.AddPrefix<FogLightManager>("IsLightVisible", typeof(AnglerfishHelper), "IsLightVisible");
            helper.HarmonyHelper.AddPrefix<CanvasMarker>("DetermineFogVisibilityToPlayer", typeof(AnglerfishHelper), "DetermineFogVisibilityToPlayer");
            helper.HarmonyHelper.AddPrefix<CanvasMarker>("SetFogVisibility", typeof(AnglerfishHelper), "SetFogVisibility");
            helper.HarmonyHelper.AddPrefix<CanvasMarker>("SetVisibility", typeof(AnglerfishHelper), "SetVisibility");
            helper.HarmonyHelper.AddPrefix<OuterFogWarpVolume>("OnAwake", typeof(AnglerfishHelper), "AwakeOuterFogWarpVolume");*/
        }

        public static void Awake()
        {
        }

        public static void Destroy(ModHelper helper)
        {

        }

        public static void createAnglerfish(AnglerfishController anglerfishController)
        {
            if (anglerfishController.enabled && createdAnglerfish.Count == 0 && Time.timeSinceLevelLoad > 1f)
            {
                var parent = Locator.GetAstroObject(AstroObject.Name.TimberHearth)?.GetAttachedOWRigidbody();
                if (parent)
                {
                    var controller = AnglerfishController.Instantiate(anglerfishController, parent.GetPosition() + new Vector3(0f, 300f, 0f), Quaternion.identity, parent.transform);
                    controller.GetAttachedOWRigidbody().SetVelocity(parent.GetVelocity());
                    controller.SetValue("_brambleBody", parent);
                    controller.SetSector((SectorHelper.getSector(Sector.Name.TimberHearth) ?? SectorManager.GetRegisteredSectors())[0].GetRootSector());
                    createdAnglerfish.Add(controller);

                    if (controller.gameObject.GetCullGroup())
                    {
                        _helper.Console.WriteLine("anglerfish-c1" + controller.gameObject.GetCullGroup());
                    }

                    if (controller.gameObject.GetLightsCullGroup())
                    {
                        _helper.Console.WriteLine("anglerfish-c2" + controller.gameObject.GetLightsCullGroup());
                    }

                    if (controller.gameObject.GetCollisionGroup())
                    {
                        _helper.Console.WriteLine("anglerfish-c3" + controller.gameObject.GetCollisionGroup());
                    }
                }

                /* controller.GetComponentsInChildren<object>()
                 * Anglerfish_Body(Clone) (OWRigidbody)	Hard Mode	
Anglerfish_Body(Clone) (MatchInitialMotion)	Hard Mode	
Anglerfish_Body(Clone) (AnglerfishController)	Hard Mode	
Anglerfish_Body(Clone) (ImpactSensor)	Hard Mode	
Anglerfish_Body(Clone) (NoiseSensor)	Hard Mode	
Anglerfish_Body(Clone) (CenterOfTheUniverseOffsetApplier)	Hard Mode	
AudioController (AnglerfishAudioController)	Hard Mode	
LoopSource (OWAudioSource)	Hard Mode	
OneShotSource (OWAudioSource)	Hard Mode	
OneShotSource_LongRange (OWAudioSource)	Hard Mode	
JawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
JawsOfDestruction (OWCollider)	Hard Mode	
JawsOfDestruction (OWTriggerVolume)	Hard Mode	
Beast_Anglerfish (AnglerfishAnimController)	Hard Mode	
Lure_PointLight (LightLOD)	Hard Mode	
Lure_FogLight (FogLight)	Hard Mode	
Beast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
Beast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
Beast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
Beast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
Beast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
Beast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode
                */
            }
        }

        public static void Update()
        {
            if (createdAnglerfish.Count == 0 && anglerfish.Count > 0)
            {
                var i = anglerfish.GetEnumerator();
                i.MoveNext();
                createAnglerfish(i.Current);
            }
            foreach (AnglerfishController anglerfishController in createdAnglerfish)
            {
                SetVisible(anglerfishController, true, true);
            }
            foreach (AnglerfishController anglerfishController in anglerfish)
            {
                updateAnglerfish(anglerfishController);
                /*
                 * anglerfishAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfishAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfishAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfishAnglerfish_Body (NoiseSensor)	Hard Mode
anglerfishAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfishAudioController (AnglerfishAudioController)	Hard Mode	
anglerfishLoopSource (OWAudioSource)	Hard Mode	
anglerfishOneShotSource (OWAudioSource)	Hard Mode	
anglerfishOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfishJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfishJawsOfDestruction (OWCollider)	Hard Mode	
anglerfishJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfishBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfishLure_PointLight (LightLOD)	Hard Mode	
anglerfishLure_FogLight (FogLight)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfishBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
parentFogLight_FishEgg (FogLight)	Hard Mode
                 */

                if (!canStun)
                {
                    updateParameter(anglerfishController, "_stunTimer", 0f);
                }

                if (anglerfishController.isActiveAndEnabled)
                {
                    if (canSmell && (anglerfishController.GetAnglerState() == AnglerfishController.AnglerState.Investigating || anglerfishController.GetAnglerState() == AnglerfishController.AnglerState.Lurking))
                    {
                        var distance = Locator.GetPlayerBody().GetPosition() - anglerfishController.transform.position;
                        var bramble = getParameter<OWRigidbody>(anglerfishController, "_brambleBody");

                        if (distance.magnitude < _smellDistance)
                        {
                            anglerfishController.SetValue("_localDisturbancePos", bramble.transform.InverseTransformPoint(getPlayerBody().GetPosition()));
                            anglerfishController.Invoke("ChangeState", AnglerfishController.AnglerState.Investigating);
                        }
                    }
                    if (canSee && (anglerfishController.GetAnglerState() == AnglerfishController.AnglerState.Investigating || anglerfishController.GetAnglerState() == AnglerfishController.AnglerState.Lurking))
                    {
                        var distance = Locator.GetPlayerBody().GetPosition() - anglerfishController.transform.position;
                        var xAngle = Vector3.Angle(distance, anglerfishController.transform.forward);
                        var yAngle = Vector3.Angle(distance, anglerfishController.transform.up) - _visionYoffset;

                        if (distance.magnitude <= _visionDistance && (xAngle * 2) <= _visionX && 0 <= yAngle && yAngle <= _visionY)
                        {
                            anglerfishController.SetValue("_targetBody", getPlayerBody());
                            anglerfishController.Invoke("ChangeState", AnglerfishController.AnglerState.Chasing);
                        }
                    }
                }
            }
        }

        private static void updateAnglerfish()
        {
            foreach (AnglerfishController anglerfishController in anglerfish)
            {
                updateAnglerfish(anglerfishController);
            }
        }

        public static void updateAnglerfish(AnglerfishController anglerfishController)
        {
            if (enabledAI != null)
                anglerfishController.enabled = enabledAI.Value;
            updateParameter(anglerfishController, "_acceleration", _overrideAcceleration);
            updateParameter(anglerfishController, "_investigateSpeed", _overrideInvestigateSpeed);
            updateParameter(anglerfishController, "_chaseSpeed", _overrideChaseSpeed);
            updateParameter(anglerfishController, "_turnSpeed", _overrideTurnSpeed);
            updateParameter(anglerfishController, "_quickTurnSpeed", _overrideQuickTurnSpeed);
            updateParameter(anglerfishController, "_arrivalDistance", _overrideMouthOpenDistance);
            updateParameter(anglerfishController, "_pursueDistance", _overridePursueDistance);
            updateParameter(anglerfishController, "_escapeDistance", _overrideEscapeDistance);
        }

        private static OWRigidbody getPlayerBody()
        {
            if (PlayerState.IsInsideShip())
            {
                return Locator.GetShipBody();
            }
            else
            {
                return Locator.GetPlayerBody();
            }
        }

        private static void updateParameter(string id, float? parameter, float defaultValue)
        {
            foreach (AnglerfishController anglerfishController in anglerfish)
            {
                updateParameter(anglerfishController, id, parameter.GetValueOrDefault(defaultValue));
            }
        }

        private static void updateParameter(AnglerfishController anglerfishController, string id, float? parameter)
        {
            if (parameter != null)
            {
                anglerfishController.SetValue(id, parameter.Value);
            }
        }

        private static T getParameter<T>(string id, T defaultValue)
        {
            foreach (AnglerfishController anglerfishController in anglerfish)
            {
                return getParameter<T>(anglerfishController, id);
            }
            return defaultValue;
        }

        private static T getParameter<T>(AnglerfishController anglerfishController, string id)
        {
            return anglerfishController.GetValue<T>(id);
        }

        private static void SetVisible(AnglerfishController item, bool visible, bool collision)
        {
            SetVisibleBehaviour(item, visible);
            SetVisibleChild(item, visible, collision);
            //item.GetComponent<AnglerfishAnimController>().GetComponent<Animator>().enabled = true;
            /*foreach (Component sibling in item.GetComponents<Component>())
            {
                SetVisibleChild(sibling, visible, collision);
                foreach (Component niece in item.GetComponentsInChildren<Component>())
                {
                    SetVisibleChild(niece, visible, collision);
                }
            }*/
            foreach (Component child in item.GetComponentsInChildren<Component>())
            {
                SetVisibleChild(child, visible, collision);
                /*foreach (Component grandchild in child.GetComponentsInChildren<Component>())
                {
                    SetVisibleChild(grandchild, visible, collision);
                }*/
            }
            /*foreach (FogWarpDetector detector in GameObject.FindObjectsOfType<FogWarpDetector>())
            {
                SetVisibleBehaviour(detector, visible);
            }
            foreach (OuterFogWarpVolume volume in GameObject.FindObjectsOfType<OuterFogWarpVolume>())
            {
                SetVisibleBehaviour(volume, visible);
            }*/
        }

        private static void SetVisibleChild(Component item, bool visible, bool collision)
        {
            if (item is Behaviour)
            {
                SetVisibleBehaviour((Behaviour)item, visible);
            }
            else
            {
                SetVisibleComponent(item, visible);
            }
            foreach (OWCollider collider in item.GetComponents<OWCollider>())
            {
                SetVisibleBehaviour(collider, visible);
                collider.SetActivation(true);
                collider.SetLODLevel(collision ? 0 : 1, 0f);
                if (collider.GetCollider())
                {
                    collider.GetCollider().enabled = collision;
                    SetVisibleComponent(collider, visible);
                }
            }
            foreach (Collider collider in item.GetComponents<Collider>())
            {
                collider.enabled = collision;
                SetVisibleComponent(collider, visible);
            }
            foreach (OWRenderer render in item.GetComponents<OWRenderer>())
            {
                SetVisibleBehaviour(render, visible);
                render.SetActivation(true);
                render.SetLODActivation(visible);
                if (render.GetRenderer())
                {
                    render.GetRenderer().enabled = true;
                    SetVisibleComponent(render, visible);
                }
            }
            foreach (Renderer render in item.GetComponents<Renderer>())
            {
                render.enabled = true;
                SetVisibleComponent(render, visible);
            }
            foreach (ParticleSystem particleSystem in item.GetComponents<ParticleSystem>())
            {
                SetVisibleComponent(particleSystem, visible);
                if (visible)
                    particleSystem.Play(true);
                else
                    particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            foreach (OWLight2 light in item.GetComponents<OWLight2>())
            {
                SetVisibleBehaviour(light, visible);
                light.SetActivation(true);
                light.SetLODActivation(visible);
                if (light.GetLight())
                {
                    SetVisibleBehaviour(light.GetLight(), visible);
                }
            }
            foreach (Light light in item.GetComponents<Light>())
            {
                SetVisibleBehaviour(light, visible);
            }
            foreach(LightLOD light in item.GetComponents<LightLOD>())
            {
                SetVisibleBehaviour(light, visible);
            }
            foreach (FogLight light in item.GetComponents<FogLight>())
            {
                SetVisibleBehaviour(light, visible);
            }
        }


        private static void SetVisibleBehaviour(Behaviour item, bool visible)
        {
            item.enabled = true;
            SetVisibleComponent(item, visible);
        }

        private static void SetVisibleComponent(Component item, bool visible)
        {
            item.gameObject.SetActive(true);
            item.GetAttachedOWRigidbody().enabled = true;

            if (visible)
            {
                item.GetAttachedOWRigidbody().Unsuspend();
            }
            else
            {
                item.GetAttachedOWRigidbody().Suspend();
            }
        }
    }



    /*
     * anglerfishAnglerfish_Body (OWRigidbody)	Hard Mode	
    anglerfishAnglerfish_Body (AnglerfishController)	Hard Mode	
    anglerfishAnglerfish_Body (ImpactSensor)	Hard Mode	
    anglerfishAnglerfish_Body (NoiseSensor)	Hard Mode	
    anglerfishAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
    anglerfishAudioController (AnglerfishAudioController)	Hard Mode	
    anglerfishLoopSource (OWAudioSource)	Hard Mode
    anglerfishOneShotSource (OWAudioSource)	Hard Mode	
anglerfishOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfishJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfishJawsOfDestruction (OWCollider)	Hard Mode	
anglerfishJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfishBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfishLure_PointLight (LightLOD)	Hard Mode	
anglerfishLure_FogLight (FogLight)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfishBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfishBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode
    anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode
    anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode
    anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAnglerfish_Body (OWRigidbody)	Hard Mode	
anglerfish-childAnglerfish_Body (AnglerfishController)	Hard Mode	
anglerfish-childAnglerfish_Body (ImpactSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (NoiseSensor)	Hard Mode	
anglerfish-childAnglerfish_Body (CenterOfTheUniverseOffsetApplier)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childAudioController (AnglerfishAudioController)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	
anglerfish-childLoopSource (OWAudioSource)	Hard Mode	3
anglerfish-childOneShotSource (OWAudioSource)	Hard Mode	3
anglerfish-childOneShotSource_LongRange (OWAudioSource)	Hard Mode	3
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (AnglerfishFluidVolume)	Hard Mode	
anglerfish-childJawsOfDestruction (OWCollider)	Hard Mode	
anglerfish-childJawsOfDestruction (OWTriggerVolume)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childBeast_Anglerfish (AnglerfishAnimController)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	
anglerfish-childLure_FogLight (FogLight)	Hard Mode	
anglerfish-childLure_PointLight (LightLOD)	Hard Mode	3
anglerfish-childLure_FogLight (FogLight)	Hard Mode	2
anglerfish-childBeast_Anglerfish_Collider_Mouth (OWCollider)	Hard Mode	4
anglerfish-childBeast_Anglerfish (StreamingSkinnedMeshHandle)	Hard Mode	3
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	
anglerfish-childBeast_Anglerfish_Collider_Body (OWCollider)	Hard Mode	3
anglerfish-childBeast_Anglerfish_Collider_LeftCheek (OWCollider)	Hard Mode	3
anglerfish-childBeast_Anglerfish_Collider_MouthFloor (OWCollider)	Hard Mode	3
anglerfish-childBeast_Anglerfish_Collider_RightCheek (OWCollider)	Hard Mode	3
parentFogLight_FishEgg (FogLight)	Hard Mode

     */
}
