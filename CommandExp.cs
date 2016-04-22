﻿using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;

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
            get { return "Give or transfer experience"; }
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
            if (command.Length == 1 && !(caller is ConsolePlayer))
            {
                UnturnedPlayer UPcaller = (UnturnedPlayer)caller;
                if (!(caller.HasPermission("exp.self"))) { return; }
                uint commandExp;
                bool isNumeric = uint.TryParse(command[0], out commandExp);
                if (isNumeric)
                {
                    UPcaller.Experience = UPcaller.Experience + commandExp;
                    if (FeexExp.Instance.Translations.Instance.Translate("exp_self") != "exp_self")
                    {
                        UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("exp_self", commandExp));
                    }
                }
            }
            else if (command.Length == 2)
            {
                if (!caller.HasPermission("exp.give") && !caller.HasPermission("exp.transfer") && !(caller is ConsolePlayer)) { return; }
                
                uint commandExp;
                bool isNumeric = uint.TryParse(command[0], out commandExp);
                if (!isNumeric) { return; }

                UnturnedPlayer player = UnturnedPlayer.FromName(command[1]);
                if (player == null)
                {
                    if (FeexExp.Instance.Translations.Instance.Translate("exp_general_not_found") != "exp_general_not_found")
                    {
                        if (caller is ConsolePlayer)
                        {
                            Logger.Log(FeexExp.Instance.Translations.Instance.Translate("exp_general_not_found"));
                        }
                        else
                        {
                            UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("exp_general_not_found"));
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
                        if (FeexExp.Instance.Translations.Instance.Translate("exp_give_player_console") != "exp_give_player_console")
                        {
                            UnturnedChat.Say(player, FeexExp.Instance.Translations.Instance.Translate("exp_give_player_console", commandExp));
                        }
                    }
                    else
                    {
                        if (FeexExp.Instance.Translations.Instance.Translate("exp_give_player") != "exp_give_player")
                        {
                            UnturnedChat.Say(player, FeexExp.Instance.Translations.Instance.Translate("exp_give_player", commandExp, caller.DisplayName));
                        }
                    }
                    
                    if (FeexExp.Instance.Translations.Instance.Translate("exp_give_caller") != "exp_give_caller")
                        {
                        if (caller is ConsolePlayer)
                        {
                            Logger.Log(FeexExp.Instance.Translations.Instance.Translate("exp_give_caller", commandExp, player.DisplayName));
                        }
                        else
                        {
                            UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("exp_give_caller", commandExp, player.DisplayName));
                        }
                    }
                }
                else if (caller.HasPermission("exp.transfer"))
                {
                    UnturnedPlayer UPcaller = (UnturnedPlayer)caller;
                    if ((Convert.ToDecimal(UPcaller.Experience) - Convert.ToDecimal(commandExp)) < 0)
                    {
                        if (FeexExp.Instance.Translations.Instance.Translate("exp_transfer_not_enough") != "exp_transfer_not_enough")
                        {
                            UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("exp_transfer_not_enough", commandExp));
                        }
                        return;
                    }

                    UPcaller.Experience = UPcaller.Experience - commandExp;
                    player.Experience = player.Experience + commandExp;

                    if (FeexExp.Instance.Translations.Instance.Translate("exp_transfer_player") != "exp_transfer_player")
                    {
                        UnturnedChat.Say(player, FeexExp.Instance.Translations.Instance.Translate("exp_transfer_player", commandExp, caller.DisplayName));
                    }
                    if (FeexExp.Instance.Translations.Instance.Translate("exp_transfer_caller") != "exp_transfer_caller")
                    {
                        UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("exp_transfer_caller", commandExp, player.DisplayName));
                    }
                }
            }
            else
            {
                if (FeexExp.Instance.Translations.Instance.Translate("exp_general_invalid_parameter") != "exp_general_invalid_parameter")
                {
                    if (caller is ConsolePlayer)
                    {
                        Logger.Log(FeexExp.Instance.Translations.Instance.Translate("exp_general_invalid_parameter"));
                    }
                    else
                    {
                        UnturnedChat.Say(caller, FeexExp.Instance.Translations.Instance.Translate("exp_general_invalid_parameter"));
                    }
                }
            }
        }
    }
}
