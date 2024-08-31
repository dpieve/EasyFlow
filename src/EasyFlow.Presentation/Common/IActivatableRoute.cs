namespace EasyFlow.Presentation.Common;

public interface IActivatableRoute
{
    void OnActivated();

    void OnDeactivated();
}