using UnityEngine.XR.Interaction.Toolkit;

public class XRLocal_DirectInteractor : XRDirectInteractor
{
    /// <summary>
    /// 물체를 잡았을 때 동작하는 메서드.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;
    }

    /// <summary>
    /// 물체를 놓았을 때 동작하는 메서드.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;
    }
}
