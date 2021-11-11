using System.Threading.Tasks;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel
    {
        private async void run()
        {
            while (true)
            {
                await Task.Delay(100);
            }
        }
    }
}