
namespace XClip.Api.Models
{
    public class tag
    {
        public string id { get; set; }

        public string text { get; set; }

        public tag() { }

        public tag(string i, string t) : this()
        {
            this.id = i;
            this.text = t;
        }

    }
}