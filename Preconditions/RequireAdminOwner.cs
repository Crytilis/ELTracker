using Discord;
using Discord.Interactions;

namespace ELTracker.Preconditions;

public class RequireAdminOwner : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        var botInfo = await context.Client.GetApplicationInfoAsync();
        if (context.User is IGuildUser { GuildPermissions.Administrator: true })
        {
            return await Task.FromResult(PreconditionResult.FromSuccess());
        }

        if (context.User.Id == botInfo.Owner.Id)
        {
            return await Task.FromResult(PreconditionResult.FromSuccess());
        }

        return await Task.FromResult(PreconditionResult.FromError("You are not authorized to use this command"));
    }
}