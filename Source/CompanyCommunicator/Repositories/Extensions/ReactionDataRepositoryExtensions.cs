// <copyright file="ReactionDataRepositoryExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Repositories.Extensions
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ReactionData;

    /// <summary>
    /// Extensions for the repository of the reaction data stored in the table storage.
    /// </summary>
    public static class ReactionDataRepositoryExtensions
    {
        /// <summary>
        /// Add reactions data in Table Storage.
        /// </summary>
        /// <param name="reactionDataRepository">The reaction data repository.</param>
        /// <param name="reaction">User's Reaction.</param>
        /// <param name="activity">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public static async Task SaveReactionDataAsync(
            this IReactionDataRepository reactionDataRepository,
            string reaction,
            IMessageReactionActivity activity)
        {
            System.Diagnostics.Trace.TraceError("Tanya, Inside SaveReactionDataAsync");
            var reactionDataEntity = ReactionDataRepositoryExtensions.ParseReactionData(reaction, activity);
            System.Diagnostics.Trace.TraceError("Tanya, Parsed activity - Reaction");
            System.Diagnostics.Trace.TraceError(reactionDataEntity.Reaction);
            if (reactionDataEntity != null)
            {
                System.Diagnostics.Trace.TraceError("Tanya, calling createorUpdateAsync");
                await reactionDataRepository.CreateOrUpdateAsync(reactionDataEntity);
            }
        }

        /// <summary>
        /// Remove reactions data in table storage.
        /// </summary>
        /// <param name="reactionDataRepository">The reaction data repository.</param>
        /// <param name="reaction">User's Reaction.</param>
        /// <param name="activity">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public static async Task RemoveReactionDataAsync(
            this IReactionDataRepository reactionDataRepository,
            string reaction,
            IMessageReactionActivity activity)
        {
            var reactionDataEntity = ReactionDataRepositoryExtensions.ParseReactionData(reaction, activity);
            if (reactionDataEntity != null)
            {
                var found = await reactionDataRepository.GetAsync(reactionDataEntity.PartitionKey, reactionDataEntity.RowKey);
                if (found != null)
                {
                    System.Diagnostics.Trace.TraceError("Tanya, found, calling DeleteAsync");
                    await reactionDataRepository.DeleteAsync(found);
                }
            }
        }

        private static ReactionDataEntity ParseReactionData(string reaction, IActivity activity)
        {
            if (activity != null)
            {
                var reactionsDataEntity = new ReactionDataEntity
                {
                    PartitionKey = activity.ReplyToId,
                    RowKey = activity.From.Id,
                    ConversationID = activity.Conversation.Id,
                    ReactionId = activity.ReplyToId,
                    Name = activity?.From?.AadObjectId,
                    User = activity?.From?.Id,
                    Reaction = reaction,
                };
                System.Diagnostics.Trace.TraceError("Tanya, Inside ParseReactionData");
                return reactionsDataEntity;
            }

            return null;
        }
    }
}