﻿using System.ComponentModel.DataAnnotations;
using Mcce.SmartOffice.Core.Entities;

namespace Mcce.SmartOffice.WorkspaceConfigurations.Entities
{
    public class WorkspaceConfiguration : AuditableEntityBase
    {
        [Key]
        public string ConfigurationNumber { get; set; }

        [Required]
        public long DeskHeight { get; set; }

        [Required]
        public string WorkspaceNumber { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
