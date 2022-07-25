using System;
using System.Collections.Generic;

namespace MudHutAPI.DAL.DBModels
{
    public partial class Log
    {
        public int Id { get; set; }
        public string LogJson { get; set; } = null!;
    }
}
