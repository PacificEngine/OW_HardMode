using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClassLibrary2
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
                Anglerfish.Start();
                ModHelper.Console.WriteLine("Hard Mode: ready!");
            }
        }

        void Destory()
        {
            Anglerfish.Destory();
            ModHelper.Console.WriteLine("Hard Mode: clean up!");
        }

        private T getConfigOrDefault<T>(IModConfig config, string id, T defaultValue)
        {
            try
            {
                var sValue = config.GetSettingsValue<T>(id);
                if (sValue == null)
                {
                    throw new NullReferenceException(id);
                }
                if (sValue is string && ((string)(object)sValue).Length < 1)
                {
                    throw new NullReferenceException(id);
                }
                return sValue;
            }
            catch (Exception e)
            {
                config.SetSettingsValue(id, defaultValue);
                return defaultValue;
            };
        }

        public override void Configure(IModConfig config)
        {
            isEnabled = config.Enabled;

            SuperNova.maxDuration = getConfigOrDefault<float>(config, "Loop Duration", 1320f);
            damageMultiplier = getConfigOrDefault<float>(config, "Damage Multiplier", 1f);
            Player.maxFuelSeconds = getConfigOrDefault<float>(config, "Fuel Percentage", 100f);
            Ship.maxFuelSeconds = Player.maxFuelSeconds * 100f;
            Player.maxOxygenSeconds = getConfigOrDefault<float>(config, "Oxygen Percentage", 100f) * 45f;
            Ship.maxOxygenSeconds = Player.maxOxygenSeconds * (600f / 45f);
            Ship.maxFuelSeconds = getConfigOrDefault<bool>(config, "Disable Ship", false) ? 0f : Ship.maxFuelSeconds;
            var anglerSpeedMultiplier = getConfigOrDefault<float>(config, "Anglerfish Speed Multiplier", 1f);
            var anglerDetectMultiplier = getConfigOrDefault<float>(config, "Anglerfish Detection Multiplier", 1f);

            updateAnglerFish(anglerSpeedMultiplier, anglerDetectMultiplier);

            ModHelper.Console.WriteLine("Hard Mode: Configured!");
        }

        void OnGUI()
        {
        }

        void onAwake()
        {
            Anglerfish.Awake();

            ModHelper.Console.WriteLine("Hard Mode: Player Awakes");
        }

        void Update()
        {
            if (isEnabled)
            {
                Player.Update();
                Ship.Update();
                Anglerfish.Update();
                SuperNova.Update();

                updateHealth(damageMultiplier);
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
