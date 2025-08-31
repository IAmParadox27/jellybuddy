using System.Collections.ObjectModel;
using Jellyfin.Api;

namespace Jellybuddy.Models
{
    public class ActiveSessionGroup : ObservableCollection<SessionInfoDto>
    {
        public string Name { get; set; }
        
        public ActiveSessionGroup(string name, IEnumerable<SessionInfoDto> sessions) : base(sessions)
        {
            Name = name;
        }
    }
}