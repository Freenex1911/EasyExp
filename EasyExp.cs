﻿using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Freenex.EasyExp
{
    public class EasyExp : RocketPlugin
    {
        public static EasyExp Instance;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"experience_self","You gave yourself {0} experience."},
                    {"experience_general_not_found","Player not found."},
                    {"experience_give_player","{1} gave you {0} experience."},
                    {"experience_give_caller","You gave {1} {0} experience."},
                    {"experience_give_player_console","You've got {0} experience."},
                    {"experience_transfer_player","{1} gave you {0} experience."},
                    {"experience_transfer_caller","You gave {1} {0} experience."},
                    {"experience_transfer_not_enough","You don't have {0} experience."},
                    {"experience_onrevive","You received {0} experience."},
                    {"experience_onkill_true","You killed {0} and received {1} experience."},
                    {"experience_onkill_false","You killed {0} but received no experience."},
                    {"experience_deleteondeath","You lost all your experience."}
                };
            }
        }

        protected override void Load()
        {
            Instance = this;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerRevive += UnturnedPlayerEvents_OnPlayerRevive;
            Logger.Log("Freenex's EasyExp has been loaded!");
        }

        protected override void Unload()
        {
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerRevive -= UnturnedPlayerEvents_OnPlayerRevive;
            Logger.Log("Freenex's EasyExp has been unloaded!");
        }

        void UnturnedPlayerEvents_OnPlayerRevive(Rocket.Unturned.Player.UnturnedPlayer player, Vector3 position, byte revive)
        {
            foreach (Rocket.API.Serialisation.Permission playerPermission in player.GetPermissions())
            {
                if (playerPermission.Name.ToLower().Contains("exp.onrevive."))
                {
                    uint permissionExp;
                    bool isNumeric = uint.TryParse(playerPermission.Name.ToLower().Replace("exp.onrevive.", string.Empty), out permissionExp);
                    if (isNumeric)
                    {
                        player.Experience = player.Experience + permissionExp;
                        if (EasyExp.Instance.Translations.Instance.Translate("experience_onrevive") != "experience_onrevive")
                        {
                            UnturnedChat.Say(player, EasyExp.Instance.Translations.Instance.Translate("experience_onrevive", permissionExp), Color.yellow);
                        }
                    }
                    else { Logger.LogError(playerPermission + " is not numeric."); }
                }
            }
        }

        void UnturnedPlayerEvents_OnPlayerDeath(Rocket.Unturned.Player.UnturnedPlayer player, SDG.Unturned.EDeathCause cause, SDG.Unturned.ELimb limb, Steamworks.CSteamID murderer)
        {
            if (player.HasPermission("exp.deleteondeath") && !player.IsAdmin)
            {
                player.Experience = 0;
                if (EasyExp.Instance.Translations.Instance.Translate("experience_deleteondeath") != "experience_deleteondeath")
                {
                    UnturnedChat.Say(player, EasyExp.Instance.Translations.Instance.Translate("experience_deleteondeath"), Color.yellow);
                }
            }

            UnturnedPlayer UPmurderer = UnturnedPlayer.FromCSteamID(murderer);
            try
            {
                if (player.Id == UPmurderer.Id) { return; }
            }
            catch { return; }

            foreach (Rocket.API.Serialisation.Permission playerPermission in UPmurderer.GetPermissions())
            {
                if (playerPermission.Name.ToLower().Contains("exp.onkill."))
                {
                    string[] OnKillPermission = playerPermission.Name.ToLower().Replace("exp.onkill.", string.Empty).Split('.');

                    uint OnKillExperience;
                    uint OnKillPercentage = 100;
                    bool isExperienceNumeric = uint.TryParse(OnKillPermission[0], out OnKillExperience);
                    if (!isExperienceNumeric) { Logger.LogError(OnKillPermission[0] + " is not numeric."); }

                    if (OnKillPermission.Length == 2)
                    {
                        bool isPercentageNumeric = uint.TryParse(OnKillPermission[1], out OnKillPercentage);
                        if (!isPercentageNumeric) { Logger.LogError(OnKillPermission[1] + " is not numeric."); }
                    }

                    System.Random rand = new System.Random();
                    int chance = rand.Next(1, 101);

                    if (chance <= OnKillPercentage)
                    {
                        UPmurderer.Experience = UPmurderer.Experience + OnKillExperience;
                        if (EasyExp.Instance.Translations.Instance.Translate("experience_onkill_true") != "experience_onkill_true")
                        {
                            UnturnedChat.Say(UPmurderer, EasyExp.Instance.Translations.Instance.Translate("experience_onkill_true", player.DisplayName, OnKillExperience), Color.yellow);
                        }
                    }
                    else
                    {
                        if (EasyExp.Instance.Translations.Instance.Translate("experience_onkill_false") != "experience_onkill_false")
                        {
                            UnturnedChat.Say(UPmurderer, EasyExp.Instance.Translations.Instance.Translate("experience_onkill_false", player.DisplayName), Color.yellow);
                        }
                    }
                }
            }
        }
    }
}
