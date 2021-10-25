using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClassLibrary2
{
    public class MainClass : ModBehaviour
    {
        bool isEnabled = true;
        bool disableShip = false;
        float loopLength = 1320f;
        float damageMultiplier = 1f;
        float maximumFuel = 0f;
        float maximumOxygen = 10f;
        float anglerSpeedMultiplier = 1f;
        float anglerDetectMultiplier = 1f;
        float _lastPlayerHealth;
        float _lastSuitHealth;

        void Start()
        {
            if (isEnabled)
            {
                ModHelper.Events.Player.OnPlayerAwake += (body) => onAwake();
                ModHelper.Console.WriteLine("Hard Mode Enabled!");
            }
        }

        public override void Configure(IModConfig config)
        {
            isEnabled = config.Enabled;

            loopLength = config.GetSettingsValue<float>("Loop Duration");
            damageMultiplier = config.GetSettingsValue<float>("Damage Multiplier");
            maximumFuel = config.GetSettingsValue<float>("Fuel Percentage");
            maximumOxygen = config.GetSettingsValue<float>("Oxygen Percentage");
            disableShip = config.GetSettingsValue<bool>("Disable Ship");
            anglerSpeedMultiplier = config.GetSettingsValue<float>("Anglerfish Speed Multiplier");
            anglerDetectMultiplier = config.GetSettingsValue<float>("Anglerfish Detection Multiplier");

            updateAnglerFish(anglerSpeedMultiplier, anglerDetectMultiplier);

            ModHelper.Console.WriteLine("Hard Mode Configured!");
        }

        void OnGUI()
        {
        }

        private void onAwake()
        {
            updateAnglerFish(anglerSpeedMultiplier, anglerDetectMultiplier);
        }

        void Update()
        {
            updateLoopDuration(loopLength);
            updateHealth(damageMultiplier);
            setMaximumFuel(maximumFuel);
            setMaximumOxygen(maximumOxygen);
            if (disableShip && Locator.GetShipTransform() && Locator.GetShipTransform().GetComponent<ShipResources>())
            {
                Locator.GetShipTransform().GetComponent<ShipResources>().SetFuel(0f);
            }
        }

        private void updateLoopDuration(float duration)
        {
            if (TimeLoop.GetSecondsRemaining() > duration)
            {
                TimeLoop.SetSecondsRemaining(duration);
            }
        }

        private void updateAnglerFish(float speedMultiplier, float detectMultiplier)
        {
            foreach (AnglerfishController anglerfishController in UnityEngine.Object.FindObjectsOfType<AnglerfishController>())
            {
                anglerfishController.SetValue("_arrivalDistance", 100f * detectMultiplier);
                anglerfishController.SetValue("_pursueDistance", 200f * detectMultiplier);
                anglerfishController.SetValue("_escapeDistance", 500f * detectMultiplier);

                anglerfishController.SetValue("_acceleration", 2f * speedMultiplier);
                anglerfishController.SetValue("_investigateSpeed", 20f * speedMultiplier);
                anglerfishController.SetValue("_chaseSpeed", 42f * speedMultiplier);
                anglerfishController.SetValue("_turnSpeed", 100f * speedMultiplier);
                anglerfishController.SetValue("_quickTurnSpeed", 360f * speedMultiplier);
            }
        }

        private void updateHealth(float damageMultiplier)
        {
            if (Locator.GetPlayerTransform() && Locator.GetPlayerTransform().GetComponent<PlayerResources>())
            {
                var resources = Locator.GetPlayerTransform().GetComponent<PlayerResources>();
                var damage = _lastPlayerHealth - resources.GetHealth();
                if (damage > 0)
                {
                    _lastPlayerHealth = _lastPlayerHealth - (damageMultiplier * damage);
                    resources.SetValue("_currentHealth", _lastPlayerHealth);
                }
                else
                {
                    _lastPlayerHealth = Locator.GetPlayerTransform().GetComponent<PlayerResources>().GetHealth();
                }

                damage = resources.GetValue<int>("_currentNumPunctures") - _lastSuitHealth;
                if (damage > 0)
                {
                    _lastSuitHealth = _lastSuitHealth + (damageMultiplier * damage);
                    resources.SetValue("_lastSuitHealth", (int)_lastSuitHealth);
                }
                else
                {
                    _lastSuitHealth = resources.GetValue<int>("_currentNumPunctures");
                }
            }
            if (Locator.GetShipTransform() && Locator.GetShipTransform().GetComponent<ShipDamageController>())
            {
                var resources = Locator.GetShipTransform().GetComponent<ShipResources>();
                // TODO
            }
        }

        private void setMaximumFuel(float maximumFuel)
        {
            if (Locator.GetPlayerTransform() && Locator.GetPlayerTransform().GetComponent<PlayerResources>())
            {
                var resources = Locator.GetPlayerTransform().GetComponent<PlayerResources>();
                var playerFuel = resources.GetFuel();
                if (playerFuel > maximumFuel)
                {
                    if (Locator.GetPlayerTransform().GetComponent<PlayerResources>().IsRefueling())
                    {
                        Locator.GetPlayerTransform().GetComponent<PlayerResources>().StopRefillResources();
                    }
                    resources.SetValue("_currentFuel", maximumFuel);
                }
            }
            if (Locator.GetShipTransform() && Locator.GetShipTransform().GetComponent<ShipResources>())
            {
                var resources = Locator.GetShipTransform().GetComponent<ShipResources>();
                var shipFuel = resources.GetFuel();
                if (shipFuel > maximumFuel * 100f)
                {
                    resources.SetFuel(maximumFuel * 100f);
                }
            }
        }

        private void setMaximumOxygen(float maximumOxygen)
        {
            if (Locator.GetPlayerTransform() && Locator.GetPlayerTransform().GetComponent<PlayerResources>())
            {
                var resources = Locator.GetPlayerTransform().GetComponent<PlayerResources>();
                var playerOxygen = resources.GetValue<float>("_currentOxygen");
                if (!resources.IsOxygenPresent() && playerOxygen > (maximumOxygen * 4.5f))
                {
                    if (resources.IsRefillingOxygen())
                    {
                        resources.SetValue("_refillingOxygen", false);
                    }
                    resources.SetValue("_currentOxygen", maximumOxygen * 4.5f);
                }
            }
            if (Locator.GetShipTransform() && Locator.GetShipTransform().GetComponent<ShipResources>())
            {
                var resources = Locator.GetShipTransform().GetComponent<ShipResources>();
                var shipOxygen = resources.GetOxygen();
                if (shipOxygen > maximumOxygen * 60f)
                {
                    resources.SetOxygen(maximumOxygen * 60f);
                }
            }
        }
    }
}
