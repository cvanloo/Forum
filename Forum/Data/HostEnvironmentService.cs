using Microsoft.Extensions.Hosting;

namespace Forum.Data
{
    public class HostEnvironmentService
    {
        public IHostEnvironment HostEnvironment { get; set; }

        public HostEnvironmentService(IHostEnvironment hostEnvironment)
        {
            HostEnvironment = hostEnvironment;
        }
    }
}