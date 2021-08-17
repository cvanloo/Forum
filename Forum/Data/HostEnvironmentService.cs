using Microsoft.Extensions.Hosting;

namespace Forum.Data
{
    public class HostEnvironmentService
    {
        public IHostEnvironment HostEnvironment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostEnvironment">The host environment</param>
        public HostEnvironmentService(IHostEnvironment hostEnvironment)
        {
            HostEnvironment = hostEnvironment;
        }
    }
}