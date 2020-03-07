/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System;
using System.Collections.Generic;

namespace Foundry.Buckets.ViewModels
{
    public class FileDetail
    {
        public int Id { get; set; }
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Extension { get; set; }
        public long Length { get; set; }
        public string Url
        {
            get { return string.Format("/f/{0}", GlobalId); }
        }

        public string UrlWithExtension
        {
            get { return string.Format("/f/{0}.{1}", GlobalId, Extension); }
        }
        public string ContentType { get; set; }
        public string BucketName { get; set; }
        public string BucketSlug { get; set; }
        public int BucketId { get; set; }
        public int VersionNumber { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> Access { get; set; } = new List<string>();

        public string CreatedByName { get; set; }
        public DateTime Created { get; set; }
    }
}
