﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace Mcce22.SmartOffice.Client.Models
{
    public partial class WorkspaceModel : ObservableObject, IModel
    {
        public string Id { get; set; }

        public string WorkspaceNumber { get; set; }

        public string RoomNumber { get; set; }

        public bool? IsAvailable { get; set; }

        public int Top { get; set; }

        public int Left { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        [ObservableProperty]
        private int _wei;
    }
}
