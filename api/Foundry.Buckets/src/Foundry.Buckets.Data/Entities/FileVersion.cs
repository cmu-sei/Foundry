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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foundry.Buckets.Data.Entities
{
    public class FileVersion : IAudit
    {
        [Key]
        public int Id { get; set; }

        public bool IsCurrent { get; set; }

        public string GlobalId { get; set; }

        public string Path { get; set; }

        public int VersionNumber { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public string ContentType { get; set; }

        public long Length { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public UploadStatus Status { get; set; }

        public int FileId { get; set; }

        [ForeignKey("FileId")]
        public File File { get; set; }

        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public Account CreatedBy { get; set; }

        public DateTime Created { get; set; }
    }

    public enum UploadStatus
    {
        Initialized,
        Uploading,
        Uploaded,
        Processing,
        Processed,
        Available,
        Failed
    }
}
