using System;
using System.ServiceModel.Syndication;

namespace HomeWorkConsoleApp9
{
    [Serializable]
    public struct StructRss
    {
        public string Url { set; get; }
        public string Title { set; get; }
        public string Summary { set; get; }

        public StructRss(SyndicationItem input)
        {
            Url = input.Id;
            Title = input.Title.Text;
            Summary = input.Summary.Text.TrimStart().TrimEnd();
        }
    }
}
