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
using Foundry.Communications.Data.Entities;
using Foundry.Communications.ViewModels;
using System.Linq;

namespace Foundry.Communications.Mapping
{
    /// <summary>
    /// notification auto mapper profile
    /// </summary>
    public class NotificationProfile : Profile
    {
        /// <summary>
        /// create an instance of notification profile
        /// </summary>
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationSummary>()
                .AfterMap((src, dest, res) => {
                    var profile = res.GetIdentity();
                    if (profile != null)
                    {
                        var globalId = profile.Id.ToLower();
                        var recipient = src.Recipients.FirstOrDefault(r => r.TargetId.ToLower() == globalId);

                        if (recipient != null)
                        {
                            dest.Read = recipient.Read;
                        }
                    }
                });

            CreateMap<NotificationValue, NotificationSummaryValue>();
        }
    }
}
