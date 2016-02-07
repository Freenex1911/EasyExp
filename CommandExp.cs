﻿using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Freenex.FeexExp
{
    public class CommandExp : IRocketCommand
    {
        public string Name
        {
            get { return "exp"; }
        }

        public string Help
        {
            get { return "Give or transfer Experience"; }
        }

        public string Syntax
        {
            get { return "<experience> [<player>]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                {
                    "exp.self",
                    "exp.give",
                    "exp.transfer"
                };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0 || command.Length > 2)
            {
                return;
            }

            if (command.Length == 1 && !(caller is ConsolePlayer))
            {
                UnturnedPlayer UPcaller = (UnturnedPlayer)caller;
                if (!(caller.HasPermission("exp.self"))) { return; }
                uint commandExp;
                bool isNumeric = uint.TryParse(command[0], out commandExp);
                if (isNumeric)
                {
                    UPcaller.Experience = UPcaller.Experience + commandExp;
                    if (FeexExp.Instance.Translations.Instance.Translate("experience_self") != "experience_self")
                    {
                        UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("experience_self", commandExp), Color.yellow);
                    }
                }
            }

            if (command.Length == 2)
            {
                if (!caller.HasPermission("exp.give") && !caller.HasPermission("exp.transfer") && !(caller is ConsolePlayer)) { return; }
                
                uint commandExp;
                bool isNumeric = uint.TryParse(command[0], out commandExp);
                if (!isNumeric) { return; }

                UnturnedPlayer player = UnturnedPlayer.FromName(command[1]);
                if (player == null)
                {
                    if (FeexExp.Instance.Translations.Instance.Translate("experience_general_not_found") != "experience_general_not_found")
                    {
                        if (caller is ConsolePlayer)
                        {
                            Logger.Log(FeexExp.Instance.Translations.Instance.Translate("experience_general_not_found"));
                        }
                        else
                        {
                            UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("experience_general_not_found"), Color.yellow);
                        }
                    }
                    return;
                }

                if (player.Id == caller.Id) { return; }

                if (caller.HasPermission("exp.give") || caller is ConsolePlayer)
                {
                    player.Experience = player.Experience + commandExp;

                    if (caller is ConsolePlayer)
                    {
                        if (FeexExp.Instance.Translations.Instance.Translate("experience_give_player_console") != "experience_give_player_console")
                        {
                            UnturnedChat.Say(player, FeexExp.Instance.Translations.Instance.Translate("experience_give_player_console", commandExp), Color.yellow);
                        }
                    }
                    else
                    {
                        if (FeexExp.Instance.Translations.Instance.Translate("experience_give_player") != "experience_give_player")
                        {
                            UnturnedChat.Say(player, FeexExp.Instance.Translations.Instance.Translate("experience_give_player", commandExp, caller.DisplayName), Color.yellow);
                        }
                    }
                    
                    if (FeexExp.Instance.Translations.Instance.Translate("experience_give_caller") != "experience_give_caller")
                        {
                        if (caller is ConsolePlayer)
                        {
                            Logger.Log(FeexExp.Instance.Translations.Instance.Translate("experience_give_caller", commandExp, player.DisplayName));
                        }
                        else
                        {
                            UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("experience_give_caller", commandExp, player.DisplayName), Color.yellow);
                        }
                    }
                }
                else if (caller.HasPermission("exp.transfer"))
                {
                    UnturnedPlayer UPcaller = (UnturnedPlayer)caller;
                    if ((Convert.ToDecimal(UPcaller.Experience) - Convert.ToDecimal(commandExp)) < 0)
                    {
                        if (FeexExp.Instance.Translations.Instance.Translate("experience_transfer_not_enough") != "experience_transfer_not_enough")
                        {
                            UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("experience_transfer_not_enough", commandExp), Color.yellow);
                        }
                        return;
                    }

                    UPcaller.Experience = UPcaller.Experience - commandExp;
                    player.Experience = player.Experience + commandExp;

                    if (FeexExp.Instance.Translations.Instance.Translate("experience_transfer_player") != "experience_transfer_player")
                    {
                        UnturnedChat.Say(player, FeexExp.Instance.Translations.Instance.Translate("experience_transfer_player", commandExp, caller.DisplayName), Color.yellow);
                    }
                    if (FeexExp.Instance.Translations.Instance.Translate("experience_transfer_caller") != "experience_transfer_caller")
                    {
                        UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("experience_transfer_caller", commandExp, player.DisplayName), Color.yellow);
                    }
                }
            }
        }

    }
}
