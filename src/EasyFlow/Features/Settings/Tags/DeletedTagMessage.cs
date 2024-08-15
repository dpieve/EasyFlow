using CommunityToolkit.Mvvm.Messaging.Messages;
using EasyFlow.Data;

namespace EasyFlow.Features.Settings.Tags;

public sealed class DeletedTagMessage(Tag tag) : ValueChangedMessage<Tag>(tag)
{ }