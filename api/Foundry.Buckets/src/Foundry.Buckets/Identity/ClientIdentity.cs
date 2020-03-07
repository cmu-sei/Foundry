/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System.Collections.Generic;

namespace Foundry.Buckets.Identity
{
    /// <summary>
    /// client identity
    /// </summary>
    public class ClientIdentity : IBucketsIdentity
    {
        /// <summary>
        /// client id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// permission
        /// </summary>
        public string[] Permissions { get; set; } = new string[] { };

        /// <summary>
        /// extra data storage
        /// </summary>
        public List<object> Data { get; set; } = new List<object>();

        /// <summary>
        /// claims subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// claims name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// claims client id
        /// </summary>
        public string ClientId { get; set; }
    }
}

