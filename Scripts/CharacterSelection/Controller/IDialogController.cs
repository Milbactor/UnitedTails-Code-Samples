namespace WhiteKNight
{
    public interface IDialogController
    {
        void ShowCanvas();
        void HideCanvas();

        void SetButtons(string id1, string id2);
        void ResetButtons();
    }
}