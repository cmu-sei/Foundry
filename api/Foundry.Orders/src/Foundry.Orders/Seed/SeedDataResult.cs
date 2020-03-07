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
using System.Linq;

namespace Foundry.Orders.Api
{
    public class SeedDataResult<TEntity>
        where TEntity : class, new()
    {
        public List<SeedDataResultItem<TEntity>> Items { get; set; } = new List<SeedDataResultItem<TEntity>>();

        public List<TEntity> Entities { get; set; } = new List<TEntity>();

        public int SuccessCount { get { return Items.Count(i => i.Status == SeedDataResultItemStatusType.Success); } }

        public int ExistsCount { get { return Items.Count(i => i.Status == SeedDataResultItemStatusType.Exists); } }

        public Exception Exception { get; set; }
        public string Message { get; set; }
    }

    public class SeedDataResultItem<TEntity>
        where TEntity : class, new()
    {
        public TEntity Entity { get; set; }

        public SeedDataResultItemStatusType Status { get; set; }
    }

    public enum SeedDataResultItemStatusType
    {
        NotSet = 0,
        Success = 1,
        Exists = 2
    }
}

