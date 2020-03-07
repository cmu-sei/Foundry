/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Foundry.Portal.Data;
using System;

namespace Foundry.Portal.Events
{
    /// <summary>
    /// an action taken by the application
    /// </summary>
    public class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// unique id for the event
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// the time the event was created
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// the entity global id related to the event
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// the parent entity global id related to the event
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// the entity name related to the event
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// the type of event raised
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DomainEventType Type { get; set; }

        public DomainEvent()
        {
            Key = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }

        public DomainEvent(string id, string name, DomainEventType type)
            : this()
        {
            Guid parsedId = new Guid();

            if (!Guid.TryParse(id, out parsedId))
            {
                throw new ArgumentException("id is not a GUID");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (type == DomainEventType.NotSet)
            {
                throw new ArgumentNullException("type");
            }

            Id = parsedId;
            Name = name;
            Type = type;
        }

        public DomainEvent(string id, string name, string parentId, DomainEventType type)
            : this(id, name, type)
        {
            Guid parsedParentId = new Guid();

            if (!Guid.TryParse(parentId, out parsedParentId))
            {
                throw new ArgumentException("parentId is not a GUID");
            }

            ParentId = parsedParentId;
        }
    }
}
