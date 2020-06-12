using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeamlessLaunchpad.Models;

namespace SeamlessLaunchpad.Controllers
{
    public class LaunchpadController : ControllerBase
    {
        private readonly SLPADDBContext _context;
        private static readonly string ApiKey;
        
        public LaunchpadController(SLPADDBContext context)
        {
            _context = context;
        }

        static LaunchpadController()
        {
            ApiKey = new StreamReader(System.IO.File.OpenRead("api.txt")).ReadToEnd().Trim('\n');
        }
    }
}