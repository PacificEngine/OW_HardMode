using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace ClassLibrary2
{
    static class Anglerfish
    {
        private static bool _enabledAI = true;
        private static AnglerfishController.AnglerState _minimumState = AnglerfishController.AnglerState.Stunned;
        private static float _acceleration = 2f;
        private static float _investigateSpeed = 20f;
        private static float _chaseSpeed = 42f;
        private static float _turnSpeed = 180f;
        private static float _quickTurnSpeed = 360f;
        private static float _mouthOpenDistance = 100f;
        private static float _pursueDistance = 200f;
        private static float _escapeDistance = 500f;

        public static bool enabledAI { get { return _enabledAI; } set { _enabledAI = value; updateAnglerfish(); } }
        public static AnglerfishController.AnglerState minimumState { get { return _minimumState; } set { _minimumState = value; updateAnglerfish(); } }
        public static float acceleration { get { return _acceleration; } set { _acceleration = value; updateAnglerfish(); } }
        public static float investigateSpeed { get { return _investigateSpeed; } set { _investigateSpeed = value; updateAnglerfish(); } }
        public static float chaseSpeed { get { return _chaseSpeed; } set { _chaseSpeed = value; updateAnglerfish(); } }
        public static float turnSpeed { get { return _turnSpeed; } set { _turnSpeed = value; updateAnglerfish(); } }
        public static float quickTurnSpeed { get { return _quickTurnSpeed; } set { _quickTurnSpeed = value; updateAnglerfish(); } }
        public static float mouthOpenDistance { get { return _mouthOpenDistance; } set { _mouthOpenDistance = value; updateAnglerfish(); } }
        public static float pursueDistance { get { return _pursueDistance; } set { _pursueDistance = value; updateAnglerfish(); } }
        public static float escapeDistance { get { return _escapeDistance; } set { _escapeDistance = value; updateAnglerfish(); } }

        public static void Start()
        {
        }

        public static void Awake()
        {
            foreach (Sector sector in SectorManager.GetRegisteredSectors())
            {
                if (Sector.Name.BrambleDimension.Equals(sector.GetName()))
                {
                    sector.OnSectorOccupantsUpdated.AddListener(() => updateAnglerfish(UnityEngine.Object.FindObjectsOfType<AnglerfishController>()));
                }
            }
        }

        public static void Destory()
        {
        }

        public static void Update()
        {
        }

        public static void onEnter()
        {
        }

        public static void onLeave()
        {
        }

        private static void updateAnglerfish(params AnglerfishController[] anglerfishControllers)
        {
            foreach (AnglerfishController anglerfishController in anglerfishControllers)
            {
                updateAnglerfish(anglerfishController, anglerfishController.GetValue<AnglerfishController.AnglerState>("_currentState"));
                anglerfishController.OnAnglerUnsuspended += (state) => updateAnglerfish(anglerfishController, state);
                anglerfishController.OnChangeAnglerState += (state) => updateAnglerfish(anglerfishController, state);
            }
        }

        private static void updateAnglerfish(AnglerfishController anglerfishController, AnglerfishController.AnglerState state)
        {
            //anglerfishController.enabled = enabledAI;
            anglerfishController.SetValue("_acceleration", acceleration);
            anglerfishController.SetValue("_investigateSpeed", investigateSpeed);
            anglerfishController.SetValue("_chaseSpeed", chaseSpeed);
            anglerfishController.SetValue("_turnSpeed", turnSpeed);
            anglerfishController.SetValue("_quickTurnSpeed", quickTurnSpeed);
            anglerfishController.SetValue("_arrivalDistance", mouthOpenDistance);
            anglerfishController.SetValue("_pursueDistance", pursueDistance);
            anglerfishController.SetValue("_escapeDistance", escapeDistance);

            updateAnglerfishState(anglerfishController, anglerfishController.GetValue<AnglerfishController.AnglerState>("_currentState"));
        }

        private static void updateAnglerfishState(AnglerfishController anglerfishController, AnglerfishController.AnglerState state)
        {
            if (minimumState == AnglerfishController.AnglerState.Stunned)
            {
                return;
            }
            else if (state == AnglerfishController.AnglerState.Stunned)
            {
                setAnglerfishState(anglerfishController, minimumState);
            }
            else if (minimumState == AnglerfishController.AnglerState.Lurking)
            {
                return;
            }
            else if (state == AnglerfishController.AnglerState.Lurking)
            {
                setAnglerfishState(anglerfishController, minimumState);
            }
            else if (minimumState == AnglerfishController.AnglerState.Investigating)
            {
                return;
            }
            else if (state == AnglerfishController.AnglerState.Investigating)
            {
                setAnglerfishState(anglerfishController, minimumState);
            }
        }

        private static void setAnglerfishState(AnglerfishController anglerfishController, AnglerfishController.AnglerState state)
        {
            if (state == AnglerfishController.AnglerState.Investigating)
            {
                var bramble = anglerfishController.GetValue<OWRigidbody>("_brambleBody");
                anglerfishController.SetValue("_localDisturbancePos", bramble.transform.TransformPoint(Locator.GetPlayerBody().GetPosition()));
            }

            if (state == AnglerfishController.AnglerState.Chasing)
            {
                anglerfishController.SetValue("_targetBody", Locator.GetPlayerBody());
            }
            anglerfishController.SetValue("_currentState", state);
        }
    }
}
