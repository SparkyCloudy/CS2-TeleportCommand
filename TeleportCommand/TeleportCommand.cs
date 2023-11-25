﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;

namespace TeleportCommand;
public class TeleportCommand : BasePlugin
{
    public override string ModuleName => "CS2 Teleport Command";
    public override string ModuleVersion => "1.1.0";
    public override string ModuleAuthor => "Oylsister, Sparky";
    public override string ModuleDescription => "Advanced Teleport Command for Counter-Strike 2";

    private readonly IFindTargetModule _findTarget = new FindTargetModule();

    public override void Load(bool hotReload)
    {
        AddCommand("teleport", "Teleport Player to Player", Command_Teleport);
        AddCommand("tele", "Teleport Player to Player", Command_Teleport);
        AddCommand("bring", "Bring Player To User", Command_Bring);
        AddCommand("goto", "Bring User To Player", Command_Goto);
    }

    [RequiresPermissions("@css/slay")]
    private void Command_Teleport(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand("[Teleport] Usage: css_teleport <client> <dest-client>");
            return;
        }

        // Get the client name that you want to be teleported.
        var targetname = _findTarget.FindTarget(client, info.GetArg(1));

        // Get the client name that you want to be destination.
        var destTemp = _findTarget.FindTarget(client, info.GetArg(2), true);

        if (targetname.Count <= 0)
        {
            info.ReplyToCommand("[Teleport] Invalid target name.");
            return;
        }

        if (destTemp.Count <= 0)
        {
            info.ReplyToCommand("[Teleport] Invalid target destination name.");
            return;
        }

        // target destination can only be one.
        var targetdestination = destTemp[0];

        // Find the angle and position.
        var playerPawn = targetdestination.PlayerPawn.Value;
        var position = playerPawn.AbsOrigin ?? new Vector(default);
        var angle = playerPawn.AbsRotation ?? new QAngle(default);
        var velocity = playerPawn.AbsVelocity;

        foreach (var targetPawn in targetname.Select(player => player.PlayerPawn.Value))
        {
            targetPawn.Teleport(position, angle, velocity);
        }

        info.ReplyToCommand("[Teleport] Successfully Teleport Client.");
    }

    [RequiresPermissions("@css/slay")]
    private void Command_Bring(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 1)
        {
            info.ReplyToCommand("[Teleport] Usage: css_bring <client>");
            return;
        }


        // Get the client name that you want to be teleported.
        var target = info.GetArg(1);
        var targetname = _findTarget.FindTarget(client, target);

        if (targetname.Count <= 0)
        {
            info.ReplyToCommand("[Teleport] Invalid target name.");
            return;
        }

        // Find the angle and position.
        var playerPawn = client?.PlayerPawn.Value;
        var position = playerPawn?.AbsOrigin;
        var angle = playerPawn?.AbsRotation;
        var velocity = playerPawn?.AbsVelocity;

        foreach (var targetPawn in targetname.Select(player => player.PlayerPawn.Value))
        {
            var name = targetPawn.Controller.Value.PlayerName;
            targetPawn.Teleport(position!, angle!, velocity!);
            info.ReplyToCommand($"[Teleport] Successfully Teleport bring { name } to you.");
        }
    }
    
    [RequiresPermissions("@css/slay")]
    private void Command_Goto(CCSPlayerController? client, CommandInfo command)
    {
        if (command.ArgCount <= 1)
        {
            command.ReplyToCommand("[Teleport] Usage: css_goto <client>");
            return;
        }

        var targetName = command.GetArg(1);
        var target = _findTarget.FindTarget(client, targetName).First();

        if (!target.IsValid)
        {
            command.ReplyToCommand("[Teleport] Invalid target name.");
            return;
        }
        
        // Find the angle and position.
        var targetPawn = target.PlayerPawn.Value;
        var position = targetPawn.AbsOrigin;
        var angle = targetPawn.AbsRotation;
        var velocity = targetPawn.AbsVelocity;
        client?.Teleport(position!, angle!, velocity);
        command.ReplyToCommand($"[Teleport] Successfully Teleport goto you to { target.PlayerName }.");
    }
}
