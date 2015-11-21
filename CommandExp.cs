﻿using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Freenex.EasyExp
{
    public class CommandExp : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0 || command.Length > 2)
            {
                return;
            }

            UnturnedPlayer UPcaller = (UnturnedPlayer)caller;

            if (command.Length == 1)
            {
                if (!(caller.HasPermission("exp.self"))) { return; }
                uint commandExp;
                bool isNumeric = uint.TryParse(command[0], out commandExp);
                if (isNumeric)
                {
                    UPcaller.Experience = UPcaller.Experience + commandExp;
                    if (!(EasyExp.Instance.Translations.Instance.Translate("experience_self") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyExp.Instance.Translations.Instance.Translate("experience_self", commandExp), Color.yellow);
                    }
                }
            }

            if (command.Length == 2)
            {
                if (!caller.HasPermission("exp.give") && !caller.HasPermission("exp.transfer")) { return; }

                uint commandExp;
                bool isNumeric = uint.TryParse(command[0], out commandExp);
                if (!isNumeric) { return; }

                UnturnedPlayer player = UnturnedPlayer.FromName(command[1]);
                if (player == null)
                {
                    if (!(EasyExp.Instance.Translations.Instance.Translate("experience_general_not_found") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyExp.Instance.Translations.Instance.Translate("experience_general_not_found"), Color.yellow);
                    }
                    return;
                }

                if (player.Id == caller.Id) { return; }

                if (caller.HasPermission("exp.give"))
                {
                    player.Experience = player.Experience + commandExp;

                    if (!(EasyExp.Instance.Translations.Instance.Translate("experience_give_player") == string.Empty))
                    {
                        UnturnedChat.Say(player, EasyExp.Instance.Translations.Instance.Translate("experience_give_player", commandExp, caller.DisplayName), Color.yellow);
                    }
                    if (!(EasyExp.Instance.Translations.Instance.Translate("experience_give_caller") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyExp.Instance.Translations.Instance.Translate("experience_give_caller", commandExp, player.DisplayName), Color.yellow);
                    }
                }
                else if (caller.HasPermission("exp.transfer"))
                {
                    if ((System.Convert.ToDecimal(UPcaller.Experience) - System.Convert.ToDecimal(commandExp)) < 0)
                    {
                        if (!(EasyExp.Instance.Translations.Instance.Translate("experience_transfer_not_enough") == string.Empty))
                        {
                            UnturnedChat.Say(caller, EasyExp.Instance.Translations.Instance.Translate("experience_transfer_not_enough", commandExp), Color.yellow);
                        }
                        return;
                    }

                    UPcaller.Experience = UPcaller.Experience - commandExp;
                    player.Experience = player.Experience + commandExp;

                    if (!(EasyExp.Instance.Translations.Instance.Translate("experience_transfer_player") == string.Empty))
                    {
                        UnturnedChat.Say(player, EasyExp.Instance.Translations.Instance.Translate("experience_transfer_player", commandExp, caller.DisplayName), Color.yellow);
                    }
                    if (!(EasyExp.Instance.Translations.Instance.Translate("experience_transfer_caller") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyExp.Instance.Translations.Instance.Translate("experience_transfer_caller", commandExp, player.DisplayName), Color.yellow);
                    }
                }
            }
        }

        public string Help
        {
            get { return "Give or transfer Experience"; }
        }

        public string Name
        {
            get { return "exp"; }
        }

        public string Syntax
        {
            get { return "<experience> [<player>]"; }
        }

        public bool AllowFromConsole
        {
            get { return false; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
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
    }
}
