/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Stack.Http.Models;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class HomeModel
    {
        public string ApplicationName { get; set; }

        public ApiStatus ApiStatus { get; set; }

        public List<ConfigurationItem> Configuration { get; set; } = new List<ConfigurationItem>();
    }

    public class ConfigurationItem
    {
        List<ConfigurationItemSetting> _settings = new List<ConfigurationItemSetting>();
        public ConfigurationItem()
        {
        }

        public ConfigurationItem(string name, IEnumerable<ConfigurationItemSetting> settings)
        {
            Name = name;
            _settings.AddRange(settings);
        }

        public ConfigurationItem(string name, Dictionary<string, object> settings)
        {
            Name = name;
            _settings.AddRange(settings.Select(s => new ConfigurationItemSetting() { Key = s.Key, Value = s.Value }));
        }

        public string Name { get; set; }
        public List<ConfigurationItemSetting> Settings
        {
            get { return _settings.OrderBy(s => s.Key).ToList(); }
            set { _settings = value; }
        }
    }

    public class ConfigurationItemSetting
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}

