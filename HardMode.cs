using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PacificEngine.OW_CommonResources.Game.State;
using PacificEngine.OW_CommonResources.Game.Player;
using PacificEngine.OW_CommonResources;
using PacificEngine.OW_CommonResources.Config;

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

            var difficulty = ConfigHelper.getConfigOrDefault<String>(config, "DifficultySettings", "Hard");
            if (!isEnabled)
            {
                difficulty = "Medium";
            }

            if ("Peaceful".Equals(difficulty))
            {
                SuperNova.maximum = 2640f;
                damageMultiplier = 0.00001f;
                Player.maxBoostSeconds = 10f;
                Player.maxFuelSeconds = 1000f;
                Player.maxOxygenSeconds = 4500f;
                Ship.maxFuelSeconds = 100000f;
                Ship.maxOxygenSeconds = 60000f;

                Anglerfish.canStun = true;
                Anglerfish.canFeel = false;
                Anglerfish.canHear = false;
                Anglerfish.canSmell = false;
                Anglerfish.canSee = false;
                Anglerfish.mouthOpenDistance = 0f;
                Anglerfish.pursueDistance = 0f;
                Anglerfish.escapeDistance = 0f;
                Anglerfish.acceleration = 2f;
                Anglerfish.investigateSpeed = 0f;
                Anglerfish.chaseSpeed = 0f;
                Anglerfish.turnSpeed = 0f;
                Anglerfish.quickTurnSpeed = 0f;
                Anglerfish.smellDistance = 0f;
                Anglerfish.visionDistance = 0f;

                Inhabitants.enabledHostility = false;
            }
            else if ("Easy".Equals(difficulty))
            {
                SuperNova.maximum = 1620f;
                damageMultiplier = 0.1f;
                Player.maxBoostSeconds = 5f;
                Player.maxFuelSeconds = 250f;
                Player.maxOxygenSeconds = 450f;
                Ship.maxFuelSeconds = 10000f;
                Ship.maxOxygenSeconds = 6000f;

                Anglerfish.canStun = true;
                Anglerfish.canFeel = true;
                Anglerfish.canHear = true;
                Anglerfish.canSmell = false;
                Anglerfish.canSee = false;
                Anglerfish.mouthOpenDistance = 100f;
                Anglerfish.pursueDistance = 150f;
                Anglerfish.escapeDistance = 300f;
                Anglerfish.acceleration = 1f;
                Anglerfish.investigateSpeed = 10f;
                Anglerfish.chaseSpeed = 21f;
                Anglerfish.turnSpeed = 100f;
                Anglerfish.quickTurnSpeed = 360f;
                Anglerfish.smellDistance = 0f;
                Anglerfish.visionDistance = 0f;

                Inhabitants.enabledHostility = true;
            }
            else if ("Standard".Equals(difficulty))
            {
                SuperNova.maximum = 1320f;
                damageMultiplier = 0.1f;
                Player.maxBoostSeconds = 5f;
                Player.maxFuelSeconds = 250f;
                Player.maxOxygenSeconds = 450f;
                Ship.maxFuelSeconds = 10000f;
                Ship.maxOxygenSeconds = 6000f;

                Anglerfish.canStun = true;
                Anglerfish.canFeel = true;
                Anglerfish.canHear = true;
                Anglerfish.canSmell = false;
                Anglerfish.canSee = false;
                Anglerfish.mouthOpenDistance = 100f;
                Anglerfish.pursueDistance = 200f;
                Anglerfish.escapeDistance = 500f;
                Anglerfish.acceleration = 2f;
                Anglerfish.investigateSpeed = 20f;
                Anglerfish.chaseSpeed = 42f;
                Anglerfish.turnSpeed = 100f;
                Anglerfish.quickTurnSpeed = 360f;
                Anglerfish.smellDistance = 0f;
                Anglerfish.visionDistance = 0f;

                Inhabitants.enabledHostility = true;
            }
            else if ("Hard".Equals(difficulty))
            {
                SuperNova.maximum = 1020f;
                damageMultiplier = 2f;
                Player.maxBoostSeconds = 1f;
                Player.maxFuelSeconds = 50f;
                Player.maxOxygenSeconds = 200f;
                Ship.maxFuelSeconds = 1000f;
                Ship.maxOxygenSeconds = 600f;

                Anglerfish.canStun = true;
                Anglerfish.canFeel = true;
                Anglerfish.canHear = true;
                Anglerfish.canSmell = false;
                Anglerfish.canSee = false;
                Anglerfish.mouthOpenDistance = 300f;
                Anglerfish.pursueDistance = 600f;
                Anglerfish.escapeDistance = 1500f;
                Anglerfish.acceleration = 6f;
                Anglerfish.investigateSpeed = 50f;
                Anglerfish.chaseSpeed = 110f;
                Anglerfish.turnSpeed = 100f;
                Anglerfish.quickTurnSpeed = 360f;
                Anglerfish.smellDistance = 0f;
                Anglerfish.visionDistance = 0f;

                Inhabitants.enabledHostility = true;
            }
            else if ("Ridiculous".Equals(difficulty))
            {
                SuperNova.maximum = 600f;
                damageMultiplier = 4f;
                Player.maxBoostSeconds = 0.5f;
                Player.maxFuelSeconds = 25f;
                Player.maxOxygenSeconds = 150f;
                Ship.maxFuelSeconds = 1000f;
                Ship.maxOxygenSeconds = 600f;

                Anglerfish.canStun = true;
                Anglerfish.canFeel = true;
                Anglerfish.canHear = true;
                Anglerfish.canSmell = true;
                Anglerfish.canSee = false;
                Anglerfish.mouthOpenDistance = 300f;
                Anglerfish.pursueDistance = 1200f;
                Anglerfish.escapeDistance = 3000f;
                Anglerfish.acceleration = 6f;
                Anglerfish.investigateSpeed = 50f;
                Anglerfish.chaseSpeed = 110f;
                Anglerfish.turnSpeed = 100f;
                Anglerfish.quickTurnSpeed = 360f;
                Anglerfish.smellDistance = 1000f;
                Anglerfish.visionDistance = 0f;

                Inhabitants.enabledHostility = true;
            }
            else if ("Improbable".Equals(difficulty))
            {
                SuperNova.maximum = 450f;
                damageMultiplier = 10f;
                Player.maxBoostSeconds = 0.1f;
                Player.maxFuelSeconds = 10f;
                Player.maxOxygenSeconds = 100f;
                Ship.maxFuelSeconds = 500f;
                Ship.maxOxygenSeconds = 300f;

                Anglerfish.canStun = true;
                Anglerfish.canFeel = true;
                Anglerfish.canHear = true;
                Anglerfish.canSmell = true;
                Anglerfish.canSee = true;
                Anglerfish.mouthOpenDistance = 300f;
                Anglerfish.pursueDistance = 2400f;
                Anglerfish.escapeDistance = 6000f;
                Anglerfish.acceleration = 20f;
                Anglerfish.investigateSpeed = 100f;
                Anglerfish.chaseSpeed = 200f;
                Anglerfish.turnSpeed = 100f;
                Anglerfish.quickTurnSpeed = 360f;
                Anglerfish.smellDistance = 600f;
                Anglerfish.visionDistance = 1000f;

                Inhabitants.enabledHostility = true;
            }
            else
            {
                SuperNova.maximum = ConfigHelper.getConfigOrDefault<float>(config, "LoopDuration", 1320f);
                damageMultiplier = ConfigHelper.getConfigOrDefault<float>(config, "DamageMultiplier", 1320f);
                Player.maxBoostSeconds = ConfigHelper.getConfigOrDefault<float>(config, "BoostSeconds", 1f);
                Player.maxFuelSeconds = ConfigHelper.getConfigOrDefault<float>(config, "JetPackFuel", 100f);
                Player.maxOxygenSeconds = ConfigHelper.getConfigOrDefault<float>(config, "JetPackFuel", 100f);
                Ship.maxFuelSeconds = ConfigHelper.getConfigOrDefault<bool>(config, "DisableShip", false) ? ConfigHelper.getConfigOrDefault<float>(config, "ShipFuel", 10000f) : 0f;
                Ship.maxOxygenSeconds = ConfigHelper.getConfigOrDefault<float>(config, "ShipOxygen", 6000f);

                Anglerfish.pursueDistance = ConfigHelper.getConfigOrDefault<float>(config, "AnglerfishHearingRange", 200f);
                Anglerfish.escapeDistance = ConfigHelper.getConfigOrDefault<float>(config, "AnglerfishFeelingRange", 500f);
                Anglerfish.acceleration = ConfigHelper.getConfigOrDefault<float>(config, "AnglerfisAcceleration", 2f);
                Anglerfish.chaseSpeed = ConfigHelper.getConfigOrDefault<float>(config, "AnglerfisChaseSpeed", 42f);
                Anglerfish.turnSpeed = 100f;
                Anglerfish.quickTurnSpeed = 360f;
                Anglerfish.smellDistance = ConfigHelper.getConfigOrDefault<float>(config, "AnglerfisSmellingRange", 0f);
                Anglerfish.visionDistance = ConfigHelper.getConfigOrDefault<float>(config, "AnglerfisSeeingRange", 0f);

                Anglerfish.canStun = ConfigHelper.getConfigOrDefault<bool>(config, "AnglerfishAreLikeGoldfish", true);
                Anglerfish.canFeel = Anglerfish.pursueDistance > 1f;
                Anglerfish.canHear = Anglerfish.escapeDistance > 1f;
                Anglerfish.canSmell = Anglerfish.smellDistance > 1f;
                Anglerfish.canSee = Anglerfish.visionDistance > 1f;
                Anglerfish.mouthOpenDistance = Anglerfish.pursueDistance / 2f;
                Anglerfish.investigateSpeed = Anglerfish.chaseSpeed / 2f;

                Anglerfish.escapeDistance = Mathf.Max(Anglerfish.escapeDistance.Value, Anglerfish.smellDistance.Value, Anglerfish.visionDistance.Value);
                Anglerfish.pursueDistance = Mathf.Max(Anglerfish.pursueDistance.Value, Anglerfish.smellDistance.Value, Anglerfish.visionDistance.Value);

                Inhabitants.enabledHostility = ConfigHelper.getConfigOrDefault<bool>(config, "InhabitantsCanAttackPlayer", true);
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
