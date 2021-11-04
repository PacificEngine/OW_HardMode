using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PacificEngine.OW_CommonResources;

namespace PacificEngine.OW_HardMode
{
    public class MainClass : ModBehaviour
    {
        bool isEnabled = true;
        float damageMultiplier = 1f;
        float _lastPlayerHealth = 100f;
        int _lastSuitHealth = 0;

        void Start()
        {
            if (isEnabled)
            {
                ModHelper.Events.Player.OnPlayerAwake += (player) => onAwake();
                ModHelper.Console.WriteLine("Hard Mode: ready!");
            }
        }

        void Destory()
        {
            ModHelper.Console.WriteLine("Hard Mode: clean up!");
        }

        public override void Configure(IModConfig config)
        {
            isEnabled = config.Enabled;

            if (!isEnabled)
            {
                SuperNova.maximum = 1320f;
                damageMultiplier = 1f;
                Player.maxFuelSeconds = 100f;
                Ship.maxFuelSeconds = Player.maxFuelSeconds * 100f;
                Player.maxOxygenSeconds = 450f;
                Ship.maxOxygenSeconds = Player.maxOxygenSeconds * (600f / 45f);

                Anglerfish.canStun = true;
                Anglerfish.canSmell = false;
                Anglerfish.canSee = false;

                var anglerSpeedMultiplier = 1f;
                var anglerDetectMultiplier = 1f;
                updateAnglerFish(anglerFishSpeedAdjustment(anglerSpeedMultiplier), anglerFishDistanceAdjustment(anglerDetectMultiplier));
            }
            else
            {
                SuperNova.maximum = Config.getConfigOrDefault<float>(config, "Loop Duration", 1320f);
                damageMultiplier = Config.getConfigOrDefault<float>(config, "Damage Multiplier", 1f);
                Player.maxFuelSeconds = Config.getConfigOrDefault<float>(config, "Fuel Percentage", 100f);
                Ship.maxFuelSeconds = Player.maxFuelSeconds * 100f;
                Player.maxOxygenSeconds = Config.getConfigOrDefault<float>(config, "Oxygen Percentage", 100f) * 4.5f;
                Ship.maxOxygenSeconds = Player.maxOxygenSeconds * (600f / 45f);
                Ship.maxFuelSeconds = Config.getConfigOrDefault<bool>(config, "Disable Ship", false) ? 0f : Ship.maxFuelSeconds;

                Anglerfish.canStun = Config.getConfigOrDefault<bool>(config, "Anglerfish Can Forget", true);
                Anglerfish.canSmell = Config.getConfigOrDefault<bool>(config, "Anglerfish Can Smell", false);
                Anglerfish.canSee = Config.getConfigOrDefault<bool>(config, "Anglerfish Can See", false);

                var anglerSpeedMultiplier = Config.getConfigOrDefault<float>(config, "Anglerfish Speed Multiplier", 1f);
                var anglerDetectMultiplier = Config.getConfigOrDefault<float>(config, "Anglerfish Detection Multiplier", 1f);
                updateAnglerFish(anglerFishSpeedAdjustment(anglerSpeedMultiplier), anglerFishDistanceAdjustment(anglerDetectMultiplier));
            }


            ModHelper.Console.WriteLine("Hard Mode: Configured!");
        }

        void OnGUI()
        {
        }

        void onAwake()
        {
            ModHelper.Console.WriteLine("Hard Mode: Player Awakes");
        }

        void Update()
        {
            if (isEnabled)
            {
                updateHealth(damageMultiplier);
            }
        }

        private float anglerFishSpeedAdjustment(float anglerSpeedMultiplier)
        {
            if (anglerSpeedMultiplier <= 5f)
            {
                return anglerSpeedMultiplier; // 1, 2, 3, 4, 5
            }
            else if (anglerSpeedMultiplier <= 6f)
            {
                return anglerSpeedMultiplier * 1.17f; // 7
            }
            else if (anglerSpeedMultiplier <= 7f)
            {
                return anglerSpeedMultiplier * 1.42f; // 10
            }
            else if (anglerSpeedMultiplier <= 8f)
            {
                return anglerSpeedMultiplier * 1.7f; // 14
            }
            else if (anglerSpeedMultiplier <= 9f)
            {
                return anglerSpeedMultiplier * 2f; // 18
            }
            else if (anglerSpeedMultiplier <= 10f)
            {
                return anglerSpeedMultiplier * 2.5f; // 25
            }
            else
            {
                return anglerSpeedMultiplier * 10f; // 110
            }
        }

        private float anglerFishDistanceAdjustment(float anglerDetectMultiplier)
        {
            if (anglerDetectMultiplier <= 3f)
            {
                return anglerDetectMultiplier; // 1, 2, 3
            }
            else if (anglerDetectMultiplier <= 4f)
            {
                return anglerDetectMultiplier * 1.17f; // 5
            }
            else if (anglerDetectMultiplier <= 5f)
            {
                return anglerDetectMultiplier * 2f; // 10
            }
            else if (anglerDetectMultiplier <= 6f)
            {
                return anglerDetectMultiplier * 4f; // 25
            }
            else if (anglerDetectMultiplier <= 7f)
            {
                return anglerDetectMultiplier * 7f; // 50
            }
            else if (anglerDetectMultiplier <= 8f)
            {
                return anglerDetectMultiplier * 12.5f; // 100
            }
            else if (anglerDetectMultiplier <= 9f)
            {
                return anglerDetectMultiplier * 50f; // 450
            }
            else if (anglerDetectMultiplier <= 10f)
            {
                return anglerDetectMultiplier * 100f; // 1000
            }
            else
            {
                return anglerDetectMultiplier * 1000f; // 11000
            }
        }

        private void updateAnglerFish(float speedMultiplier, float detectMultiplier)
        {
            Anglerfish.mouthOpenDistance = 100f * detectMultiplier;
            Anglerfish.pursueDistance = 200f * detectMultiplier;
            Anglerfish.escapeDistance = 500f * detectMultiplier;

            Anglerfish.acceleration = 2f * speedMultiplier;
            Anglerfish.investigateSpeed = 20f * speedMultiplier;
            Anglerfish.chaseSpeed = 42f * speedMultiplier;
            Anglerfish.turnSpeed = 100f * speedMultiplier;
            Anglerfish.quickTurnSpeed = 360f * speedMultiplier;

            Anglerfish.visionDistance = 200f * detectMultiplier;
            Anglerfish.smellDistance = 500f * detectMultiplier;
        }

        private void updateHealth(float damageMultiplier)
        {
            var damage = _lastPlayerHealth - Player.health;
            if (damage > 0)
            {
                _lastPlayerHealth = _lastPlayerHealth - (damageMultiplier * damage);
                Player.health = _lastPlayerHealth;
            }
            else if (damage < 0)
            {
                _lastPlayerHealth = Player.health;
            }

            var punctures = Player.suitPunctureCount - _lastSuitHealth;
            if (punctures > 0)
            {
                _lastSuitHealth = _lastSuitHealth + (int)(damageMultiplier * punctures);
                Player.suitPunctureCount = _lastSuitHealth;
            }
            else if (punctures < 0)
            {
                _lastSuitHealth = Player.suitPunctureCount;
            }
            // TODO ship damagae multiplier
        }
    }
}
