/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.ViewModels;
using System.Linq;
using System.Collections.Generic;
using Foundry.Buckets.Data;
using Stack.Http.Identity;
using Foundry.Buckets.Extensions;

namespace Foundry.Buckets.Mapping
{
    /// <summary>
    /// auto mapper profile for files
    /// </summary>
    public class FileProfile : Profile
    {
        /// <summary>
        /// create an instance of file profile
        /// </summary>
        public FileProfile()
        {
            CreateMap<File, FileDetail>()
                .ForMember(dest => dest.BucketName, opt => opt.MapFrom(src => src.Bucket == null ? "" : src.Bucket.Name))
                .ForMember(dest => dest.BucketSlug, opt => opt.MapFrom(src => src.Bucket == null ? "" : src.Bucket.Slug))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.CurrentVersionNumber))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.FileTags.Any()
                        ? src.FileTags.OrderBy(ft => ft.Tag.Name).Select(ft => ft.Tag.Name).ToList()
                        : new List<string>()))
                .AfterMap((src, dest, res) => {
                    var current = src.FileVersions.SingleOrDefault(fv => fv.IsCurrent);

                    if (current != null)
                    {
                        dest.ContentType = current.ContentType;
                        dest.Extension = current.Extension;
                        dest.Length = current.Length;
                        dest.CreatedByName = current.CreatedBy == null ? "" : current.CreatedBy.Name;
                    }

                    dest.Access = res.GetIdentity()?.SetAccess(src);

                });

            CreateMap<File, FileSummary>()
                .ForMember(dest => dest.BucketName, opt => opt.MapFrom(src => src.Bucket == null ? "" : src.Bucket.Name))
                .ForMember(dest => dest.BucketSlug, opt => opt.MapFrom(src => src.Bucket == null ? "" : src.Bucket.Slug))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.CurrentVersionNumber))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.FileTags.Any()
                        ? src.FileTags.OrderBy(ft => ft.Tag.Name).Select(ft => ft.Tag.Name).ToList()
                        : new List<string>()))
                .AfterMap((src, dest, res) => {
                    var current = src.FileVersions.SingleOrDefault(fv => fv.IsCurrent);

                    if (current != null)
                    {
                        dest.ContentType = current.ContentType;
                        dest.Extension = current.Extension;
                        dest.Length = current.Length;
                        dest.CreatedByName = current.CreatedBy == null ? "" : current.CreatedBy.Name;
                    }

                    dest.Access = res.GetIdentity()?.SetAccess(src);
                });
        }
    }
}

