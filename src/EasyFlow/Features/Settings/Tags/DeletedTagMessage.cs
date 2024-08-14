using CommunityToolkit.Mvvm.Messaging.Messages;

namespace EasyFlow.Features.Settings.Tags;

public sealed class DeletedTagMessage(Tag tag) : ValueChangedMessage<Tag>(tag)
{ }