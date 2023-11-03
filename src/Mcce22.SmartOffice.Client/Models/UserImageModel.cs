namespace Mcce22.SmartOffice.Client.Models
{
    public class UserImageModel : ModelBase
    {
        public string Url { get; set; }

        public string FileName { get; set; }

        public string ImageKey { get; set; }

        public string UserId { get; set; }

        public bool HasContent { get; set; }
    }
}
