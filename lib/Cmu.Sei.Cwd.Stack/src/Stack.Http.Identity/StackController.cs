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
using Microsoft.AspNetCore.Mvc;
using System;

namespace Stack.Http.Identity
{
    public abstract class StackController : Controller
    {
        protected IStackIdentityResolver IdentityResolver { get; }
        IStackIdentity _identity;

        public StackController(IStackIdentityResolver identityResolver)
        {
            IdentityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));

            _identity = IdentityResolver.GetIdentityAsync().Result;
        }

        public IStackIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    _identity = IdentityResolver.GetIdentityAsync().Result;
                }

                return _identity;
            }
        }
        public IActionResult Ok<TEntityModel, TViewModel>(TEntityModel entity)
        {
            return Ok(Mapper.Map<TEntityModel, TViewModel>(entity));
        }

        public IActionResult Created<TEntityModel, TViewModel>(string uri, TEntityModel entity)
        {
            return Created(uri, Mapper.Map<TEntityModel, TViewModel>(entity));
        }
    }
}
