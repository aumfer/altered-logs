using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace Altered.Logs
{
    public sealed class AlteredLog
    {
        public DateTime Time { get; set; }

        public string App { get; set; }

        public string Env { get; set; }

        public string Sha { get; set; }

        public JObject Log { get; set; }

        internal DateTime AlteredTime { get; set; }
    }
}
