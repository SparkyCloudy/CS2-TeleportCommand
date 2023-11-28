﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;

namespace TeleportCommand;
public class TeleportCommand : BasePlugin
{
    public override string ModuleName => "CS2 Teleport Command";
    public override string ModuleVersion => "1.1.1";
    public override string ModuleAuthor => "Oylsister, Sparky";
    public override string ModuleDescription => "Advanced Teleport Command for Counter-Strike 2";

    public override void Load(bool hotReload)
    {
        AddCommand("teleport", "Teleport Player to Player", Command_Teleport);
        AddCommand("tele", "Teleport Player to Player", Command_Teleport);
        AddCommand("bring", "Bring Player To User", Command_Bring);
        AddCommand("goto", "Bring User To Player", Command_Goto);
    }

    [RequiresPermissions("@css/slay")]
    private void Command_Teleport(CCSPlayerController? client, CommandInfo command)
    {
        if (client == null)
        {
            command.ReplyToCommand("[Teleport] The command cannot be used through the server console");
            return;
        }
        
        if (command.ArgCount <= 2)
        {
            command.ReplyToCommand("[Teleport] Usage: css_teleport <client> <dest-client>");
            return;
        }

        // Get the client name that you want to be teleported.
        var target = command.GetArgTargetResult(1);

        // Get the client name that you want to be destination.
        var destTarget = command.GetArgTargetResult(2);

        if (target == null)
        {
            command.ReplyToCommand("[Teleport] Invalid target name.");
            return;
        }

        if (destTarget == null)
        {
            command.ReplyToCommand("[Teleport] Invalid target destination name.");
            return;
        }

        // Find the angle and position.
        var playerPawn = destTarget.Players.First().PlayerPawn.Value;
        var position = playerPawn.AbsOrigin ?? new Vector(default);
        var angle = playerPawn.AbsRotation ?? new QAngle(default);
        var velocity = playerPawn.AbsVelocity;

        foreach (var targetPawn in target.Players.Select(player => player.PlayerPawn.Value))
        {
            targetPawn.Teleport(position, angle, velocity);
        }

        command.ReplyToCommand("[Teleport] Successfully Teleport Client.");
    }

    [RequiresPermissions("@css/slay")]
    private void Command_Bring(CCSPlayerController? client, CommandInfo command)
    {
        if (client == null)
        {
            command.ReplyToCommand("[Teleport] The command cannot be used through the server console");
            return;
        }
        
        if (command.ArgCount <= 1)
        {
            command.ReplyToCommand("[Teleport] Usage: css_bring <client>");
            return;
        }


        // Get the client name that you want to be teleported.
        var target = command.GetArgTargetResult(1);

        if (target.Players.Count <= 0)
        {
            command.ReplyToCommand("[Teleport] Invalid target name.");
            return;
        }

        // Find the angle and position.
        var playerPawn = client.PlayerPawn.Value;
        var position = playerPawn.AbsOrigin!;
        var angle = playerPawn.AbsRotation!;
        var velocity = playerPawn.AbsVelocity;

        foreach (var targetPawn in target.Players.Select(player => player.PlayerPawn.Value))
        {
            var name = targetPawn.Controller.Value.PlayerName;
            targetPawn.Teleport(position, angle, velocity);
            command.ReplyToCommand($"[Teleport] Successfully Teleport bring { name } to you.");
        }
    }
    
    [RequiresPermissions("@css/slay")]
    private void Command_Goto(CCSPlayerController? client, CommandInfo command)
    {
        if (client == null)
        {
            command.ReplyToCommand("[Teleport] The command cannot be used through the server console");
            return;
        }
        
        if (command.ArgCount <= 1)
        {
            command.ReplyToCommand("[Teleport] Usage: css_goto <client>");
            return;
        }

        var targetArg = command.GetArgTargetResult(1);

        if (targetArg.Players.Count <= 0)
        {
            command.ReplyToCommand("[Teleport] Invalid target name.");
            return;
        }

        var target = targetArg.Players.First();

        var targetPawn = target.PlayerPawn.Value;
        var clientPawn = client.PlayerPawn.Value;
        
        // Find the angle and position.
        var position = targetPawn.AbsOrigin!;
        var angle = targetPawn.AbsRotation!;
        var velocity = targetPawn.AbsVelocity;
        
        clientPawn.Teleport(position, angle, velocity);
        
        command.ReplyToCommand($"[Teleport] Successfully Teleport goto you to { target.PlayerName }.");
    }
}
