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
using System.Text;

namespace Foundry.Portal.Data
{
    /// <summary>
    /// the types of domain events raised
    /// </summary>
    public enum DomainEventType
    {
        NotSet = 0,
        ContentAdd = 14,
        ContentUpdate = 15,
        ContentDelete = 16,
        ContentRate = 17,
        ContentLevel = 18,
        PlaylistAdd = 19,
        PlaylistUpdate = 20,
        PlaylistDelete = 21,
        PlaylistContentAdd = 22,
        PlaylistContentDelete = 23,
        PlaylistFollow = 24,
        PlaylistUnfollow = 25,
        PlaylistGroupFollow = 26,
        PlaylistGroupUnfollow = 27,
        PlaylistRate = 28,
        ContentFlagged = 29,
        ProfilePermissionGranted = 30,
        System = 31,
        ProfilePost = 34
    }
}
