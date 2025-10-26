public interface IPopupPresenter
{
    // Show text immediately (no queue logic here). Presenter just renders.
    void ShowNow(PopupRequest request);
    // Called when there is nothing to show.
    void Hide();
}
