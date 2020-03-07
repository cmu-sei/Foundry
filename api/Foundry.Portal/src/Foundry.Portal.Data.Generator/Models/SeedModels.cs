/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System.Linq;

namespace Foundry.Portal.Data.Generator.Seed
{
    public class Profile {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GlobalId { get; set; }
    }

    public class Channel {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProfileId { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public int Access { get; set; }
        public string GlobalId { get; set; }
    }
    public class Subscription
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public int ProfileId { get; set; }
        public int Permissions { get; set; }
    }
    public class Group {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProfileId { get; set; }
        public string LogoUrl { get; set; }
        public string GlobalId { get; set; }
    }
    public class Membership
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ProfileId { get; set; }
        public int Permissions { get; set; }
    }

    public class Content
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public int ProfileId { get; set; }
        public int? GroupId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string HoverUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string GlobalId { get; set; }
        public string Tags { get; set; }

        private string _types = "document|video|lab|course|quiz|exercise|event|simulation|webpage|curriculum|game|image";

        public string ContentType {
            get
            {
                string type = _types.Split('|').Intersect(this.Tags.Split('|')).FirstOrDefault();
                return !string.IsNullOrEmpty(type) ? type : "Course";
            }
        }
    }

    public class Playlist {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProfileId { get; set; }
        public string LogoUrl { get; set; }
        public bool IsPublic { get; set; }
        public string GlobalId { get; set; }
    }

    public class ItemLink {
        public int OwnerId { get; set; }
        public int ItemId { get; set; }
    }
}
