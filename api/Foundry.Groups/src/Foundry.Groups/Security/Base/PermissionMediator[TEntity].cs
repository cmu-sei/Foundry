/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Stack.Http.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.Security
{
    public abstract class PermissionMediator<TEntity>
        where TEntity : class
    {
        internal IStackIdentityResolver _identityResolver;

        IStackIdentity _identity;
        public IStackIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    try
                    {
                        _identity = _identityResolver.GetIdentityAsync().Result;

                    }
                    catch { }
                }

                return _identity;
            }
        }

        public bool IsAdministrator
        {
            get
            {
                return Identity != null && Identity.Permissions.Contains("administrator");
            }
        }

        public PermissionMediator(IStackIdentityResolver identityResolver)
        {
            _identityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
        }

        public virtual IQueryable<TEntity> Process(IQueryable<TEntity> query)
        {
            return query;
        }

        public abstract bool CanUpdate(TEntity entity);

        public abstract bool CanDelete(TEntity entity);

        public abstract bool CanAdd(TEntity entity);
    }
}

