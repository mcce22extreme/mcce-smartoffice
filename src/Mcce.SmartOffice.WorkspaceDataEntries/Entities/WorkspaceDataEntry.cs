﻿using System.ComponentModel.DataAnnotations;
using Mcce.SmartOffice.Core.Entities;

namespace Mcce.SmartOffice.WorkspaceDataEntries.Entities
{
    public class WorkspaceDataEntry : EntityBase
    {
        [Required]
        public string WorkspaceNumber { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public int Wei { get; set; }

        public float Temperature { get; set; }

        public float Co2Level { get; set; }

        public float Humidity { get; set; }
    }
}
