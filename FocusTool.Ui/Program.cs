namespace FocusTool.Ui;

static class Program
{
    [STAThread]
    static void Main()
    {
        using var coordinator = AppInstanceCoordinator.Create();
        if (!coordinator.IsPrimaryInstance)
        {
            coordinator.ActivateExistingInstance();
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
